using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMain : MonoBehaviour {

    public float m_BallAddSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ball")
        {
            if (collision.transform.GetComponent<BallMain>().MainState.State != BallMain.BallStatus.Attack) return;

            collision.transform.GetComponent<BallMain>().CallAddSpeed(m_BallAddSpeed);

        }

    }


}
