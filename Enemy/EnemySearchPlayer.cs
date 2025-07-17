using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを探索し、敵の攻撃や移動を制御するスクリプト
/// </summary>
public class EnemySearchPlayer : MonoBehaviour
{
    #region 変数宣言

    [SerializeField] private float searchRadius = 5f;          // プレイヤー探索の半径
    [SerializeField] private float attackLoopInterval = 3f;    // 攻撃判定のループ間隔
    [SerializeField] private float attackDistance = 1.5f;      // 攻撃判定距離
    [SerializeField] private float rayDistance = 5f;           // レイの長さ
    [SerializeField] private Enemy enemy;                      // 操作対象の敵インスタンス

    private const float searchLoopInterval = 0.01f;            // 探索間隔（固定）

    private bool canAttack = true;                             // 攻撃可能か
    private bool playerInFront = false;                        // 正面にプレイヤーがいるか

    private string targetTag = "Player";                       // ターゲットとするタグ

    private Vector3 playerPosition = Vector3.zero;             // プレイヤーの位置

    #endregion

    #region プロパティ

    /// <summary>
    /// プレイヤーが正面にいるかどうか
    /// </summary>
    public bool PlayerInFront { get => playerInFront; set => playerInFront = value; }

    /// <summary>
    /// プレイヤーの現在位置
    /// </summary>
    public Vector3 PlayerPosition { get => playerPosition; set => playerPosition = value; }

    #endregion

    #region Unityイベント

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // プレイヤーを探す処理開始
        StartCoroutine(SearchPlayer());
    }

    /// <summary>
    /// 毎フレームの攻撃判定処理
    /// </summary>
    private void Update()
    {
        // 攻撃が可能なら
        if (canAttack)
        {
            // 攻撃開始して、攻撃不可状態にする
            canAttack = false;
            StartCoroutine(AttackPlayer());
        }

        // デバッグ用レイ表示
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);
    }

    #endregion

    #region コルーチン

    /// <summary>
    /// 一定間隔でプレイヤーを探索する
    /// </summary>
    private IEnumerator SearchPlayer()
    {
        // 無限にループ
        while (true)
        {
            // 指定半径内にある全てのコライダーを取得
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
            bool playerFound = false;

            foreach (Collider collider in hitColliders)
            {
                // プレイヤーが見つかった場合
                if (collider.CompareTag(targetTag))
                {
                    playerFound = true;
                    playerPosition = collider.transform.position;

                    // 向きをプレイヤー方向に補正（Y軸を固定）
                    Vector3 direction = (playerPosition - transform.position).normalized;
                    direction.y = 0;

                    if (direction != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
            }

            // 見つかった情報をEnemy側へ通知
            if (enemy != null)
            {
                enemy.EnemyMove(playerFound, playerPosition);
            }

            // インターバル間待機する
            yield return new WaitForSeconds(searchLoopInterval);
        }
    }

    /// <summary>
    /// 一定間隔で攻撃レイを飛ばして判定する
    /// </summary>
    private IEnumerator AttackPlayer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // レイを可視化（デバッグ用）
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        // レイキャストでプレイヤーを検出
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag(targetTag))
            {
                // 攻撃する
                enemy?.EnemyAttack();
            }
        }

        // 一定時間後に再び攻撃可能とする
        yield return new WaitForSeconds(attackLoopInterval);
        canAttack = true;
    }

    #endregion

    #region カスタムメソッド

    /// <summary>
    /// 攻撃中、目の前にプレイヤーがいるかを判定
    /// </summary>
    public void CheckPlayerInFront()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 指定距離内にプレイヤーがいるかを判定して結果を保存
        playerInFront = Physics.Raycast(ray, out hit, attackDistance) && hit.collider.CompareTag(targetTag);
    }

    #endregion
}
