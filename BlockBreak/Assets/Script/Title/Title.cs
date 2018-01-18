using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    // Use this for initialization
    void Start () {
        SoundManager.Instance.PlayBgm("gamestart");
    }
	
	// Update is called once per frame
	void Update () {
        TouchInfo touchInfo = MultiPlatformTouchUtils.GetTouch(0);
        if (touchInfo == TouchInfo.Began)
        {
            SoundManager.Instance.PlaySe("start");
            SceneNavigator.Instance.Change("PlayInfo", 0.3f);
        }
    }
}
