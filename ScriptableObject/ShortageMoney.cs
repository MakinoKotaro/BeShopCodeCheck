using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 所持金不足時のアニメーション付きUI表示を行うクラス
/// </summary>
public class ShortageMoney : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private TextMeshProUGUI shortageMoneyText; // 不足金額のテキスト

    private SFXManager sfxManager;       // 効果音再生用マネージャ
    #endregion

    #region Unityイベント
    private void Start()
    {
        shortageMoneyText.enabled = false;                         // 初期状態では非表示
        shortageMoneyText.transform.localScale = Vector3.one * 2f; // 初期スケール（2倍）
        shortageMoneyText.alpha = 0f;                               // 初期透明

        sfxManager = FindAnyObjectByType<SFXManager>();
    }
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// 所持金不足の演出を表示する
    /// </summary>
    public void PopShortageMoneyText()
    {
        shortageMoneyText.enabled = true;
        shortageMoneyText.alpha = 1f; // 表示（不透明）

        Sequence animationSequence = DOTween.Sequence();

        animationSequence
            .Append(
                shortageMoneyText.transform
                    .DOScale(Vector3.one, 0.5f) // 通常サイズへ
                    .SetEase(Ease.OutBack)
            )
            .Join(
                shortageMoneyText.transform
                    .DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360) // 一回転
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        sfxManager.OnClickBackButton();
                    })
            )
            .AppendInterval(1f) // 表示維持時間
            .Append(
                shortageMoneyText
                    .DOFade(0f, 0.5f)
                    .SetEase(Ease.InCubic)
            )
            .OnComplete(ResetText);
    }

    /// <summary>
    /// テキスト表示を初期状態に戻す
    /// </summary>
    private void ResetText()
    {
        shortageMoneyText.enabled = false;
        shortageMoneyText.transform.localScale = Vector3.one * 2f;
        shortageMoneyText.transform.rotation = Quaternion.identity;
        shortageMoneyText.alpha = 0f;
    }
    #endregion
}
