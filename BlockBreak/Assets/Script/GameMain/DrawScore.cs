using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DrawScore : MonoBehaviour {

    public Text m_ScoreText;

    public float m_nowDrawScore;
	// Use this for initialization
	void Start () {
        m_nowDrawScore = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

        
        if (GameMainManager.Instance.m_NowScore > m_nowDrawScore + 5)
        {
            m_nowDrawScore = Mathf.Lerp(m_nowDrawScore, GameMainManager.Instance.m_NowScore, Time.deltaTime * 10.0f);
        }else{
            m_nowDrawScore = GameMainManager.Instance.m_NowScore;
        }

        //四捨五入
        m_nowDrawScore = Mathf.Ceil(m_nowDrawScore);
        m_ScoreText.text = m_nowDrawScore.ToString();
	}
}
