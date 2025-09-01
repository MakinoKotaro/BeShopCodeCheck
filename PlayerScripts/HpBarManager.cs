using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HPの可視化スクリプト（UI上にHPバーや毒状態を表示する）
/// </summary>
public class HpBarManager : MonoBehaviour
{
    #region 変数宣言

    [SerializeField] private Image hpImage;              // HPバーのImage
    [SerializeField] private Image takePoisonImage;      // 毒状態表示用Image

    private Color originalColor;                         // HPバーの元の色
    private GameObject player;                           // プレイヤーオブジェクト
    private PlayerParameter playerParameter;             // プレイヤーの状態を保持するクラス

    private float nowFillAmount = 1f;                    // 現在のHP割合
    
    private static readonly Color poisonedColor = Color.blue; // 毒状態時の色
    private static readonly Color normalColor = Color.red;    // 通常時の色

    #endregion

    #region プロパティ

    /// <summary>
    /// 現在のHP割合を取得・設定
    /// </summary>
    public float NowFillAmount
    {
        get => nowFillAmount;
        set => nowFillAmount = value;
    }

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // プレイヤーオブジェクトをタグで検索
        player = GameObject.FindWithTag("Player");

        // プレイヤーが存在する場合
        if (player != null)
        {
            // PlayerParameterコンポーネントを取得
            playerParameter = player.GetComponent<PlayerParameter>();
        }

        // 毒状態Imageは初期状態では非表示
        takePoisonImage.enabled = false;
    }

    /// <summary>
    /// 毎フレーム実行される処理
    /// </summary>
    private void Update()
    {
        // hpImageかplayerParameterがnullなら以下の処理を呼ばない
        if (hpImage == null || playerParameter == null) return;

        // プレイヤーが毒状態の場合
        bool isPoisoned = playerParameter.TakePoison;

        // 毒状態Imageの表示切り替え
        takePoisonImage.enabled = isPoisoned;

        // HPバーの色変更処理
        ChangeHpBarColor(isPoisoned);
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 現在のHPをUI上に反映する
    /// </summary>
    /// <param name="hp">0〜1のHP割合</param>
    public void ShowCurrentHp(float hp)
    {
        // hpImageが未設定なら以下の処理を呼ばない
        if (hpImage == null)
        {
            Debug.LogError("hpImage is missing");
            return;
        }

        // fillAmountでHPバーを更新
        hpImage.fillAmount = hp;

        // 現在の値を保持
        nowFillAmount = hp;

        // 現在の色を保存（復元用）
        originalColor = hpImage.color;
    }

    /// <summary>
    /// 毒状態によってHPバーの色を変更する
    /// </summary>
    /// <param name="takePoison">毒状態フラグ</param>
    private void ChangeHpBarColor(bool takePoison)
    {
        // takePoisonがtrueの場合 → 青に、falseの場合 → 赤に設定
        hpImage.color = takePoison ? poisonedColor : normalColor;
    }

    #endregion
}
