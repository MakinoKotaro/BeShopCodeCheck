using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを管理するスクリプト
/// </summary>
public class PlayerAnimationScript : MonoBehaviour
{
    #region 変数宣言

    private Animator animator;                                     // アニメーター本体

    [SerializeField] private GameObject sFXManageObj;              // 効果音マネージャの参照

    [Header("各アニメーション")]
    [SerializeField] private string idle_animation;                // 待機アニメーション名
    [SerializeField] private string normal_attack_animation;       // 通常攻撃アニメーション名
    [SerializeField] private string area_masic_animation;          // 範囲魔法アニメーション名
    [SerializeField] private string walk_animation;                // 歩行アニメーション名
    [SerializeField] private string dodge_animation;               // 回避アニメーション名
    [SerializeField] private string fall_animation;                // 落下アニメーション名
    [SerializeField] private string dead_animation;                // 死亡アニメーション名

    private string nowAnimation = "";                              // 現在再生中のアニメーション名
    private bool is_playing_animation = false;                     // アニメーション再生中かどうか
    private bool stopping = true;                                  // 停止中かどうか

    private float originalAnimationSpeed = 1f;                     // 通常再生速度
    private float fastAnimationSpeed = 1.5f;                       // 高速再生用の速度

    private bool isPlayingDodgeAnimation;                          // Dodgeアニメーションが再生中かどうか

    #endregion

    #region プロパティ

    public Animator Animator { get => animator; set => animator = value; }
    public bool IsPlayingDodgeAnimation { get => isPlayingDodgeAnimation; set => isPlayingDodgeAnimation = value; }

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 毎フレーム更新
    /// </summary>
    private void Update()
    {
        // 各レイヤーのアニメーション状態を取得
        AnimatorStateInfo layer0 = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo layer1 = animator.GetCurrentAnimatorStateInfo(1);

        // アニメーションが終了済み、または停止状態、再生中フラグが立っていれば、再生中フラグをリセット
        if ((layer0.normalizedTime >= 1f && layer0.length > 0) ||
            (layer1.normalizedTime >= 1f && layer1.length > 0) ||
            stopping || is_playing_animation)
        {
            is_playing_animation = false;
        }

        // Dodge中は通常速度、それ以外は高速再生
        animator.speed = nowAnimation == "Dodge" ? originalAnimationSpeed : fastAnimationSpeed;

        // Dodgeアニメーション再生中かどうかを判定（第2レイヤー）
        AnimatorStateInfo layer2 = animator.GetCurrentAnimatorStateInfo(2);
        isPlayingDodgeAnimation = layer2.length > 0 && layer2.normalizedTime < 1f;
    }

    #endregion

    #region アニメーション再生メソッド

    /// <summary>
    /// 待機アニメーションを再生
    /// </summary>
    public void PlayIdleAnim()
    {
        // 現在のアニメーションをIdleに設定
        nowAnimation = "Idle";

        // 停止中として扱う
        stopping = true;

        // Idleアニメーションを再生
        animator.Play(idle_animation);
    }

    /// <summary>
    /// 歩行アニメーションを再生
    /// </summary>
    public void PlayWalkAnim()
    {
        // 停止フラグを解除
        stopping = false;

        // アニメーション再生中なら以下の処理を呼ばない
        if (is_playing_animation) return;

        // 現在のアニメーションをWalkに設定
        nowAnimation = "Walk";

        // 歩行アニメーションを再生
        animator.Play(walk_animation);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;
    }

    /// <summary>
    /// 走行アニメーションを再生（現状はWalkと同じ）
    /// </summary>
    public void PlayRunAnim()
    {
        // Walkアニメーションを再生（Run専用アニメが未設定）
        animator.Play(walk_animation);
    }

    /// <summary>
    /// 通常攻撃アニメーションを再生
    /// </summary>
    public void PlayNormalAttackAnim()
    {
        // 現在のアニメーションをNormalAttackに設定
        nowAnimation = "NormalAttack";

        // 通常攻撃アニメーションを再生
        animator.Play(normal_attack_animation);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;
    }

    /// <summary>
    /// 範囲魔法アニメーションを再生
    /// </summary>
    public void PlayAreaMagicAnim()
    {
        // 現在のアニメーションをAreaMagicに設定
        nowAnimation = "AreaMagic";

        // 範囲魔法アニメーションを再生
        animator.Play(area_masic_animation);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;
    }

    /// <summary>
    /// 回避アニメーションを再生（第2レイヤーを使用）
    /// </summary>
    public void PlayDodgeAnim()
    {
        // 現在のアニメーションをDodgeに設定
        nowAnimation = "Dodge";

        // 第2レイヤーでDodgeアニメーションを再生
        animator.Play(dodge_animation, 2);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;

        // 効果音マネージャが存在する場合はSEを再生
        if (sFXManageObj.TryGetComponent(out SFXManager sFXManager))
        {
            sFXManager.SetDodgetSound();
        }
    }

    /// <summary>
    /// 落下アニメーションを再生
    /// </summary>
    public void PlayFallAnim()
    {
        // 現在のアニメーションをFallに設定
        nowAnimation = "Fall";

        // 落下アニメーションを再生
        animator.Play(fall_animation);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;
    }

    /// <summary>
    /// 死亡アニメーションを再生
    /// </summary>
    public void PlayDeadAnim()
    {
        // 現在のアニメーションをDeadに設定
        nowAnimation = "Dead";

        // 死亡アニメーションを再生
        animator.Play(dead_animation);

        // アニメーション再生中フラグを立てる
        is_playing_animation = true;
    }

    #endregion
}
