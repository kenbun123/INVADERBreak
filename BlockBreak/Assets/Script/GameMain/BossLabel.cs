using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossLabel : MonoBehaviour {

    //点滅フラグ
    bool m_canFlashing;

    Image m_image;
    public float m_Speed;
    float m_timeCnt;
    public float m_Interval;
	// Use this for initialization
	void Start () {
        m_image = GetComponent<Image>();
        m_canFlashing = true;
        m_timeCnt = 0.0f;
        //SoundManager.Instance.PlaySe("BossHeep");
	}

    void OnEnable()
    {
        m_canFlashing = true;
        m_timeCnt = 0.0f;
    }

    // Update is called once per frame
    void Update () {
        SoundManager.Instance.PlaySe("BossHeep");
        m_timeCnt += Time.deltaTime;
        if (m_timeCnt >= m_Interval)
        {
            GameMainManager.Instance.CallEndBossLabel();
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        if (m_canFlashing)
        {
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, m_image.color.a + Time.deltaTime * m_Speed);
            if (m_image.color.a >= 1.0f)
            {
                m_canFlashing = false;
                
            }
            
        }
        else
        {
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, m_image.color.a - Time.deltaTime * m_Speed);
            if (m_image.color.a <= 0)
            {
                m_canFlashing = true;
            }
        }
	}

}
