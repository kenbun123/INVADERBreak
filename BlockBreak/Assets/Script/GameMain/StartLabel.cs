using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLabel : MonoBehaviour {

    public float m_Interval;

    float m_timeCnt;

    bool m_canCnt = false;
	// Use this for initialization
	void Start () {
        m_timeCnt = 0;
        iTween.MoveTo(this.gameObject, iTween.Hash(
         "y", 0.0f,
         "time", 2.0f,
         "oncomplete", "NextMove",
         "isLocal", true,
         "oncompletetarget", this.gameObject)
        );
    }
	
	// Update is called once per frame
	void Update () {

        //if (transform.position.y <= 0.1f)
        //{
        //    m_timeCnt += Time.deltaTime;
        //}
        //if (m_timeCnt >= m_Interval)
        //{
        //    iTween.MoveTo(this.gameObject, iTween.Hash(
        //        "y", 1300,
        //        "time", 2.0f)
        //        );
        //    if (transform.position.y >= 1299)
        //    {

        //        Destroy(this.gameObject);
        //    }
        //}

        if (m_canCnt)
        {
            m_timeCnt += Time.deltaTime;
        }
        if (m_timeCnt >= m_Interval)
        {
            EndMove();
            m_canCnt = false;
            m_timeCnt = 0.0f;
        }
        

    }

    void NextMove()
    {
       // Debug.Log("call");
        m_canCnt = true;
    }
    void EndMove()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash(
                 "y", 800.0f,
                 "time", 2.0f,
                 "isLocal", true,
                 "oncomplete", "CallEnd",
                 "oncompletetarget", this.gameObject));
    }

    void CallEnd()
    {
        
        GameMainManager.Instance.m_GameModeFSM.ChangeState(GameMainManager.GameMode.Normal);
        gameObject.SetActive(false);
    }
}
