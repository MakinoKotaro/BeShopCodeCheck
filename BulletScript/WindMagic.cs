using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 風属性の魔法。プレイヤーを浮遊させる効果を持つ。
/// </summary>
public class WindMagic : MagicBase
{
    #region 変数宣言

    [SerializeField] private SO_Spell spell;                    // 使用する魔法のScriptableObject
    [SerializeField] private float moveSpeed = 5f;              // 魔法の速度
    [SerializeField] private GameObject particleManagerObj;     // パーティクル制御用オブジェクト

    private List<GameObject> activeSpells = new();              // 現在発動中の魔法リスト
    private GameObject currentSpell;                            // 今発射された魔法のインスタンス

    private GameObject player;                                  // プレイヤー参照用
    private PlayerController playerController;                  // プレイヤーのコントローラー
    private GameObject sFXManagerObj;                           // 効果音再生用オブジェクト
    private GameObject castPoint;                               // 魔法の発射位置

    private Vector3 screenCenter;                               // 画面中央（未使用）
    private Vector3 bulletDirection;                            // 魔法の方向（未使用）

    #endregion

    #region コンストラクタ

    public WindMagic()
    {
        ManaCost = 0;
        MagicDamage = 2.0f;
    }

    #endregion

    #region Unityイベント

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        sFXManagerObj = GameObject.FindWithTag("SFXManager");
    }

    #endregion

    #region 継承メソッド

    /// <summary>
    /// 魔法の挙動（未実装）
    /// </summary>
    public override void Behaviour(Vector3 targetPoint)
    {
        // 今回の風魔法では実装不要
    }

    /// <summary>
    /// 魔法の発射処理
    /// </summary>
    public override void Cast(Transform castPoint)
    {
        // 現在の魔法のプレハブを生成
        currentSpell = Instantiate(spell.SpellPrefab, castPoint.position, Quaternion.identity);


        // SE再生
        SFXManager sFXManager = sFXManagerObj.GetComponent<SFXManager>();
        sFXManager.SetWindMagicSound();
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 魔法オブジェクトを破棄
    /// </summary>
    public void DestroySpell()
    {
        Destroy(currentSpell);
    }

    #endregion

    #region 衝突イベント

    /// <summary>
    /// プレイヤーと接触した際に浮遊処理を実行
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 接触したオブジェクトがプレイヤーだった場合
        if (other.CompareTag("Player"))
        {
            // プレイヤーに含まれるPlayerFloatingを取得し、浮かせる関数を呼ぶ
            PlayerFloating playerFloating = player.GetComponent<PlayerFloating>();
            playerFloating.Float();


            // SE再生
            SFXManager sFXManager = sFXManagerObj.GetComponent<SFXManager>();
            sFXManager.SetFloatSound();
        }
    }

    #endregion
}
