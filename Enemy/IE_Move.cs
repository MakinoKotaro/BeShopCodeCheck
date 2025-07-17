using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の移動処理を定義するインターフェイス
/// </summary>
public interface IE_Move
{
    /// <summary>
    /// 敵の移動を実行する
    /// </summary>
    /// <param name="foundPlayer">プレイヤーを発見したか</param>
    /// <param name="playerPosition">プレイヤーの位置</param>
    void EnemyMove(bool foundPlayer, Vector3 playerPosition);
}
