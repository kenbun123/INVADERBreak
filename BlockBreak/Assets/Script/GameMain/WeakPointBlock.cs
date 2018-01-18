using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPointBlock : BlockBase {

    /// <summary>
    /// 自分の本体を全部破壊
    /// </summary>
    public void CallDestroyAllBlock()
    {
        GameMainManager.Instance.m_instanceEnemy.Remove(transform.root.gameObject);
        Destroy(transform.root.gameObject);
    }
}
