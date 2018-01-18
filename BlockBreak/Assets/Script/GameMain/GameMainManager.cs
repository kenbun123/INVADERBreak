using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMainManager : SingletonMonoBehaviour<GameMainManager> {

    public enum GameMode {
        Start,
        Normal,
        Boss,
        GameOver

    }

    public enum EnemyType
    {
        Enemy1,
        Enemy2,
        Max_Enemy
    }

    public float m_NowScore = 0;
    public float m_MaxScore;

    public StateMachine<GameMode> m_GameModeFSM;

    [Header("生成時間")]
    public List<float> m_intervalTime = new List<float>((int)EnemyType.Max_Enemy);
    List<float> m_timeCnt = new List<float>((int)EnemyType.Max_Enemy);
    [Header("エネミープレハブ")]
    public List<GameObject> m_EnemyPrefab = new List<GameObject>();
    public List<GameObject> m_instanceEnemy = new List<GameObject>();

    //ボスステージのポイント
    public float m_bossStagePoint;
    float m_nowBossStagePoint;
    public GameObject m_BossLabel;
    bool m_CanSpawnBoss = false;    
    private void Awake()
    {
        m_GameModeFSM = StateMachine<GameMode>.Initialize(this,GameMode.Start);
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start () {
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("main");
        m_NowScore = 0.0f;
        m_nowBossStagePoint = m_bossStagePoint;

        m_GameModeFSM.ChangeState(GameMode.Start);
        for (int i = 0; i < (int)EnemyType.Max_Enemy; i++)
        {
            m_timeCnt.Add(0.0f);
        }
	}
	
	// Update is called once per frame
	void Update () {

        //最大Scoreを超えないようにする
        if (m_NowScore > m_MaxScore)
        {
            m_NowScore = m_MaxScore;
        }
	}

#region ノーマルフェーズ
    void Normal_Enter()
    {
        GameObject enemy = Instantiate(m_EnemyPrefab[(int)EnemyType.Enemy2], new Vector3(Random.Range(-1.7f, 1.7f), 0, 12), m_EnemyPrefab[(int)EnemyType.Enemy2].transform.rotation);
        m_instanceEnemy.Add(enemy);

    }
    void Normal_Update()
    {
        //ボスPhaseのScoreを超えるとボスフェーズを開始する
        if (m_NowScore>= m_nowBossStagePoint)
        {
            if (m_instanceEnemy.Count == 0)
            {
                m_GameModeFSM.ChangeState(GameMode.Boss);
                
            }
            return;
        }

        //生成間隔
        for (int i = 0; i < (int)EnemyType.Max_Enemy; i++)
        {
            m_timeCnt[i] += Time.deltaTime;
            if (m_timeCnt[i] >= m_intervalTime[i])
            {
                GameObject enemy = Instantiate(m_EnemyPrefab[i], new Vector3(0, 0,12),m_EnemyPrefab[i].transform.rotation);
                m_instanceEnemy.Add(enemy);
                m_timeCnt[i] = 0.0f;
            }
        }

    }
#endregion

    #region ボスフェーズ
    void Boss_Enter()
    {
        m_BossLabel.SetActive(true);


    }

    void Boss_Update()
    {
        if(m_CanSpawnBoss)
        {
            if (m_instanceEnemy.Count == 0)
            {
                m_GameModeFSM.ChangeState(GameMode.Normal);
            }
        }


    }

    void Boss_Exit()
    {

        m_CanSpawnBoss = false;
        for (int i = 0; i < (int)EnemyType.Max_Enemy; i++)
        {
            m_timeCnt[i] = 0.0f;
            //生成間隔を短縮
            m_intervalTime[i] -= 1.5f;
        }
        m_nowBossStagePoint *= 3;
    }
#endregion



    //終了フラグ
    public void CallEndStage()
    {
        SceneNavigator.Instance.Change("Result", 0.3f);
    }

    public void CallEndBossLabel()
    {
        m_CanSpawnBoss = true;
        GameObject enemy = Instantiate(m_EnemyPrefab[m_EnemyPrefab.Count - 1], new Vector3(0, 0, 12), m_EnemyPrefab[m_EnemyPrefab.Count - 1].transform.rotation);
        m_instanceEnemy.Add(enemy);
    }

    //Scoreの加算
    public void CallAddScore(float _score)
    {
        m_NowScore += _score;
    }



}
