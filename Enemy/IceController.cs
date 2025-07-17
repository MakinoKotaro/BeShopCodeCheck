using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iceオブジェクトの物理挙動を制御するクラス
/// </summary>
public class IceController : MonoBehaviour
{
    #region 変数宣言

    private Rigidbody rb; // アタッチされたRigidbody参照用

    // 凍結に使う制約定数
    private const RigidbodyConstraints FreezeConstraints =
        RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationY;

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // Rigidbody取得
        rb = GetComponent<Rigidbody>();

        // Rigidbodyがなければエラーを出す
        if (rb == null)
        {
            Debug.LogError("Rigidbodyがこのオブジェクトにアタッチされていません！");
            return;
        }

        // Y軸の移動を固定し、X軸・Y軸の回転を固定する（Z軸回転のみ許可）
        rb.constraints = FreezeConstraints;
    }

    #endregion
}
