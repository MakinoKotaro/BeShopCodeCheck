using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落下する岩がプレイヤーや地面と接触したときの処理を行うクラス
/// </summary>
public class RockController : MonoBehaviour
{
    #region 定数
    private const float DamageAmount = 3f; // プレイヤーに与えるダメージ量
    #endregion

    #region 変数
    private PlayerParameter playerParameter; // プレイヤーのパラメータ取得用
    #endregion

    #region Unityイベント

    /// <summary>
    /// 岩が接触した際の処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに衝突した場合の処理
        if (other.CompareTag("Player"))
        {
            // プレイヤーのパラメータを取得
            playerParameter = other.GetComponent<PlayerParameter>();

            if (playerParameter != null)
            {
                // プレイヤーにダメージを与える
                playerParameter.PlayerTakeDamage(DamageAmount);
            }

            Destroy(gameObject);
        }

        // 地面に衝突した場合の処理
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
