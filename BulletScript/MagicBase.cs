using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法の基本クラス（継承用）
/// </summary>
public abstract class MagicBase : MonoBehaviour, IMagic
{
    #region 変数宣言

    [SerializeField] private string magicName;     // 魔法の名前
    [SerializeField] private float manaCost;       // 消費マナ量
    [SerializeField] private float magicDamage;    // 魔法のダメージ量

    #endregion

    #region プロパティ

    public string MagicName
    {
        get => magicName;
        set => magicName = value;
    }

    internal float ManaCost
    {
        get => manaCost;
        set => manaCost = value;
    }

    internal float MagicDamage
    {
        get => magicDamage;
        set => magicDamage = value;
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// マナを消費する処理
    /// </summary>
    /// <param name="magicName">使用する魔法名</param>
    /// <param name="manaCost">消費マナ量</param>
    protected void SpendMana(string magicName, float manaCost)
    {
        // Debug.Log($"マナコスト {manaCost} で魔法 {magicName} を使った。");
    }

    #endregion

    #region 継承メソッド

    /// <summary>
    /// 魔法を発射する抽象メソッド（継承用）
    /// </summary>
    /// <param name="castPoint">発射位置</param>
    public abstract void Cast(Transform castPoint);

    /// <summary>
    /// 魔法の移動・効果処理（継承用）
    /// </summary>
    /// <param name="targetPoint">ターゲット座標</param>
    public abstract void Behaviour(Vector3 targetPoint);

    #endregion
}
