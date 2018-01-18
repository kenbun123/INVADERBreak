using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;


public class NormalEnemy : MonoBehaviour {

    public enum MovePattern
    {
        Start,
        Attack,
        Death
    }

    //ステートマシン
   protected StateMachine<MovePattern> m_mainFSM;

   [SerializeField][Header("移動速度")]
    protected float m_moveSpeed;

    [SerializeField][Header("向き")]
    protected Vector3 m_diretion;


    //元のXスケール
    protected float m_oldScaleX;

    //最初に止まるZ座標
    public float m_StopZpos;

    public Vector3 Diretion
    {
        get { return m_diretion; }
        set { m_diretion = value; }
    }

    public float MoveSpeed
    {
        set { m_moveSpeed = value; }
    }

    private void Awake()
    {
        m_mainFSM = StateMachine<MovePattern>.Initialize(this, MovePattern.Start);
    }
    // Use this for initialization
    void Start () {

    }

   // Update is called once per frame
 //   void Update () {


	//}

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


        if (transform.position.z <= m_StopZpos + 0.1f)
        {
            m_mainFSM.ChangeState(MovePattern.Attack);
        }

    }



    void Attack_Update()
    {

        transform.position += m_diretion * m_moveSpeed * Time.deltaTime;
    }


    //ダメージ食らった時の演出
    public void CallHitAnim()
    {
        iTween.PunchPosition(this.gameObject, iTween.Hash(
                "y", 0.5f,
                "time", 0.5f)
            );
    }


}
