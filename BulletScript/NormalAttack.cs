using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 通常攻撃のスクリプト
/// </summary>
public class NormalAttack : MagicBase
{
    #region 変数宣言

    [SerializeField] private SO_Spell spell;                     // 使用する魔法情報
    [SerializeField] private float moveSpeed = 5f;               // 魔法の速度
    [SerializeField] private float spellLength = 10.0f;          // 魔法の飛距離
    [SerializeField] private float moveLength = 0.5f;            // 移動にかかる時間
    [SerializeField] private float destroyDelay = 0.7f;          // 魔法の削除タイミング

    [SerializeField] private GameObject player;                  // プレイヤーオブジェクト
    [SerializeField] private GameObject particleManagerObj;      // パーティクル管理オブジェクト

    private PlayerController playerController;                   // プレイヤーのコントローラー
    private GameObject sFXManagerObj;                            // SE管理オブジェクト

    private List<GameObject> activeSpells = new();               // 発動中の魔法リスト
    private GameObject currentSpell;                             // 現在発射中の魔法

    private GameObject castPoint;                                // 発射位置オブジェクト
    private Vector3 screenCenter;                                // 画面中央位置
    private Vector3 bulletDirection;                             // 弾の飛ぶ方向

    #endregion

    #region コンストラクタ

    /// <summary>
    /// 初期値設定
    /// </summary>
    public NormalAttack()
    {
        ManaCost = 5;
        MagicDamage = 5;
    }

    #endregion

    #region Unityイベント

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>(); // PlayerController取得
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0).normalized; // 中心を取得
        sFXManagerObj = GameObject.FindWithTag("SFXManager");       // SFXマネージャ取得
    }

    #endregion

    #region 継承メソッド

    /// <summary>
    /// 魔法の移動処理
    /// </summary>
    /// <param name="targetPoint">ターゲット位置</param>
    public override void Behaviour(Vector3 targetPoint)
    {
        // 発射する場所を取得
        castPoint = GameObject.FindWithTag("CastPoint");


        // カメラ位置から、魔法を発射する目標位置を設定
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 targetPosition = cameraPosition + cameraForward * spellLength;


        // 弾を発射するベクトルを計算
        bulletDirection = targetPosition - castPoint.transform.position;

        // DoTweenで弾を飛ばす
        currentSpell.transform
            .DOLocalMove(bulletDirection, moveLength)
            .SetRelative(true)
            .SetEase(Ease.Linear);


        // 弾を壊す処理を遅らせて呼ぶ
        Invoke(nameof(DestroySpell), destroyDelay);
    }

    /// <summary>
    /// 魔法のキャスト処理
    /// </summary>
    /// <param name="castPoint">キャスト位置</param>
    public override void Cast(Transform castPoint)
    {
        // 現在の魔法となるプレハブを生成
        currentSpell = Instantiate(spell.SpellPrefab, castPoint.position, Quaternion.identity);

        // SE再生
        SFXManager sFXManager = sFXManagerObj.GetComponent<SFXManager>();
        sFXManager.SetShotSound();
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 魔法を破棄する
    /// </summary>
    private void DestroySpell()
    {
        Destroy(currentSpell);
    }

    #endregion

    #region 衝突イベント

    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトが敵だったら
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");

            // 敵に含まれているIE_TakeDamageを取得し、攻撃を受ける関数を呼ぶ
            IE_TakeDamage e_TakeDamage = collision.gameObject.GetComponent<IE_TakeDamage>();
            e_TakeDamage.EnemyTakeDamage(MagicDamage);

            // 魔法を削除する
            DestroySpell();
        }
    }

    #endregion
}
