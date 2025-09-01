using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アイテム購入の処理
/// </summary>
public class PurchaseManager : MonoBehaviour
{
    #region 変数宣言
    [SerializeField] private List<SO_ShopItem> items;                 // アイテムのScriptableObjectのリスト
    [SerializeField] private Image selectedItemImage;                 // 選択されたアイテムの画像表示先
    [SerializeField] private TextMeshProUGUI countText;               // 購入数の表示テキスト
    [SerializeField] private GameObject sFXManagerObj;                // 効果音管理オブジェクト

    private SO_ShopItem selectedItem;                                 // 選択中の通常アイテム
    private SO_Spell selectedSpell;                                   // 選択中の魔法アイテム
    private int itemCount = 0;                                        // 購入数
    private PlayerParameter playerParameter;                          // プレイヤーパラメータ管理
    private GameObject spellInventoryObj;                             // 魔法インベントリオブジェクト
    private ShortageMoney shortageMoney;                              // お金不足時のUI表示クラス
    private List<string> boughtSpellName = new List<string>();        // 購入済み魔法名リスト
    private JSONManager jsonManager;                                  // データ保存マネージャ
    #endregion

    #region プロパティ
    public int ItemCount { get => itemCount; set => itemCount = value; }
    #endregion

    #region Unityイベント
    private void Start()
    {
        countText = countText.GetComponent<TextMeshProUGUI>();
        selectedItemImage = selectedItemImage.GetComponent<Image>();
        playerParameter = GetComponent<PlayerParameter>();

        //spellInventoryObj = GameObject.FindWithTag("SpellInventory");
        //jsonManager = spellInventoryObj.GetComponent<JSONManager>();

        jsonManager = FindAnyObjectByType<JSONManager>();
    }

    private void Update()
    {
        // 選択アイテムに応じて画像の透明度を切り替え
        Color color = selectedItemImage.color;
        color.a = (selectedSpell == null && selectedItem == null) ? 0 : 1;
        selectedItemImage.color = color;

        // 購入個数を表示する
        countText.text = itemCount.ToString();
    }
    #endregion

    #region カスタムメソッド
    public void SelectedItem(SO_ShopItem item)
    {
        selectedItem = item;
        selectedItemImage.sprite = item.ItemSprite;
        sFXManagerObj.GetComponent<SFXManager>().SetSelectBuyItemSound();
        CountReset();
    }

    public void SelectedSpell(SO_Spell spell)
    {
        selectedSpell = spell;
        selectedItemImage.sprite = spell.SpellSprite;
        sFXManagerObj.GetComponent<SFXManager>().SetSelectBuyItemSound();
        CountReset();
    }

    public void BuyItems()
    {
        if (selectedItem != null && itemCount != 0)
        {
            if (selectedItem.ItemPrice <= jsonManager.PlayerInfo.NowMoney)
            {
                sFXManagerObj.GetComponent<SFXManager>().SetPurchaseSound();

                if (selectedItem.ItemDesc.Contains("+"))
                {
                    playerParameter.AddParameter(selectedItem.CalcType, selectedItem.ItemEffectValue, itemCount);
                }
                else if (selectedItem.ItemDesc.Contains("*"))
                {
                    playerParameter.MulParameter(selectedItem.CalcType, selectedItem.ItemEffectValue, itemCount);
                }
                else
                {
                    playerParameter.UnlockSpecialMove(selectedItem.name, itemCount);
                }

                jsonManager.PlayerInfo.NowMoney -= selectedItem.ItemPrice;
                selectedItem = null;
            }
            else
            {
                ShowShortageMessage();
            }
        }

        CountReset();
    }

    public void BuySpells()
    {
        if (selectedSpell != null && itemCount != 0 &&
            selectedSpell.SpellPrice <= jsonManager.PlayerInfo.NowMoney &&
            !boughtSpellName.Contains(selectedSpell.name))
        {
            jsonManager.PlayerInfo.PurchasedSpells.Add(selectedSpell);
            jsonManager.PlayerInfo.NowMoney -= selectedSpell.SpellPrice;
            sFXManagerObj.GetComponent<SFXManager>().SetPurchaseSound();
            boughtSpellName.Add(selectedSpell.name);
        }
        else
        {
            ShowShortageMessage();
        }

        CountReset();
    }

    public void PlusItemCount()
    {
        itemCount++;
        sFXManagerObj.GetComponent<SFXManager>().SetChangeItemAmountSound();
    }

    public void MinusItemCount()
    {
        if (itemCount > 0)
        {
            itemCount--;
            sFXManagerObj.GetComponent<SFXManager>().SetChangeItemAmountSound();
        }
    }

    public void CountReset()
    {
        itemCount = 0;
    }

    private void ShowShortageMessage()
    {
        Debug.Log("おかねがない！");
        //GameObject canvas = GameObject.FindWithTag("Canvas");
        //shortageMoney = canvas.GetComponent<ShortageMoney>();

        shortageMoney = FindAnyObjectByType<ShortageMoney>();
        shortageMoney.PopShortageMoneyText();
    }
    #endregion
}
