using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの進行状況などを保持する ScriptableObject。セーブ/ロードの対象になるデータ構造。
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    #region 変数宣言

    [SerializeField] private float hp;                                         // 現在の体力
    [SerializeField] private List<SO_Spell> purchasedSpells = new();          // 購入済みの魔法
    [SerializeField] private string nowSubSpell;                               // 現在装備中のサブスペル
    [SerializeField] private int keyCount;                                     // 所持している鍵の数
    [SerializeField] private int nowMoney;                                     // 所持金
    [SerializeField] private bool watchTutorial;                               // チュートリアルを見たか
    [SerializeField] private bool watchSupportText;                            // サポートテキストを見たか

    private bool isInitialized = true;                                         // 初期化済みかどうか

    private event OnPlayerInfoChanged PlayerInfoChanged;                       // 情報が変化した際の通知イベント

    #endregion

    #region デリゲート定義

    /// <summary>
    /// プレイヤー情報の変更通知イベント用デリゲート
    /// </summary>
    public delegate void OnPlayerInfoChanged();

    #endregion

    #region プロパティ

    public float Hp
    {
        get => hp;
        set
        {
            if (hp != value)
            {
                hp = value;
                PlayerInfoChanged?.Invoke();
            }
        }
    }

    public List<SO_Spell> PurchasedSpells
    {
        get => purchasedSpells;
        set
        {
            if (purchasedSpells != value)
            {
                purchasedSpells = value;
                Debug.Log("スキル追加");
                PlayerInfoChanged?.Invoke();
            }
        }
    }

    public string NowSubSpell
    {
        get => nowSubSpell;
        set
        {
            if (nowSubSpell != value)
            {
                nowSubSpell = value;
                Debug.Log("サブスペル変更！");
                PlayerInfoChanged?.Invoke();
            }
        }
    }

    public int KeyCount
    {
        get => keyCount;
        set
        {
            if (keyCount != value)
            {
                keyCount = value;
                PlayerInfoChanged?.Invoke();
            }
        }
    }

    public int NowMoney
    {
        get => nowMoney;
        set
        {
            if (nowMoney != value)
            {
                nowMoney = value;
                PlayerInfoChanged?.Invoke();
            }
        }
    }

    public bool IsInitialized
    {
        get => isInitialized;
        set => isInitialized = value;
    }

    public bool WatchTutorial
    {
        get => watchTutorial;
        set => watchTutorial = value;
    }

    public bool WatchSupportText
    {
        get => watchSupportText;
        set => watchSupportText = value;
    }

    #endregion

    #region メソッド

    /// <summary>
    /// イベントリスナーを追加する
    /// </summary>
    public void AddListener(OnPlayerInfoChanged listener)
    {
        PlayerInfoChanged += listener;
    }

    /// <summary>
    /// イベントリスナーを解除する
    /// </summary>
    public void RemoveListener(OnPlayerInfoChanged listener)
    {
        PlayerInfoChanged -= listener;
    }

    /// <summary>
    /// プレイヤー情報の初期化
    /// </summary>
    public void InitializePlayerInfo()
    {
        hp = 100f;
        purchasedSpells.Clear();
        nowSubSpell = "";
        keyCount = 0;
        nowMoney = 0;
    }

    #endregion
}
