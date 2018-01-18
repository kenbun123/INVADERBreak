using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMain : MonoBehaviour {
    [SerializeField]
    private float m_hp;

    public GameObject m_HpBar;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (m_hp <= 0.0f)
        {
            //GameOver
            //SceneNavigator.Instance.Change("Title", 0.5f);
            GameMainManager.Instance.CallEndStage();
        }

        m_HpBar.GetComponent<Hpbar>().SetHp(m_hp);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ball")
        {
            //Debug.Log("ボールに当った");
            if (collision.transform.GetComponent<BallMain>().MainState.State != BallMain.BallStatus.Normal) return;

            SoundManager.Instance.PlaySe("hasamu2");
            Destroy(collision.gameObject);
            HitDamge(collision.transform.GetComponent<BallMain>().m_Damage);

           

        }
    }

    //ダメージを食らう
    void HitDamge(float _damage)
    {
        m_hp -= _damage;
        iTween.ShakeRotation(this.transform.root.gameObject, iTween.Hash(
            "z", 15,
            "time", 0.5f)
        );
    }
    
}
