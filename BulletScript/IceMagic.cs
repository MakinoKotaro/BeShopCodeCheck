using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 氷の魔法の挙動を制御するクラス
/// </summary>
public class IceMagic : MagicBase
{
    #region 変数宣言

    [SerializeField] private SO_Spell spell;                     // 使用する魔法情報

    [SerializeField] private float moveSpeed = 5f;               // 魔法の移動速度
    [SerializeField] private float spellLength = 10f;            // 魔法の飛距離
    [SerializeField] private float moveLength = 0.5f;            // DOTween移動距離
    [SerializeField] private float destroyDelay = 0.7f;          // 消失までの遅延

    [SerializeField] private GameObject player;                  // プレイヤー本体
    private PlayerController playerController;                   // プレイヤー制御用

    private GameObject castPoint;                                // 発射位置オブジェクト

    private Vector3 screenCenter;                                // 画面中央座標
    private Vector3 bulletDirection;                             // 弾の方向
    private Vector3 targetPointPosition;                         // 魔法の着弾位置

    private List<GameObject> activeSpells = new();               // 発動中の魔法リスト
    private GameObject currentSpell;                             // 現在の魔法インスタンス

    [SerializeField] private GameObject particleManagerObj;      // パーティクルマネージャ
    private GameObject sFXManagerObj;                            // SFXマネージャ

    #endregion

    #region コンストラクタ

    /// <summary>
    /// IceMagicの初期化
    /// </summary>
    public IceMagic()
    {
        ManaCost = 5;
        MagicDamage = 5;
    }

    #endregion

    #region Unityイベント

    private void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0).normalized;
        sFXManagerObj = GameObject.FindWithTag("SFXManager");
    }

    private void Update()
    {
        // 処理なし
    }

    #endregion

    #region 継承メソッド

    /// <summary>
    /// 魔法の移動挙動
    /// </summary>
    public override void Behaviour(Vector3 targetPoint)
    {
        castPoint = GameObject.FindWithTag("CastPoint");

        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 targetPosition = cameraPosition + cameraForward * spellLength;

        bulletDirection = targetPosition - castPoint.transform.position;

        currentSpell.transform.DOLocalMove(bulletDirection, moveLength)
            .SetRelative(true)
            .SetEase(Ease.Linear);

        Invoke(nameof(DestroySpell), destroyDelay);
    }

    /// <summary>
    /// 魔法を発射する処理
    /// </summary>
    public override void Cast(Transform castPoint)
    {
        SpendMana("通常攻撃", ManaCost);

        Debug.Log("Shot Ice magic");

        targetPointPosition = playerController.TargetPoint01.position;

        currentSpell = Instantiate(spell.SpellPrefab, castPoint.position, Quaternion.identity);

        SFXManager sFXManager = sFXManagerObj.GetComponent<SFXManager>();
        sFXManager.SetShotSound();
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 魔法オブジェクトを破棄する
    /// </summary>
    private void DestroySpell()
    {
        Destroy(currentSpell);
    }


    /// <summary>
    /// 敵と衝突すると凍らせる処理を呼ぶ
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("衝突したオブジェクトの名前: " + collision.gameObject.name);


        // 衝突したオブジェクトが敵だった場合
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("ice magic Hit enemy");


            // 敵に含まれているIE_GetFrozeを取得し、凍らせる関数を呼ぶ
            IE_GetFroze e_GetFroze = collision.gameObject.GetComponent<IE_GetFroze>();
            e_GetFroze.EnemyGetFroze();

            ContactPoint contact = collision.contacts[0];
            Vector3 collisionPoint = contact.point;

            DestroySpell();
        }
    }

    #endregion
}
