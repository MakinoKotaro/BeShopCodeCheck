using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// HPが一定以下のときに警告UI（ビネット）を表示するスクリプト
/// </summary>
public class WarnigUiProcessing : MonoBehaviour
{
    #region 変数宣言

    [SerializeField] private float minValue = 0.05f;                   // ビネットの最小スムース値
    [SerializeField] private float maxValue = 0.2f;                    // ビネットの最大スムース値
    [SerializeField] private float speed = 0.5f;                       // アニメーションスピード
    [SerializeField] private PostProcessVolume volume;                 // ポストプロセスボリューム（Vignette用）

    private float currentValue = 0f;                                   // 現在のスムース値
    private Vignette vignette;                                         // 実際に操作するビネット
    private bool canShowWarningUi = false;                             // 警告UIを表示できるか
    private bool isEffectPlaying = false;                              // エフェクト再生中かどうか

    #endregion

    #region プロパティ

    public bool CanShowWarningUi
    {
        get => canShowWarningUi;
        set
        {
            // 状態を設定
            canShowWarningUi = value;

            // 表示不可にされた場合はビネットをリセット
            if (!canShowWarningUi && vignette != null)
            {
                ResetVignette();
            }
        }
    }

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理（vignetteの取得と初期設定）
    /// </summary>
    private void Start()
    {
        // volumeおよびvolume.profileがnullなら以下の処理を呼ばない
        if (volume != null && volume.profile != null)
        {
            // プロファイルからVignette設定を取得
            if (!volume.profile.TryGetSettings(out vignette))
            {
                // 設定がなければ新規追加
                vignette = volume.profile.AddSettings<Vignette>();
            }

            // 初期状態の設定（強度とスムース）
            vignette.intensity.value = 0f;
            vignette.smoothness.value = minValue;

            // エフェクトを有効化
            vignette.enabled.Override(true);
        }
        else
        {
            // エラーログ出力（設定が未登録の場合）
            Debug.LogError("PostProcessVolume or its profile is null.");
        }
    }

    /// <summary>
    /// 警告UIが表示可能な場合、PingPongでビネットをゆらす
    /// </summary>
    private void Update()
    {
        // 表示不可 または エフェクト再生中 または vignette未取得なら以下の処理を呼ばない
        if (canShowWarningUi && !isEffectPlaying && vignette != null)
        {
            // 時間に応じてPingPong値を計算
            float pingPongValue = Mathf.PingPong(Time.time * speed, 1f);

            // 現在のスムース値を補間
            currentValue = Mathf.Lerp(minValue, maxValue, pingPongValue);

            // ビネットに反映
            vignette.smoothness.value = currentValue;
        }
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 一時的な警告ビネットを表示する（コルーチン使用）
    /// </summary>
    public void PlayWarningEffect()
    {
        // 再生中でなく、vignetteが存在する場合のみ実行
        if (vignette != null && !isEffectPlaying)
        {
            StartCoroutine(PlayEffectCoroutine());
        }
    }

    /// <summary>
    /// 一時的な警告エフェクトの再生処理
    /// </summary>
    private IEnumerator PlayEffectCoroutine()
    {
        // 再生中フラグをオン
        isEffectPlaying = true;

        // 経過時間と継続時間を初期化
        float elapsedTime = 0f;
        float duration = 1.0f / speed;

        // ビネット強度を一時的に0.5へ上昇
        vignette.intensity.value = 0.5f;

        // durationの間、スムース値をゆらす
        while (elapsedTime < duration)
        {
            float pingPongValue = Mathf.PingPong(elapsedTime * speed, 1.0f);
            currentValue = Mathf.Lerp(minValue, maxValue, pingPongValue);
            vignette.smoothness.value = currentValue;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 終了後に初期状態に戻す
        ResetVignette();

        // フラグをオフに
        isEffectPlaying = false;
    }

    /// <summary>
    /// ビネット効果を初期状態に戻す
    /// </summary>
    private void ResetVignette()
    {
        // vignetteが存在し、かつ表示が無効な場合のみ実行
        if (vignette != null && !canShowWarningUi)
        {
            vignette.intensity.value = 0f;
            vignette.smoothness.value = minValue;
        }
    }

    #endregion
}
