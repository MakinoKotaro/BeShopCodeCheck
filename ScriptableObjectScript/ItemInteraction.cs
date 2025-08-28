using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static UnityEditor.Progress;

/// <summary>
/// カーソルがどのアイテムのUIに乗ったかを判定し、説明のUIを表示するスクリプト
/// </summary>
public class ItemInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    #region 変数宣言
    [SerializeField] private ShopUiManager shopUiManager;       // UI表示を制御するマネージャ
    [SerializeField] private PurchaseManager purchaseManager;   // 購入処理を行うマネージャ
    [SerializeField] private SO_ShopItem shopItem;              // 対象のアイテムデータ
    [SerializeField] private ItemPreview itemPreview;           // 選択中フレームの表示処理
    [SerializeField] private GameObject itemSelectedImage;      // 選択中フレーム画像

    private bool isShowing = false;                             // 説明UIが表示中かどうか
    #endregion

    #region Unityイベント
    private void Start()
    {
        itemSelectedImage.SetActive(false); // 起動時に選択フレームを非表示
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isShowing)
        {
            shopUiManager.ShowDescUi(shopItem);
            isShowing = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopUiManager.HideDescUi();
        isShowing = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        purchaseManager.SelectedItem(shopItem);
        itemPreview.ShowSelectedFrame(itemSelectedImage);
    }
    #endregion

    #region カスタムメソッド
    public void HideFrame(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
    #endregion
}

