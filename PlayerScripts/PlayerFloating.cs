using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFloating : MonoBehaviour
{
    #region 変数宣言

    [Header("浮遊設定")]
    [SerializeField] private float floatHeight = 5.0f;               // 浮遊する高さ
    [SerializeField] private float floatDuration = 2.0f;             // 浮遊にかかる時間
    [SerializeField] private Ease floatEase = Ease.InOutSine;        // 浮遊の動き（イージング）

    private bool isFloating = false;                                 // 現在浮遊中かどうか
    private Vector3 originalPosition;                                // 元の座標位置

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 一度だけ上方向に浮かぶアニメーションを再生
    /// </summary>
    public void Float()
    {
        // すでに浮遊中なら以下の処理を呼ばない
        if (isFloating) return;

        // 浮遊中フラグをオンにする
        isFloating = true;

        // 現在の位置を保存
        originalPosition = transform.position;

        // Y軸方向にfloatHeight分だけfloatDuration秒かけて移動し、完了時にisFloatingをfalseに戻す
        transform.DOMoveY(originalPosition.y + floatHeight, floatDuration)
            .SetEase(floatEase)
            .OnComplete(() =>
            {
                // 浮遊アニメーションが完了したらフラグを戻す
                isFloating = false;
            });
    }

    #endregion
}
