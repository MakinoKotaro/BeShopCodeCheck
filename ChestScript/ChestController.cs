using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// 宝箱の開閉処理を担当するクラス。キー所持に応じて報酬取得が可能。
/// </summary>
public class ChestController : MonoBehaviour
{
    #region 変数宣言

    [Header("判定・UI関連")]
    [SerializeField] private float radius = 2f;                          // プレイヤーとの接触判定半径
    [SerializeField] private GameObject popGetMoneyUiObj;               // 所持金取得UIプレハブ
    [SerializeField] private TextMeshProUGUI pressFText;                // 「Fキーで開ける」UI

    [Header("メッセージUI")]
    [SerializeField] private TextMeshPro textMeshPro;                   // 表示用TextMeshPro
    [SerializeField] private float moveDuration = 2f;                   // 上昇アニメ時間
    [SerializeField] private float fadeDuration = 2f;                   // フェード時間
    [SerializeField] private float textDelay = 1f;                      // 表示遅延
    [SerializeField] private float moveDistance = 100f;                 // テキストの上昇距離
    [SerializeField] private float hideDelay = 2f;                      // 非表示までの遅延

    [Header("報酬設定")]
    [SerializeField] private int dropMoney = 600;                       // 宝箱から得られるお金

    private GameObject player;                                          
    private JSONManager jsonManager;
    private bool isOpen = false;                                        // 空いているかどうか
    private bool canOpen = false;                                       // 開けられるかどうか

    #endregion

    #region Unityイベント

    private void Start()
    {
        // プレイヤーを取得
        player = GameObject.FindWithTag("Player"); //違うやり方で取得してください。

        // TextMeshProを取得
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshPro>();
        }

        // JsonManagerを取得
        jsonManager = player.GetComponent<JSONManager>();

        // 初期状態で非表示
        textMeshPro.alpha = 0f;
        textMeshPro.gameObject.SetActive(false);
        pressFText.enabled = false;
    }

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        // 鍵を持っていればcanOpenをtrueに設定
        canOpen = jsonManager.PlayerInfo.KeyCount > 0;

        // 接触したプレイヤーが宝箱を開けられる場合
        foreach (Collider collider in hitColliders)
        {
            if (!collider.CompareTag("Player") || isOpen) continue;

            Debug.Log("in Range");

            //以下のインプット処理はせっかくInputSystemを使っているので、InputActionを使うべきです。
            //これじゃ違うプラットフォームで動かないからもったいない
            if (Keyboard.current.fKey.wasPressedThisFrame) // 開ける処理を呼ばれた場合
            {
                // 開けられる場合
                if (canOpen)
                {
                    // 開ける処理を呼ぶ
                    OpenChest(collider);
                }
                // 開けられない場合
                else
                {
                    // 開けられないテキスト表示処理を呼ぶ
                    ShowCannotOpenText();
                }
            }
        }
    }


    /// <summary>
    /// プレイヤーが近づいた場合、ガイドテキストを表示
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) //enum化した方がいい
        {
            pressFText.enabled = true;
        }
    }

    /// <summary>
    /// プレイヤーが離れるとガイドテキスト非表示
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pressFText.enabled = false;
        }
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 宝箱を開ける処理
    /// </summary>
    /// <param name="collider">プレイヤーのCollider</param>
    private void OpenChest(Collider collider)
    {
        // 空いている状態にする
        isOpen = true;
        // ガイドテキスト非表示
        pressFText.enabled = false;

        // お金を取得し、JSONデータを更新
        jsonManager = collider.GetComponent<JSONManager>();
        jsonManager.PlayerInfo.NowMoney += dropMoney;
        jsonManager.PlayerInfo.KeyCount--;

        // PopGetMoneyUiを取得し、取得したことを知らせるUIを表示する関数を呼ぶ
        PopGetMoneyUi popGetMoneyUi = popGetMoneyUiObj.GetComponent<PopGetMoneyUi>();
        popGetMoneyUi.ShowUi(dropMoney);

        // 削除する
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 鍵が無いときのメッセージ表示処理
    /// </summary>
    private void ShowCannotOpenText()
    {
        // 開けられないことを知らせるテキストを表示
        DOVirtual.DelayedCall(textDelay, () =>
        {
            textMeshPro.gameObject.SetActive(true);
            textMeshPro.transform.DOMoveY(textMeshPro.transform.position.y + moveDistance, moveDuration).SetEase(Ease.OutCubic);
            textMeshPro.DOFade(1f, fadeDuration).SetEase(Ease.OutCubic);
        });

        // ディレイで非表示にする
        DOVirtual.DelayedCall(hideDelay, () =>
        {
            textMeshPro.gameObject.SetActive(false);
        });
    }

    #endregion
}
