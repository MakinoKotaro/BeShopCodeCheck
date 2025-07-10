using System.Collections;
using UnityEngine;

/// <summary>
/// 敵：クマのスクリプト
/// </summary>
public class Bear : Enemy, IE_GetFroze
{
    #region 定数
    private const float DefaultDestroyDelay = 0.7f;
    private const float DefaultDistanceToPlayer = 3.0f;
    private const int DefaultDropMoney = 100;
    private static readonly Vector3 IceSpawnOffset = new Vector3(0, 0.9f, 0);
    private const float AttackWaitBeforeHit = 0.6f;
    private const float AttackInterval = 4.4f;
    #endregion

    #region シリアライズ変数
    [Header("参照オブジェクト")]
    [SerializeField] private GameObject enemyManagerObj;
    [SerializeField] private GameObject popGetMoneyObj;
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private GameObject sFXManagerObj;

    [Header("基本パラメータ")]
    [SerializeField] private float bearHealth = 100f;
    [SerializeField] private int bearAttackPower = 10;
    [SerializeField] private float bearMoveSpeed = 5f;

    [Header("アニメーション名")]
    [SerializeField] private string idle_animation;
    [SerializeField] private string attack_animation;
    [SerializeField] private string walk_animation;
    [SerializeField] private string dead_animation;
    [SerializeField] private string hurt_animation;
    #endregion

    #region プライベート変数
    private EnemyManager _enemyManager;
    private GameObject player;
    private GameObject playerInventoryObj;
    private SFXManager sFXManager;
    private JSONManager jsonManager;
    private Animator animator;
    private bool playerIsHere = false;
    private float destroyDelay = DefaultDestroyDelay;
    private float distanceToPlayer = DefaultDistanceToPlayer;
    private int dropMoneyAmount = DefaultDropMoney;
    #endregion

    #region Unityイベント
    private void Start()
    {
        // アニメーター取得
        animator = GetComponent<Animator>();

        // 初期パラメータ設定
        health = bearHealth;
        attackPower = bearAttackPower;
        canMove = true;

        // プレイヤー取得
        player = GameObject.FindWithTag("Player");
        playerInventoryObj = GameObject.FindWithTag("PlayerInventory");

        // 各スクリプトが付いたオブジェクトを取得
        _enemyManager = enemyManagerObj.GetComponent<EnemyManager>();
        sFXManager = sFXManagerObj.GetComponent<SFXManager>();
        jsonManager = player.GetComponent<JSONManager>();
    }
    #endregion

    #region オーバーライドメソッド

    /// <summary>
    /// 移動開始を判定する処理
    /// </summary>
    /// <param name="foundPlayer"></param>
    /// <param name="playerPosition"></param>
    public override void EnemyMove(bool foundPlayer, Vector3 playerPosition)
    {
        // 動けない状態なら以下の処理を呼ばない
        if (!canMove) return;

        // 見つけたかどうか
        playerIsHere = foundPlayer;

        // 見つけている場合
        if (foundPlayer)
        {
            // 移動コルーチン開始
            StartCoroutine(Move(playerPosition));
        }
        // 見つけていない場合
        else
        {
            // 停止状態のアニメーション再生
            animator.Play(idle_animation);
        }
    }

    /// <summary>
    /// 攻撃処理の開始を判定
    /// </summary>
    public override void EnemyAttack()
    {
        StartCoroutine(Attack());
    }

    /// <summary>
    /// 攻撃処理を止める判定
    /// </summary>
    public override void EnemyStopAttack()
    {
        StopCoroutine(Attack());
    }


    /// <summary>
    /// 敵が攻撃を受けるときの処理
    /// </summary>
    /// <param name="damageAmount"></param>
    public override void EnemyTakeDamage(float damageAmount)
    {
        // ダメージを受けるアニメーション再生
        animator.Play(hurt_animation);

        // 体力を減らす
        health -= damageAmount;

        // SE再生
        sFXManager.SetBearTakeDamageSound();


        // 体力が０以下になった場合
        if (health <= 0)
        {
            // 死ぬ処理を呼ぶ
            EnemyDead();
        }
    }

    /// <summary>
    /// 死んだときの処理
    /// </summary>
    public override void EnemyDead()
    {
        // 死ぬアニメーション再生
        animator.Play(dead_animation);

        // 動けないようにする
        canMove = false;

        // お金を獲得したことをUIで表示する
        popGetMoneyObj.GetComponent<PopGetMoneyUi>()?.ShowUi(dropMoneyAmount);

        // JSONデータを更新
        jsonManager.PlayerInfo.NowMoney += dropMoneyAmount;

        // 死亡時のコルーチンを開始
        StartCoroutine(HandleDeath());
    }
    #endregion

    #region コルーチン

    /// <summary>
    /// 移動コルーチン
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    private IEnumerator Move(Vector3 playerPosition)
    {
        // プレイヤーを検知した場合
        while (playerIsHere)
        {
            // 歩きアニメーションを再生
            animator.Play(walk_animation);

            // 移動処理
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, bearMoveSpeed * Time.deltaTime);

            // プレイヤーとの距離が一定に達すると停止する
            if (Vector3.Distance(transform.position, playerPosition) < distanceToPlayer)
                break;

            yield return null;
        }
    }

    /// <summary>
    /// 攻撃コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        // 攻撃アニメーション再生
        animator.Play(attack_animation);

        // SE再生
        sFXManager.SetSwingAttackSound();

        // プレイヤー検知用のスクリプトを取得
        EnemySearchPlayer enemySearchPlayer = GetComponent<EnemySearchPlayer>();

        // 一定時間待ってから
        yield return new WaitForSeconds(AttackWaitBeforeHit);

        // プレイヤーが正面にいた場合
        if (enemySearchPlayer.PlayerInFront)
        {
            // プレイヤーにダメージを与える
            DoneDamageToPlayer(bearAttackPower);
        }

        // 攻撃インターバルの間待機
        yield return new WaitForSeconds(AttackInterval);
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
    #endregion

    #region 凍結インターフェース
    public void EnemyGetFroze()
    {
        if (icePrefab == null)
        {
            Debug.LogError("Iceプレハブがアタッチされていません！");
            return;
        }

        Vector3 spawnPosition = transform.position + IceSpawnOffset;
        GameObject newIce = Instantiate(icePrefab, spawnPosition, Quaternion.identity);
        transform.SetParent(newIce.transform);

        foreach (var component in GetComponents<Component>())
        {
            if (component is Transform) continue;

            if (component is MonoBehaviour mono) mono.enabled = false;
            else if (component is Behaviour behav) behav.enabled = false;
        }

        canMove = false;
        Destroy(GetComponent<EnemySearchPlayer>());
        StopAllCoroutines();
    }
    #endregion
}
