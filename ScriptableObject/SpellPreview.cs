using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 魔法ショップのアイテムを表示するスクリプト
/// </summary>
public class SpellPreview : MonoBehaviour
{
    #region 定数
    private const int ItemSlotCount = 8; // 表示スロット数
    #endregion

    #region 変数宣言
    [SerializeField] private List<SO_Spell> spell = new List<SO_Spell>(); // ショップの魔法リスト

    [SerializeField] private Image[] images = new Image[ItemSlotCount]; // アイテム画像表示用
    [SerializeField] private TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[ItemSlotCount]; // 名前表示
    [SerializeField] private TextMeshProUGUI[] priceTexts = new TextMeshProUGUI[ItemSlotCount]; // 価格表示
    [SerializeField] private GameObject[] selectedFrames = new GameObject[ItemSlotCount]; // 選択枠表示
    #endregion

    #region Unityイベント
    private void Start()
    {
        if (images != null)
        {
            // 各スロットに対応する魔法情報を設定
            for (int i = 0; i < images.Length; i++)
            {
                images[i].sprite = spell[i].SpellSprite;
                nameTexts[i].text = spell[i].SpellName;
                priceTexts[i].text = spell[i].SpellPrice.ToString() + "yen";
            }
        }
    }
    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 指定のゲームオブジェクトに選択枠を表示する
    /// </summary>
    /// <param name="gameObject">選択されたアイテムの枠</param>
    public void ShowSelectedFrame(GameObject gameObject)
    {
        foreach (GameObject g in selectedFrames)
        {
            g.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// すべての選択枠を非表示にする
    /// </summary>
    /// <param name="gameObject">現在アクティブな選択枠</param>
    public void HideSelectedFrame(GameObject gameObject)
    {
        foreach (GameObject g in selectedFrames)
        {
            g.SetActive(false);
        }
    }

    #endregion
}
