using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUnderbar : MonoBehaviour {

    //アンダーバーのモード
    public enum UnderBarMode {
        None,
        Move,
        Aiming
    }

    public UnderBarMode m_mode;

	// Use this for initialization
	void Start () {

        m_mode = UnderBarMode.None;

	}
	
	// Update is called once per frame
	void Update () {
                            
        MoveUnderBar();
    }

    /// <summary>
    /// アンダーバーの移動
    /// </summary>
    void MoveUnderBar()
    {
        //タッチ情報を取得
        TouchInfo touchinfo = MultiPlatformTouchUtils.GetTouch(0);

        if (touchinfo == TouchInfo.None) return;

        Vector3 touchScreenPosition = MultiPlatformTouchUtils.GetTouchPosition(0);

        //スクリーンから出ないようにする
        touchScreenPosition.x = Mathf.Clamp(touchScreenPosition.x, 0.0f, Screen.width);
        touchScreenPosition.y = Mathf.Clamp(touchScreenPosition.y, 0.0f, Screen.height);

        touchScreenPosition.z = 10.0f;

        Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touchScreenPosition);

        transform.position = new Vector3(touchWorldPosition.x, 0.0f, -5.0f);
    }
}

