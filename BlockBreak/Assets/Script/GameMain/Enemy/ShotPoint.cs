using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPoint : MonoBehaviour {

    public GameObject m_BulletPrefab;
    //向き
    public Vector3 m_Direction;
    //ショット間隔
    public float m_ShotInterval;

    float m_timeCnt = -2.0f;

    public float m_BallSpeed;

    List<GameObject> m_instanceBullet = new List<GameObject>();
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        m_timeCnt += Time.deltaTime;

        if (m_timeCnt > m_ShotInterval)
        {
            m_timeCnt = 0;
            GameObject bullet = Instantiate(m_BulletPrefab, transform.position, transform.rotation);
            m_instanceBullet.Add(bullet);
            bullet.GetComponent<BallMain>().m_MoveSpeed = m_BallSpeed;
           
            //向きを設定しなければ　自機の向きで発射する
            if (m_Direction == Vector3.zero)
            {
                bullet.GetComponent<BallMain>().Direction = -transform.root.up;
            }
            else
            {
                bullet.GetComponent<BallMain>().Direction = m_Direction;
            }

        }
	}

    private void OnDestroy()
    {
        for (int i = 0; i < m_instanceBullet.Count; i++)
        {
            if (m_instanceBullet[i] != null)
            {
                Destroy(m_instanceBullet[i]);
            }

        }
    }
}
