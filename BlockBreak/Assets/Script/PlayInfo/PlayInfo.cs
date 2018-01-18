using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayInfo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TouchInfo touchInfo = MultiPlatformTouchUtils.GetTouch(0);
        if (touchInfo == TouchInfo.Began)
        {
            SoundManager.Instance.PlaySe("start");
            SceneNavigator.Instance.Change("Main", 0.3f);
        }
    }
}
