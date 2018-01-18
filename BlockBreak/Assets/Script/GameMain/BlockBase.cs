using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBase : MonoBehaviour {
    [Header("HP")]
    [SerializeField]
    private float m_Hp;
    
    public float HP
    {
        get { return m_Hp; }
        //set { m_Hp = value; }
    }

    public void HitDamage(float _damage)
    {
        m_Hp -= _damage;

    }
}
