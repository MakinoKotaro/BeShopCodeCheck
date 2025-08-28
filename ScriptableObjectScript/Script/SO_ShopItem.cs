using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムの ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/ShopItem")]
public class SO_ShopItem : ScriptableObject
{
    #region 変数宣言
    [SerializeField] private string itemName;           // アイテム名
    [SerializeField] private string itemDesc;           // アイテムの説明
    [SerializeField] private Sprite itemSprite;         // アイテム画像
    [SerializeField] private int itemPrice;             // アイテム価格
    [SerializeField] private float itemEffectValue;     // アイテム効果の数値
    [SerializeField] private string calcType;           // 効果計算タイプ（例: 加算/乗算）
    [SerializeField] private bool doSpecialMove;        // 特殊移動アイテムかどうか
    #endregion

    #region プロパティ
    public string ItemName => itemName;
    public string ItemDesc => itemDesc;
    public Sprite ItemSprite => itemSprite;
    public int ItemPrice => itemPrice;
    public float ItemEffectValue => itemEffectValue;
    public string CalcType => calcType;
    public bool DoSpecialMove => doSpecialMove;
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// アイテム名を取得する
    /// </summary>
    public string GetItemName() => itemName;

    /// <summary>
    /// アイテムの説明を取得する
    /// </summary>
    public string GetItemDesc() => itemDesc;

    /// <summary>
    /// アイテム価格を取得する
    /// </summary>
    public int GetItemPrice() => itemPrice;

    /// <summary>
    /// アイテムのスプライトを取得する
    /// </summary>
    public Sprite GetItemSprite() => itemSprite;

    /// <summary>
    /// アイテム効果値を取得する
    /// </summary>
    public float GetItemEffectValue() => itemEffectValue;

    /// <summary>
    /// 効果の計算方式を取得する
    /// </summary>
    public string GetCalcType() => calcType;

    /// <summary>
    /// 特殊移動フラグを取得する
    /// </summary>
    public bool IsSpecialMoveItem() => doSpecialMove;
    #endregion
}
