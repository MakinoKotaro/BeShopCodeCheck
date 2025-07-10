using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 魔法発射のインターフェイス
/// </summary>
public interface IMagic 
{
    /// <summary>
    /// 魔法発射
    /// </summary>
    /// <param name="castPoint"></param>
    void Cast(Transform castPoint);
}
