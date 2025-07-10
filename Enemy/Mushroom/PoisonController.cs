using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 毒の挙動を管理するクラス
/// </summary>
public class PoisonController : MonoBehaviour
{
    #region 変数宣言

    [SerializeField] private GameObject mushParent; // 毒の親（Mushroom本体）

    private Mushroom mushroom;                     // 毒を吐くキノコクラス
    private BoxCollider poisonCollider;            // 当たり判定用コライダー
    private GameObject player;                     // プレイヤーオブジェクト参照

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        poisonCollider = GetComponent<BoxCollider>();
        poisonCollider.enabled = false;

        mushroom = mushParent.GetComponent<Mushroom>();
        player = GameObject.FindWithTag("Player");
    }

    /// <summary>
    /// 毎フレームの処理（毒状態であればコライダーを有効化）
    /// </summary>
    private void Update()
    {
        poisonCollider.enabled = !mushroom.CanShotPoison;
    }

    /// <summary>
    /// 毒の当たり判定にプレイヤーが滞在しているときの処理
    /// </summary>
    /// <param name="other">接触しているコライダー</param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // CanShotPoison が false のとき、プレイヤーに毒効果を付与
            if (!mushroom.CanShotPoison)
            {
                PlayerParameter playerParameter = player.GetComponent<PlayerParameter>();
                if (playerParameter != null)
                {
                    playerParameter.PlayerTakePoison();
                    playerParameter.TakePoison = true;
                }
            }
            else
            {
                Debug.Log("canShotPoison");
            }
        }
    }

    #endregion
}
