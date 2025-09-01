using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回復魔法の挙動を制御するスクリプト
/// </summary>
public class HealMagic : MagicBase
{
    #region 変数宣言

    [SerializeField] private SO_Spell spell;               // 使用する魔法情報
    [SerializeField] private float moveSpeed = 5f;         // 魔法の速度（未使用）

    private Vector3 targetPointPosition;                   // 着弾位置
    private List<GameObject> activeSpells = new List<GameObject>(); // 発動中の魔法を格納
    private GameObject currentSpell;                       // 現在発射中の魔法

    [SerializeField] private GameObject player;            // プレイヤー本体
    private PlayerController playerController;             // プレイヤーコントローラ取得用

    private GameObject castPoint;                          // 発射位置
    private Vector3 screenCenter;                          // 画面中央（未使用）
    private Vector3 bulletDirection;                       // 弾の方向（未使用）

    [SerializeField] private GameObject particleManagerObj; // パーティクル管理オブジェクト
    private GameObject sfxManagerObj;                      // SFXマネージャ取得用

    #endregion

    #region コンストラクタ

    /// <summary>
    /// HealMagicの初期化
    /// </summary>
    public void Initialize()
    {
        ManaCost = 0;
        MagicDamage = 2.0f;
    }

    #endregion

    #region Unityイベント

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>(); // プレイヤーコントローラ取得
        sfxManagerObj = GameObject.FindWithTag("SFXManager");       // SFXマネージャ取得
        //sfxManagerObjにSFXManager.csがついているかな？何か.csファイルがついているなら
        //FindObjectOfType<○○.cs>();で取得してください
    }

    #endregion

    #region 継承メソッド

    /// <summary>
    /// ヒール魔法の振る舞い（未実装）
    /// </summary>
    public override void Behaviour(Vector3 targetPoint)
    {
        // 未使用
    }

    /// <summary>
    /// ヒール魔法のキャスト処理
    /// </summary>
    public override void Cast(Transform castPoint)
    {
        SpendMana("回復魔法", ManaCost);

        Debug.Log(castPoint.position);

        // 魔法のプレハブを生成
        currentSpell = Instantiate(spell.SpellPrefab, castPoint.position, Quaternion.identity);

        // プレイヤーのHPを回復
        PlayerParameter playerParameter = player.GetComponent<PlayerParameter>();
        playerParameter.PlayerHeal(MagicDamage);

        // ヒール効果音を再生
        SFXManager sFXManager = sfxManagerObj.GetComponent<SFXManager>();
        sFXManager.SetHealSound();
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 魔法のオブジェクトを破壊する
    /// </summary>
    public void DestroySpell()
    {
        Destroy(currentSpell);
    }

    #endregion
}
