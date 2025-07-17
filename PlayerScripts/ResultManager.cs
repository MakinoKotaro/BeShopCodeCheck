using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    #region 変数宣言

    private Animator animator;                               // アニメーターコンポーネントの参照

    [SerializeField] private string victoryAnimationName;    // 勝利時に再生するアニメーション名

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();

        // animatorがnullでなく、かつアニメーション名が空でない場合
        if (animator != null && !string.IsNullOrEmpty(victoryAnimationName))
        {
            // 指定された勝利アニメーションを再生
            animator.Play(victoryAnimationName);
        }
    }

    #endregion
}
