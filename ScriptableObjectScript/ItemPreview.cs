using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ショップ内にアイテムの情報を表示する処理
/// </summary>
public class ItemPreview : MonoBehaviour
{
    #region 定数
    private const int MaxItemCount = 8; // 配列の最大数（UI枠数）
    #endregion

    #region 変数宣言
    [SerializeField] private List<SO_ShopItem> items = new List<SO_ShopItem>(); // ショップのアイテム一覧

    [SerializeField] private Image[] images = new Image[MaxItemCount];                // アイテム画像UI
    [SerializeField] private TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[MaxItemCount]; // アイテム名表示UI
    [SerializeField] private TextMeshProUGUI[] priceTexts = new TextMeshProUGUI[MaxItemCount]; // アイテム価格表示UI
    [SerializeField] private GameObject[] selectedFrames = new GameObject[MaxItemCount];       // 選択中フレームUI
    #endregion

    #region Unityイベント
    private void Start()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].sprite = items[i].ItemSprite;
            nameTexts[i].text = items[i].ItemName;
            priceTexts[i].text = items[i].ItemPrice.ToString() + "yen";
        }
    }
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// 選択されたことをわかりやすくするフレームを表示
    /// </summary>
    /// <param name="gameObject">表示する選択フレーム</param>
    public void ShowSelectedFrame(GameObject gameObject)
    {
        foreach (GameObject g in selectedFrames)
        {
            g.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 選択されたフレームを非表示にする
    /// </summary>
    /// <param name="gameObject">非表示にする選択フレーム</param>
    public void HideSelectedFrame(GameObject gameObject)
    {
        foreach (GameObject g in selectedFrames)
        {
            g.SetActive(false);
        }
    }
    #endregion
}
