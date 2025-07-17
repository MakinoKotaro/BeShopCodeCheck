using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵が死亡したときの処理を定義するインターフェイス
/// </summary>
public interface IE_Dead
{
    /// <summary>
    /// 敵が死亡した際の処理を実行する
    /// </summary>
    void EnemyDead();
}
