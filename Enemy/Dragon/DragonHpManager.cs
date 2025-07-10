using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ドラゴンのHP表示を管理するクラス
/// </summary>
public class DragonHpManager : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private Image dragonHpImage; // HP表示用イメージ
    private Color originalColor;                 // 元の色（未使用）
    private float currentHp;                     // 現在のHP（未使用）
    #endregion

    #region メソッド
    /// <summary>
    /// 現在のHPをUIに反映する
    /// </summary>
    /// <param name="hp">現在のHP割合（0.0~1.0）</param>
    public void ShowCurrentHp(float hp)
    {
        if (dragonHpImage == null)
        {
            Debug.LogError("dragonHpImage が設定されていません");
            return;
        }

        // HPの割合をImageに反映
        dragonHpImage.fillAmount = Mathf.Clamp01(hp);
    }
    #endregion
}
