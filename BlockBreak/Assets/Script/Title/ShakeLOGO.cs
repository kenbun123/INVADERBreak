using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeLOGO : MonoBehaviour {

    public float m_X;
    public float m_Y;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        iTween.PunchScale(this.gameObject, iTween.Hash(
                    "x", m_X,
                    "y", m_Y,
                     "isLocal", true)
                );
    }
}
