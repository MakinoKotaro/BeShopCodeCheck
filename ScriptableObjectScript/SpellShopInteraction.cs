using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// カーソルがどの魔法のUIに乗ったかを判定し、説明のUIを表示するスクリプト
/// </summary>
public class SpellShopInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region 変数宣言
    [SerializeField] private ShopUiManager shopUiManager;         // Shop UI 管理クラス
    [SerializeField] private PurchaseManager purchaseManager;     // 購入処理管理クラス
    [SerializeField] private SO_Spell shopSpell;                  // 対象の魔法情報
    [SerializeField] private ItemPreview itemPreview;             // プレビュー制御クラス
    [SerializeField] private GameObject itemSelectedImage;        // 選択中を示す画像オブジェクト
    private bool isShowing = false;                               // 説明表示中フラグ
    #endregion

    #region Unityイベント
    private void Start()
    {
        // 選択中イメージを初期は非表示にする
        itemSelectedImage.SetActive(false);
    }
    #endregion

    #region インターフェース実装

    /// <summary>
    /// マウスポインタがアイテムUIに入ったとき
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isShowing)
        {
            shopUiManager.ShowSpellDescUi(shopSpell);
            isShowing = true;
        }
    }

    /// <summary>
    /// マウスポインタがアイテムUIから出たとき
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        shopUiManager.HideDescUi();
        isShowing = false;
    }

    /// <summary>
    /// UIがクリックされたときの処理
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        purchaseManager.SelectedSpell(shopSpell);
        itemPreview.ShowSelectedFrame(itemSelectedImage);
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 外部から選択枠を非表示にする
    /// </summary>
    public void HideFrame(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    #endregion
}
