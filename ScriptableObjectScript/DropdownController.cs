using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ショップUIのドロップダウン選択に応じて、
/// 表示するアイテム名・説明・画像を切り替え、購入状態を保持するコントローラ。
/// </summary>
public class DropdownController : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private SO_ShopItem knife;              // 初期表示用のアイテム（ナイフ）
    [SerializeField] private TextMeshProUGUI itemName;       // アイテム名表示用テキスト
    [SerializeField] private TextMeshProUGUI itemDesc;       // アイテム説明表示用テキスト
    [SerializeField] private Image itemImage;                // アイテム画像表示用Image

    private TMP_Dropdown dropdown;                           // ドロップダウンコンポーネント参照
    private bool isBought = false;                           // 購入済みフラグ（true=購入済み）
    #endregion

    #region プロパティ
    public bool IsBought { get => isBought; set => isBought = value; } // 外部から購入状態を参照・変更可能にするプロパティ
    #endregion

    #region Unityイベント
    private void Start()
    {
        // 同一GameObject上の TMP_Dropdown コンポーネントを取得して保持
        dropdown = GetComponent<TMP_Dropdown>();
    }
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// ドロップダウンの選択肢が変更されたときに呼ぶ想定の処理。
    /// 選択値（dropdown.value）に応じて、テキストと画像を切り替える。
    /// </summary>
    public void OnSelected()
    {
        switch (dropdown.value)
        {
            case 0:
                // 選択肢0：knife（初期アイテム）の情報を各UIに反映
                itemName.text = knife.GetItemName();
                itemDesc.text = knife.GetItemDesc();
                itemImage.sprite = knife.GetItemSprite();

                if (isBought == true)
                {
                    // 購入済み時の追加処理がある場合はここに記述（現状は何もしない）
                }
                break;

            default:
                // 未対応の選択肢：プレースホルダー表示（名称・説明は固定文言、画像はクリア）
                itemName.text = "アイテム名";
                itemDesc.text = "アイテム説明";
                itemImage.sprite = null;
                break;
        }
    }

    /// <summary>
    /// 購入実行時に呼ぶ想定の処理。
    /// 現状は「購入済みフラグを立てるだけ」で、決済や在庫反映などの処理は別箇所で行う前提。
    /// </summary>
    public void BuyItem()
    {
        isBought = true; // 購入済みに設定
    }
    #endregion
}
