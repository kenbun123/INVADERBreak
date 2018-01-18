using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMain : MonoBehaviour {

    //ステート
    public enum BallStatus
    {
        Normal,
        
        Attack
    }
    //ステートマシン
    StateMachine<BallStatus> m_mainFSM;

    public StateMachine<BallStatus> MainState
    {
         get {return m_mainFSM;}
    }


    public GameObject m_P_BlockBreak;
    //現在のステート
    public BallStatus m_NowStatus;

    [SerializeField]
    Vector3 m_direction = Vector3.zero;            //向き

    public float m_MoveSpeed = 0.0f;              //移動速度
    public int m_MaxHitCnt;
    int m_hitCnt = 0;

    public float m_ExtinctionTime;                  //ボール消滅タイム
    public float m_Damage = 0.0f;                  //攻撃力
    public GameObject m_lineManager;

    public Material m_attackColor;

    public float MoveSpeed
    {
        set { m_MoveSpeed = value; }
    }
    public Vector3 Direction
    {
        get { return m_direction; }
        set { m_direction = value; }
    }

   

    private void Awake()
    {
        m_mainFSM = StateMachine<BallStatus>.Initialize(this, BallStatus.Normal);
    }
    private void Start()
    {
        //m_rb = GetComponent<Rigidbody>();
        m_lineManager = LineManager.Instance.gameObject;
        StartCoroutine("Extinction");

    }
    private void Update()
    {
        m_NowStatus = m_mainFSM.State;


    }
    void Normal_Update(){

        transform.position  += m_direction * m_MoveSpeed * Time.deltaTime ;

    }

    void Attack_Enter()
    {
        transform.GetComponent<MeshRenderer>().material = m_attackColor;
        transform.gameObject.layer = 8;
    }

    void Attack_Update(){

        transform.position += m_direction * m_MoveSpeed * Time.deltaTime;
        if (m_hitCnt >= m_MaxHitCnt)
        {
            Destroy(this.gameObject);
        }
    }

    public void CallAddSpeed(float _speed)
    {
        m_MoveSpeed = _speed;
    }

    /// <summary>
    /// 時間経過による消滅
    /// </summary>
    /// <returns></returns>
    IEnumerator Extinction()
    {
        float time = 0.0f;
        while (time < m_ExtinctionTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    #region 当たり判定
    //当たり判定

    Vector3 ReflectObj(Collision other)
    {
        Vector3 reflect = Vector3.zero;
        Vector3 framenormal = other.contacts[0].normal;
        framenormal = Vector3.Normalize(framenormal);
        reflect = Vector3.Reflect(m_direction, framenormal);
        reflect.y = 0.0f;

        return reflect;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ScreenFrame")
        {
            HitScreenFrame(other);
        }
        if (other.gameObject.tag == "Line")
        {
            if (m_mainFSM.State == BallStatus.Normal)
                HitLine(other);
        }
        if (other.gameObject.tag == "Block")
        {
            if(m_mainFSM.State == BallStatus.Attack)
            HitBlock(other);
        }
 
    }

    void HitLine(Collision other)
    {
        SoundManager.Instance.PlaySe("shototsu2");
        m_direction = ReflectObj(other);
        m_mainFSM.ChangeState(BallStatus.Attack);
    }

    void HitScreenFrame(Collision other)
    {
        if (m_mainFSM.State == BallStatus.Attack)
        {
            m_hitCnt++;
            m_direction = ReflectObj(other);
        }
        else {
            Destroy(this.gameObject);
        }


    }

    //ブロックに当たった場合
    void HitBlock(Collision other)
    {
        SoundManager.Instance.PlaySe("shototsu4");
        if (other.transform.GetComponent<WeakPointBlock>())
        {
            //死亡時爆発を生成
            SoundManager.Instance.PlaySe("hasamu3");
            Instantiate(m_P_BlockBreak, transform.position, m_P_BlockBreak.transform.rotation);
            GameMainManager.Instance.CallAddScore(100.0f);
            other.transform.GetComponent<WeakPointBlock>().CallDestroyAllBlock();
        }
        else
        {
            //
            other.transform.root.GetComponent<NormalEnemy>().CallHitAnim();
            GameMainManager.Instance.CallAddScore(10.0f);
        }
        other.transform.GetComponent<BlockBase>().HitDamage(m_Damage);
        m_P_BlockBreak.GetComponent<ParticleSystem>().Play();



        Destroy(this.gameObject);
        Destroy(other.gameObject);
            //m_direction = ReflectObj(other);
    }



    #endregion

}
