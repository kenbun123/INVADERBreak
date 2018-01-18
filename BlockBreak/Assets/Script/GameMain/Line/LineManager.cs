using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : SingletonMonoBehaviour<LineManager> {

    /// <summary>
    /// ラインのプレハブ
    /// </summary>
    public GameObject m_LinePrefab = null;

    /// <summary>
    /// 最初にタッチした座標
    /// </summary>
    Vector3 m_firstTouch = Vector3.zero;

    /// <summary>
    /// 最後にタッチした座標
    /// </summary>
    Vector3 m_endTouch = Vector3.zero;

    public GameObject m_lineObj = null;

    /// <summary>
    /// タッチしているかどうか
    /// </summary>
    public bool m_IsTouch = false;

    /// <summary>
    /// ラインの最小サイズ
    /// </summary>
    [Header("ライン最小サイズ")]
    public float m_LineMinSize = 0.0f;


    [Header("ライン最大サイズ")]
    public float m_LineMaxSize = 0.0f;

    public LayerMask m_Mask;
    
	// Use this for initialization
	void Start () {


        m_IsTouch = false;
        m_firstTouch = Vector3.zero;
        m_endTouch = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {

        TouchControl();


        if (!m_lineObj) return;

        LineControl();
    }

    void TouchControl()
    {
        TouchInfo touchInfo = MultiPlatformTouchUtils.GetTouch(0);

        switch (touchInfo)
        {
            //押された瞬間
            case TouchInfo.Began:
                //TODO.　RayCastの追加
                if (!Physics.Raycast(ChangePosition(MultiPlatformTouchUtils.GetTouchWorldPosition(Camera.main, 0)), -Vector3.up, 100.0f,m_Mask))
                {
                    Debug.Log("hit");
                    break;
                }
                //


                if (m_IsTouch) break;
                m_IsTouch = true;

                //時間を遅くする
                Time.timeScale = 0.1f;
                m_firstTouch = ChangePosition(MultiPlatformTouchUtils.GetTouchWorldPosition(Camera.main, 0));
                m_endTouch = ChangePosition(MultiPlatformTouchUtils.GetTouchWorldPosition(Camera.main, 0));

                //既存のラインを消す
                if (m_lineObj)
                {
                    Destroy(m_lineObj);
                }
                break;
            //
            case TouchInfo.Moved:
                if (!m_IsTouch) break;
                if (m_lineObj != null)
                {
                    //伸びている点の座標を計算
                    Vector3 direction = Vector3.Normalize(m_endTouch - m_firstTouch);
                    Vector3 linemaxpos = m_firstTouch + (direction * m_lineObj.transform.localScale.x);

                    //line描画可能エリアにいなければモデルを赤にする
                    if (!Physics.Raycast(linemaxpos, -Vector3.up, 100.0f, m_Mask))
                    {

                        m_lineObj.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    else
                    {
                        m_lineObj.GetComponent<MeshRenderer>().material.color = Color.white;
                    }
                }
                m_endTouch = ChangePosition(MultiPlatformTouchUtils.GetTouchWorldPosition(Camera.main, 0));

                break;
            case TouchInfo.None:
            case TouchInfo.Canceled:
            case TouchInfo.Ended:


                if (m_IsTouch == true)
                {
                    Time.timeScale = 1.0f;
                }
                m_IsTouch = false;
                if (m_lineObj)
                {
                    m_lineObj.GetComponent<BoxCollider>().enabled = true;
                    //伸びている点の座標を計算
                    Vector3 direction = Vector3.Normalize(m_endTouch - m_firstTouch);
                    Vector3 linemaxpos = m_firstTouch + (direction * m_lineObj.transform.localScale.x);
                    //line描画可能エリアにいなければ削除
                    if (!Physics.Raycast(linemaxpos, -Vector3.up,100.0f, m_Mask))
                    {
                        m_firstTouch = Vector3.zero;
                        m_endTouch = Vector3.zero;
                        Destroy(m_lineObj);
                    }
                }

                break;

        }
        if (Vector3.Distance(m_firstTouch,m_endTouch)>= m_LineMinSize && !m_lineObj )
        {
            //ラインを生成
            m_lineObj = Instantiate(m_LinePrefab, m_firstTouch, m_LinePrefab.transform.rotation);


        }


    }

    void LineControl()
    {
        //距離を取得
        float linedistance = Vector3.Distance(m_firstTouch, m_endTouch);
        //角度を計算
        m_lineObj.transform.rotation = Quaternion.Euler(new Vector3(0, GetRadian(m_firstTouch, m_endTouch) * Mathf.Rad2Deg * -1.0f, 0));


        if (linedistance >= m_LineMaxSize)
        {
            Vector3 direction = Vector3.Normalize(m_endTouch - m_firstTouch);
            Vector3 linemaxpos = m_firstTouch + (direction * m_LineMaxSize);
            m_lineObj.transform.position = GetLineCenterPos(m_firstTouch,linemaxpos);
        }
        else {
            //ラインの中心点をセット
            m_lineObj.transform.position = GetLineCenterPos(m_firstTouch, m_endTouch);
            m_lineObj.transform.localScale = new Vector3(linedistance, m_lineObj.transform.localScale.y, m_lineObj.transform.localScale.z);
        }

    }

    /// <summary>
    /// 座標変換
    /// </summary>
    /// <returns>The position.</returns>
    /// <param name="pos">Position.</param>
    Vector3 ChangePosition(Vector3 pos)
    {
        pos = new Vector3(pos.x, 0.0f, pos.z);
        return pos;

    }

    Vector3 GetLineCenterPos(Vector3 v1,Vector3 v2)
    {
        Vector3 center = new Vector3((v1.x + v2.x) / 2, 0.0f, (v1.z + v2.z) / 2);

        return center;
    }

    /// <summary>
    /// Gets the Vector radian.
    /// </summary>
    /// <returns>The radian.</returns>
    /// <param name="v1">V1.</param>
    /// <param name="v2">V2.</param>
    float GetRadian(Vector3 v1, Vector3 v2)
    {

        float radian = Mathf.Atan2(v2.z - v1.z, v2.x - v1.x);
        return radian;
    }
}
