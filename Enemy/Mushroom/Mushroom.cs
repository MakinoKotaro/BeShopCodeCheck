using System.Collections;
using UnityEngine;

/// <summary>
/// 敵：キノコの挙動スクリプト
/// </summary>
public class Mushroom : Enemy, IE_SpecialMove, IE_GetFroze
{
    #region 定数
    private const float DefaultDestroyDelay = 1.8f;           // 死亡後にオブジェクトが削除されるまでの遅延時間
    private const float DefaultAttackInterval = 5.0f;         // 攻撃のインターバル（秒）
    private const float DefaultPopPoisonDelay = 0.5f;         // 毒を吐くまでの待機時間
    private const float DefaultDistanceToPlayer = 3.0f;       // プレイヤーとの接近距離の閾値
    private const int DefaultDropMoney = 200;                 // 倒した際に得られるお金の量
    private const float ResetPoisonTime = 1.0f;               // 毒を再度撃てるようになるまでの時間
    #endregion

    #region SerializeField変数
    [SerializeField] private GameObject enemyManagerObj;      // 敵マネージャーの参照
    [SerializeField] private float mushroomHealth;            // キノコの体力
    [SerializeField] private int mushroomAttackPower;         // キノコの攻撃力
    [SerializeField] private float mushroomMoveSpeed = 5f;    // キノコの移動速度
    [SerializeField] private string idleAnimation;            // 待機アニメーション名
    [SerializeField] private string attackAnimation;          // 攻撃アニメーション名
    [SerializeField] private string walkAnimation;            // 歩行アニメーション名
    [SerializeField] private string deadAnimation;            // 死亡アニメーション名
    [SerializeField] private string hurtAnimation;            // 被ダメージ時のアニメーション名
    [SerializeField] private ParticleSystem poisonParticle;   // 毒のパーティクルエフェクト
    [SerializeField] private GameObject popGetMoneyObj;       // お金UI表示プレハブ
    [SerializeField] private GameObject icePrefab;            // 氷のエフェクトプレハブ
    [SerializeField] private GameObject sfxManagerObj;        // 効果音マネージャーの参照
    #endregion

    #region プライベート変数
    private EnemyManager enemyManager;                        // 敵マネージャーのコンポーネント
    private PoisonController poisonController;                // 毒攻撃の管理コンポーネント
    private JSONManager jsonManager;                          // セーブデータ管理
    private SFXManager sfxManager;                            // 効果音管理

    private GameObject player;                                // プレイヤーの参照
    private GameObject playerInventoryObj;                    // プレイヤーのインベントリオブジェクト
    private Animator animator;                                // アニメーターコンポーネント

    private bool playerIsHere = false;                        // プレイヤーが近くにいるかどうかのフラグ
    private bool canShotPoison = true;                        // 毒を撃てる状態かどうか
    private float destroyDelay = DefaultDestroyDelay;         // オブジェクト削除までの遅延時間
    private float attackInterval = DefaultAttackInterval;     // 攻撃間隔
    private float popPoisonDelay = DefaultPopPoisonDelay;     // 毒発射までの待機時間
    private float distanceToPlayer = DefaultDistanceToPlayer; // プレイヤーとの接近距離
    private int dropMoneyAmount = DefaultDropMoney;           // ドロップするお金の量
    #endregion

    #region プロパティ
    /// <summary>
    /// 毒を吐けるかどうかのフラグ（外部から変更可能）
    /// </summary>
    public bool CanShotPoison
    {
        get => canShotPoison;
        set => canShotPoison = value;
    }
    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // コンポーネント取得と初期値設定
        animator = GetComponent<Animator>();
        health = mushroomHealth;
        attackPower = mushroomAttackPower;
        canMove = true;

        if (enemyManagerObj != null)
            enemyManager = enemyManagerObj.GetComponent<EnemyManager>();

        poisonController = GetComponent<PoisonController>();

        if (player != null)
            jsonManager = player.GetComponent<JSONManager>();

        if (sfxManagerObj != null)
            sfxManager = sfxManagerObj.GetComponent<SFXManager>();

        // プレイヤー関連オブジェクトを取得
        player = GameObject.FindWithTag("Player");
        playerInventoryObj = GameObject.FindWithTag("PlayerInventory");

        // 氷生成時のオフセットを設定
        iceOffset = new Vector3(0, 1.1f, 0);
    }
    #endregion

    #region Enemy継承処理

    /// <summary>
    /// プレイヤーの発見状況に応じた移動処理を行う。
    /// </summary>
    public override void EnemyMove(bool foundPlayer, Vector3 playerPosition)
    {
        // 動けない状態なら以下の処理を呼ばない
        if (!canMove) return;

        // プレイヤーを検知したかをフラグに保存
        playerIsHere = foundPlayer;

        if (foundPlayer)
        {
            // 見つけたら移動する
            StartCoroutine(Move(playerPosition));
        }
        else
        {
            // 見つけていないなら停止アニメーション再生
            animator.Play(idleAnimation);
        }
    }

    /// <summary>
    /// 攻撃アニメーションと毒攻撃処理を開始する。
    /// </summary>
    public override void EnemyAttack()
    {
        StartCoroutine(Attack());
    }

    /// <summary>
    /// 攻撃コルーチンを停止する。
    /// </summary>
    public override void EnemyStopAttack()
    {
        StopCoroutine(Attack());
    }

    /// <summary>
    /// ダメージを受けた際の処理（体力減少・死亡判定など）。
    /// </summary>
    public override void EnemyTakeDamage(float damageAmount)
    {
        // 攻撃を受けるアニメーション再生
        animator.Play(hurtAnimation);

        // ダメージを受ける
        health -= damageAmount;

        // SE再生
        sfxManager?.SetMushroomTakeDamageSound();


        // HPが0以下で死んだ処理を呼ぶ
        if (health <= 0)
        {
            EnemyDead();
        }
    }

    /// <summary>
    /// 死亡時のアニメーションと報酬表示、オブジェクト削除を行う。
    /// </summary>
    public override void EnemyDead()
    {
        // 死亡アニメーション再生
        animator.Play(deadAnimation);

        // 動けない状態にする
        canMove = false;

        // お金獲得UIを表示
        PopGetMoneyUi popUi = popGetMoneyObj.GetComponent<PopGetMoneyUi>();
        popUi.ShowUi(dropMoneyAmount);

        // JSONデータ更新
        jsonManager.PlayerInfo.NowMoney += dropMoneyAmount;

        // 送らせてオブジェクトを破棄
        Invoke(nameof(DestroyObj), destroyDelay);
    }

    #endregion

    #region カスタム処理

    /// <summary>
    /// 敵オブジェクトを削除する。
    /// </summary>
    private void DestroyObj()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 毒攻撃（特殊行動）を実行する。
    /// </summary>
    public void EnemySpecialMove()
    {
        // 毒を撃てない状態にする
        canShotPoison = false;

        // パーティクル生成
        poisonParticle.Play();

        StartCoroutine(SpecialAttack());
    }

    /// <summary>
    /// 凍結状態にする。すべての挙動を停止。
    /// </summary>
    public void EnemyGetFroze()
    {
        if (icePrefab == null)
        {
            Debug.LogError("Iceプレハブがアタッチされていません！");
            return;
        }

        // 氷を出現させる位置を指定
        Vector3 spawnPosition = transform.position + iceOffset;
        
        // 氷生成
        GameObject newIce = Instantiate(icePrefab, spawnPosition, Quaternion.identity);

        // 氷を親にする
        transform.SetParent(newIce.transform);


        // 全コンポーネント停止
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component is Transform) continue;

            if (component is MonoBehaviour mono)
            {
                mono.enabled = false;
            }
            else if (component is Behaviour behaviour)
            {
                behaviour.enabled = false;
            }
        }

        // 動けないし、プレイヤーも検知しないようにする
        canMove = false;
        EnemySearchPlayer script = GetComponent<EnemySearchPlayer>();
        Destroy(script);

        // 全コルーチン停止
        StopAllCoroutines();
    }

    #endregion

    #region コルーチン

    /// <summary>
    /// プレイヤーに向かって近づく移動処理。
    /// </summary>
    private IEnumerator Move(Vector3 playerPosition)
    {
        // プレイヤーを検知した場合
        while (playerIsHere)
        {
            // 一定距離まで歩く
            animator.Play(walkAnimation);
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, mushroomMoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, playerPosition) < distanceToPlayer)
                break;

            yield return null;
        }
    }

    /// <summary>
    /// 毒攻撃を含む攻撃処理。
    /// </summary>
    private IEnumerator Attack()
    {
        // 攻撃アニメーション再生
        animator.Play(attackAnimation);

        // 毒攻撃関数を呼ぶ
        Invoke(nameof(EnemySpecialMove), popPoisonDelay);

        // SE再生
        SFXManager sfx = GameObject.FindWithTag("SFXManager")?.GetComponent<SFXManager>();
        sfx?.SetSwingAttackSound();

        // インターバルを設ける
        yield return new WaitForSeconds(attackInterval);
    }

    /// <summary>
    /// 一定時間後に毒を再度撃てるようにする。
    /// </summary>
    private IEnumerator SpecialAttack()
    {
        // 一定時間待って
        yield return new WaitForSeconds(ResetPoisonTime);

        // 毒を撃てる状態にする
        canShotPoison = true;
    }

    #endregion
}
