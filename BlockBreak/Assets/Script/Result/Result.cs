using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Result : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlayBgm("gameover");
    }
	
	// Update is called once per frame
	void Update () {
        TouchInfo touchInfo = MultiPlatformTouchUtils.GetTouch(0);
        if (touchInfo == TouchInfo.Began)
        {
            SoundManager.Instance.PlaySe("start");
            SceneNavigator.Instance.Change("Title", 0.3f);
            Destroy(GameMainManager.Instance.gameObject);
        }
    }

}
