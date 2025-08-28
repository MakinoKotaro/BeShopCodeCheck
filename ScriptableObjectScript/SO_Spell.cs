using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法のScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/Spell")]
public class SO_Spell : ScriptableObject
{
    #region 変数宣言
    [SerializeField] private string spellName;      // 魔法の名前
    [SerializeField] private string spellDesc;      // 魔法の説明
    [SerializeField] private Sprite spellSprite;    // 魔法の画像
    [SerializeField] private int spellPrice;        // 魔法の値段
    [SerializeField] private GameObject spellPrefab; // 魔法のプレハブ
    #endregion

    #region プロパティ
    public string SpellName => spellName;
    public string SpellDesc => spellDesc;
    public Sprite SpellSprite => spellSprite;
    public int SpellPrice => spellPrice;
    public GameObject SpellPrefab => spellPrefab;
    #endregion

    #region カスタムメソッド
    /// <summary>
    /// 魔法名を取得する処理
    /// </summary>
    /// <returns></returns>
    public string GetItemName()
    {
        return this.spellName;
    }

    /// <summary>
    /// 魔法の説明を取得する処理
    /// </summary>
    /// <returns></returns>
    public string GetItemDesc()
    {
        return this.spellDesc;
    }

    /// <summary>
    /// 魔法の価格を取得する処理
    /// </summary>
    /// <returns></returns>
    public int GetPrice()
    {
        return this.spellPrice;
    }

    /// <summary>
    /// 魔法の画像を取得する処理
    /// </summary>
    /// <returns></returns>
    public Sprite GetItemSprite()
    {
        return this.spellSprite;
    }

    /// <summary>
    /// 魔法のプレハブを取得する処理
    /// </summary>
    /// <returns></returns>
    public GameObject GetPrefab()
    {
        return this.spellPrefab;
    }
    #endregion
}
