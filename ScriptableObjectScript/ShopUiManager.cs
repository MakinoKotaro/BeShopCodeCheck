using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ショップ画面に表示されているアイテムにカーソルが乗ったときに表示する説明のUIを管理するスクリプト
/// </summary>
public class ShopUiManager : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private Image panelImage; // アイテム説明パネルの背景画像
    [SerializeField] private TextMeshProUGUI itemNameText; // パネル内のアイテム名表示UI
    [SerializeField] private TextMeshProUGUI itemDescText; // パネル内のアイテム説明表示UI
    [SerializeField] private Button buyButton;             // 購入ボタン

    [SerializeField] private TextMeshProUGUI playerInfoText_attackPower; // プレイヤー攻撃力表示UI
    [SerializeField] private TextMeshProUGUI playerInfoText_hitPoint;    // プレイヤーHP表示UI
    [SerializeField] private TextMeshProUGUI playerInfoText_speed;       // プレイヤー速度表示UI
    [SerializeField] private GameObject playerParameterObj;              // プレイヤーパラメータ保持オブジェクト

    private PurchaseManager purchaseManager;     // 購入マネージャ参照
    private PlayerParameter playerParameter;     // プレイヤーパラメータ参照
    #endregion

    #region Unityイベント
    private void Start()
    {
        panelImage.GetComponent<Image>(); // 明示的な取得（冗長だが明示）
        itemNameText = itemNameText.GetComponent<TextMeshProUGUI>();
        itemDescText = itemDescText.GetComponent<TextMeshProUGUI>();

        if (playerInfoText_attackPower != null)
        {
            playerInfoText_attackPower = playerInfoText_attackPower.GetComponent<TextMeshProUGUI>();
        }

        purchaseManager = FindObjectOfType<PurchaseManager>();
        playerParameter = playerParameterObj.GetComponent<PlayerParameter>();

        HideDescUi(); // 初期状態では非表示
    }

    private void Update()
    {
        if (playerInfoText_attackPower != null)
        {
            playerInfoText_attackPower.text = "Damage: \n" + playerParameter.PlayerPower.ToString();
            playerInfoText_hitPoint.text = "HP: \n" + playerParameter.PlayerHitPoint.ToString();
            playerInfoText_speed.text = "Speed: \n" + playerParameter.PlayerSpeed.ToString();
        }
    }
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// 通常アイテムの説明UIを表示
    /// </summary>
    public void ShowDescUi(SO_ShopItem shopItem)
    {
        itemNameText.text = shopItem.ItemName;
        itemDescText.text = shopItem.ItemDesc;

        SetPanelAlpha(1f);
    }

    /// <summary>
    /// 魔法アイテムの説明UIを表示
    /// </summary>
    public void ShowSpellDescUi(SO_Spell shopSpell)
    {
        itemNameText.text = shopSpell.SpellName;
        itemDescText.text = shopSpell.SpellDesc;

        SetPanelAlpha(1f);
    }

    /// <summary>
    /// 説明UIを非表示にする
    /// </summary>
    public void HideDescUi()
    {
        SetPanelAlpha(0f);
    }

    /// <summary>
    /// UIの透明度を設定する共通処理
    /// </summary>
    /// <param name="alpha">透明度</param>
    private void SetPanelAlpha(float alpha)
    {
        Color panelColor = panelImage.color;
        panelColor.a = alpha;
        panelImage.color = panelColor;

        Color itemNameColor = itemNameText.color;
        itemNameColor.a = alpha;
        itemNameText.color = itemNameColor;

        Color itemDescColor = itemDescText.color;
        itemDescColor.a = alpha;
        itemDescText.color = itemDescColor;
    }
    #endregion
}


