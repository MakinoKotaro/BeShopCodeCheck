using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の抽象基底クラス
/// </summary>
public abstract class Enemy : MonoBehaviour, IE_Attack, IE_Move, IE_TakeDamage, IE_Dead, IE_StopAttack
{
    #region 保護変数

    /// <summary>体力</summary>
    protected float health;

    /// <summary>攻撃力</summary>
    protected float attackPower;

    /// <summary>移動可能か</summary>
    protected bool canMove;

    /// <summary>凍結演出の位置調整用</summary>
    protected Vector3 iceOffset;

    #endregion

    #region プライベート変数

    /// <summary>プレイヤー参照</summary>
    private GameObject player;

    #endregion

    #region 抽象メソッド

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    /// <param name="foundPlayer">プレイヤーを発見しているか</param>
    /// <param name="playerPosition">プレイヤーの位置</param>
    public abstract void EnemyMove(bool foundPlayer, Vector3 playerPosition);

    /// <summary>
    /// 敵の攻撃処理
    /// </summary>
    public abstract void EnemyAttack();

    /// <summary>
    /// 敵がダメージを受けたときの処理
    /// </summary>
    /// <param name="damageAmount">ダメージ量</param>
    public abstract void EnemyTakeDamage(float damageAmount);

    /// <summary>
    /// 敵の死亡処理
    /// </summary>
    public abstract void EnemyDead();

    /// <summary>
    /// 攻撃の停止処理
    /// </summary>
    public abstract void EnemyStopAttack();

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理（現在は未使用）
    /// </summary>
    private void Start()
    {
        // 必要に応じて派生クラスでオーバーライド
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// プレイヤーにダメージを与える処理
    /// </summary>
    /// <param name="damageAmount">与えるダメージ量</param>
    protected void DoneDamageToPlayer(float damageAmount)
    {
        // プレイヤーを探す
        player = GameObject.FindWithTag("Player");

        // プレイヤーがnullなら以下を呼ばない
        if (player == null) return;
        
        // プレイヤーのスクリプトコンポーネントを取得
        PlayerParameter playerParameter = player.GetComponent<PlayerParameter>();
        PlayerController playerController = player.GetComponent<PlayerController>();

        // コンポーネントが存在していて、プレイヤーが回避していない場合
        if (playerParameter != null && playerController != null && playerController.CanDodge)
        {
            // ダメージを与える
            playerParameter.PlayerTakeDamage(damageAmount);
            Debug.Log($"{damageAmount} のダメージをプレイヤーに与えました。");
        }
    }

    #endregion
}
