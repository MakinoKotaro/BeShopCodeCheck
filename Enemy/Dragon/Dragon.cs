using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// ドラゴンの敵キャラ挙動クラス
/// </summary>
public class Dragon : Enemy, IE_SpecialMove
{
    #region 定数
    private const float ReachHeightThreshold = 5f;      // 飛行高度の上限
    private const int RockShotCount = 3;                // 岩の発射回数
    private const float RockShotInterval = 0.8f;        // 岩の発射間隔
    private const float PostAttackDelay = 5f;           // 攻撃後の待機時間
    private const float DropMoneyAmount = 1000f;        // ドロップするお金の額
    #endregion

    #region 依存オブジェクト
    [SerializeField] private GameObject enemyManagerObj;
    [SerializeField] private GameObject dragonHpManagerObj;
    [SerializeField] private GameObject loadSceneObj;
    [SerializeField] private GameObject popGetMoneyObj;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform rockCastPoint;
    #endregion

    #region ステータス
    [SerializeField] private float dragonHealth;
    [SerializeField] private int dragonAttackPower;
    [SerializeField] private float dragonMoveSpeed = 0f;
    [SerializeField] private float takeOffForce;
    [SerializeField] private float rockShotForce;
    [SerializeField] private float destroyDelay = 2.0f;
    [SerializeField] private float distanceToPlayer = 3.0f;
    #endregion

    #region アニメーション名
    [SerializeField] private string idleAnimation;
    [SerializeField] private string attackAnimation;
    [SerializeField] private string deadAnimation;
    [SerializeField] private string takeOffAnimation;
    [SerializeField] private string landAnimation;
    [SerializeField] private string flyIdleAnimation;
    #endregion

    #region プライベート変数
    private EnemyManager enemyManager;
    private DragonHpManager dragonHpManager;
    private LoadScene loadScene;
    private PlayerParameter playerParameter;
    private GameObject player;
    private GameObject playerInventoryObj;
    private Rigidbody rigidbody;
    private Animator animator;
    private Transform targetPosition;
    private JSONManager jsonManager;

    private float dragonMaxHp;
    private float currentHp;
    private bool playerIsHere = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool reachLimit = false;
    private bool endStageFlag = false;
    #endregion

    #region プロパティ
    // 最大HP（読み取り専用）
    public float DragonMaxHp { get => dragonMaxHp; private set => dragonMaxHp = value; }

    // 現在HP
    public float DragonHealth { get => dragonHealth; set => dragonHealth = value; }
    #endregion

    #region Unityイベント
    private void Start()
    {
        // 各コンポーネントと初期値の設定
        animator = GetComponent<Animator>();
        health = dragonHealth;
        attackPower = dragonAttackPower;
        canMove = true;
        dragonMaxHp = dragonHealth;

        // プレイヤーオブジェクトの取得
        player = GameObject.FindWithTag("Player");
        playerInventoryObj = GameObject.FindWithTag("PlayerInventory");

        // コンポーネント取得
        rigidbody = GetComponent<Rigidbody>();
        jsonManager = player.GetComponent<JSONManager>();
        enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
    }

    private void Update()
    {
        // HPバー表示の更新
        currentHp = health / dragonMaxHp;
        dragonHpManager = dragonHpManagerObj.GetComponent<DragonHpManager>();
        dragonHpManager.ShowCurrentHp(currentHp);

        // 高さ制限のチェック
        reachLimit = transform.position.y >= ReachHeightThreshold;
    }
    #endregion

    #region Enemy基底クラスの実装

    /// <summary>
    /// プレイヤーが発見されたかによって移動やアニメーションを制御する。
    /// </summary>
    public override void EnemyMove(bool foundPlayer, Vector3 playerPosition)
    {
        // 動けない状態なら以下の処理を呼ばない
        if (!canMove) return;

        // プレイヤーを見つけたか同かをフラグに保存
        playerIsHere = foundPlayer;

        // プレイヤーを見つけていなかったら
        if (!foundPlayer)
        {
            // 停止アニメーション再生
            animator.Play(idleAnimation);
        }
    }

    /// <summary>
    /// 攻撃開始処理。攻撃中でなければ攻撃コルーチンを開始する。
    /// </summary>
    public override void EnemyAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    /// <summary>
    /// 攻撃を中断する（コルーチン停止）。
    /// </summary>
    public override void EnemyStopAttack()
    {
        StopCoroutine(Attack());
    }

    /// <summary>
    /// ダメージを受けてHPを減らし、死亡判定があれば死亡処理を呼ぶ。
    /// </summary>
    public override void EnemyTakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            EnemyDead();
        }
    }

    /// <summary>
    /// 死亡時のアニメーションと報酬UI表示、リザルト画面遷移処理。
    /// </summary>
    public override void EnemyDead()
    {
        // 死亡アニメーション再生
        animator.Play(deadAnimation);

        // お金を獲得するUIを表示
        PopGetMoneyUi popGetMoneyUi = popGetMoneyObj.GetComponent<PopGetMoneyUi>();
        popGetMoneyUi.ShowUi((int)DropMoneyAmount);

        // JSONデータ更新
        jsonManager.PlayerInfo.NowMoney += (int)DropMoneyAmount;


        playerParameter = player.GetComponent<PlayerParameter>();

        // フラグ更新
        canMove = false;
        isDead = true;
        endStageFlag = true;

        // ディレイをかけてオブジェクト破棄
        Invoke(nameof(DestroyObj), destroyDelay);
    }

    #endregion

    #region コルーチンと特殊行動

    /// <summary>
    /// ドラゴンの攻撃動作。飛行→岩の発射→着地までを制御するコルーチン。
    /// </summary>
    private IEnumerator Attack()
    {
        // 死んでいたら以下の処理を呼ばない
        if (isDead) yield break;

        // 攻撃している状態とする
        isAttacking = true;

        // SE再生
        SFXManager sfxManager = GameObject.FindWithTag("SFXManager").GetComponent<SFXManager>();
        sfxManager.SetSwingAttackSound();

        // 飛翔処理
        animator.Play(takeOffAnimation);
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(Vector3.up * takeOffForce, ForceMode.VelocityChange);

        // 上昇中待機する
        yield return new WaitUntil(() => reachLimit);

        // 重力を木って、浮遊状態のアニメーション再生
        rigidbody.useGravity = false;
        animator.Play(flyIdleAnimation);

        // １秒待って
        yield return new WaitForSeconds(1.0f);

        // プレイヤー検知スクリプトを取得
        EnemySearchPlayer enemySearchPlayer = GetComponent<EnemySearchPlayer>();

        // ３回岩発射
        for (int i = 0; i < RockShotCount; i++)
        {
            // アニメーション再生
            animator.Play(attackAnimation);

            // プレイヤーの現在の位置を目標にする
            Vector3 playerPosition = enemySearchPlayer.PlayerPosition;
            if (targetPosition == null)
            {
                targetPosition = new GameObject("TargetPosition").transform;
            }
            targetPosition.position = playerPosition;

            // 発射関数を呼ぶ
            ShotRock(targetPosition);

            // インターバルを設ける
            yield return new WaitForSeconds(RockShotInterval);
        }

        // １秒待って
        yield return new WaitForSeconds(1.0f);

        // 浮遊状態のアニメーション再生
        animator.Play(flyIdleAnimation);

        // １秒待って
        yield return new WaitForSeconds(1.0f);


        // 重量を有効にして落下させる
        rigidbody.useGravity = true;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        // 着地アニメーション再生
        animator.Play(landAnimation);


        // 着地の時間待機する
        reachLimit = false;
        float landAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(landAnimationLength);
        yield return new WaitForSeconds(PostAttackDelay);

        // 攻撃終了
        rigidbody.velocity = Vector3.zero;
        isAttacking = false;
    }

    /// <summary>
    /// 指定ターゲット位置に向かって岩を発射する。
    /// </summary>
    private void ShotRock(Transform target)
    {
        // 岩生成
        GameObject rock = Instantiate(rockPrefab, rockCastPoint.position, Quaternion.identity);

        // 方向指定
        Vector3 direction = (target.position - rockCastPoint.position).normalized;
        Rigidbody rockRb = rock.GetComponent<Rigidbody>();

        if (rockRb != null)
        {
            // 発射
            rockRb.AddForce(direction * rockShotForce, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// ドラゴン自身を削除し、リザルトシーンへの遷移を実行する。
    /// </summary>
    private void DestroyObj()
    {
        // オブジェクトを破棄し、シーンロード
        Destroy(gameObject);
        loadScene = loadSceneObj.GetComponent<LoadScene>();
        loadScene.LoadResultScene();
    }

    /// <summary>
    /// 特殊行動の実装。現在は未使用だが、拡張用として定義。
    /// </summary>
    public void EnemySpecialMove()
    {
        // 特殊アクションを追加する予定
    }

    #endregion
}
