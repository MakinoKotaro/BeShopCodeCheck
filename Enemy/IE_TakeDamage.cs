using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵が攻撃を受けるインターフェイス
/// </summary>
public interface IE_TakeDamage
{
    /// <summary>
    /// 敵が攻撃を受ける
    /// </summary>
    /// <param name="damageAmount">受けるダメージ量</param>
    void EnemyTakeDamage(float damageAmount);
}
