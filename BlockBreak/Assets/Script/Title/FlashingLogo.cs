using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingLogo : MonoBehaviour {

    Text m_text;

	// Use this for initialization
	void Start () {
        m_text = GetComponent<Text>();

        //iTween.ColorTo(gameObject, iTween.Hash("color", new Color(0, 0, 0, 0), "time", 1.0f, "looptype", iTween.LoopType.pingPong));

        iTween.ValueTo(gameObject,
                        iTween.Hash(
                            "from", m_text.color,
                            "to", new Color(0,0,0,0),
                            "time", 1.0f,
                            "easetype", "easeOutCubic",
                            "onUpdate", "FadeUpdate",
                            "looptype",
                            iTween.LoopType.pingPong
                        )
                    );
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FadeUpdate(Color _color)
    {
        m_text.color = _color;
    }
}
