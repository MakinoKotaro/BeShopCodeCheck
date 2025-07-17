using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵が攻撃をやめる処理を定義するインターフェイス
/// </summary>
public interface IE_StopAttack
{
    /// <summary>
    /// 敵が攻撃を中止する処理
    /// </summary>
    void EnemyStopAttack();
}
