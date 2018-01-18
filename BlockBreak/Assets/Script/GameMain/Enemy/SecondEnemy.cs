using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class SecondEnemy : NormalEnemy {

    /// <summary>
    /// 回転速度
    /// </summary>
    public float m_AngleSpeed;

    public float m_MinAngle;
    public float m_MaxAngle;

    float m_angle = 180;
    //回転方向
    bool m_isAngle = true;



    private void Awake()
    {
        m_mainFSM = StateMachine<MovePattern>.Initialize(this, MovePattern.Start);
    }
    // Use this for initialization
    void Start () {
		
	}
    void Start_Enter()
    {
        m_oldScaleX = transform.localScale.x;
        transform.SetLocalScaleX(0.01f);

    }


    void Start_Update()
    {

        iTween.ScaleTo(this.gameObject, iTween.Hash(
            "x", m_oldScaleX,
            "time", 3.0f)
            );

        iTween.MoveTo(this.gameObject, iTween.Hash(
             "position", new Vector3(transform.position.x, transform.position.y, m_StopZpos),
                "time", 2.0f)
            );

        if (transform.position.z <= m_StopZpos + 0.01f)
        {
            m_mainFSM.ChangeState(MovePattern.Attack);
        }
    }



    void Attack_Update()
    {
        if (m_isAngle)
        {
            m_angle += Time.deltaTime * m_AngleSpeed;
        }
        else
        {
            m_angle -= Time.deltaTime * m_AngleSpeed;
        }

        if (m_angle > m_MaxAngle)
        {
            m_isAngle = false;
        }

        if (m_angle < m_MinAngle)
        {
            m_isAngle = true;
        }


        transform.SetEulerAnglesY(m_angle);
        transform.position += m_diretion * m_moveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update () {

    }
}
