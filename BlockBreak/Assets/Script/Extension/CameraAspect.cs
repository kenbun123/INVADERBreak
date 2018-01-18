using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraAspect : MonoBehaviour
{

    //public float x_aspect = 10.0f;
    //public float y_aspect = 16.0f;

    //void Awake()
    //{
    //    Camera camera = GetComponent<Camera>();
    //    Rect rect = calcAspect(x_aspect, y_aspect);
    //    camera.rect = rect;
    //}

    //private Rect calcAspect(float width, float height)
    //{
    //    float target_aspect = width / height;
    //    float window_aspect = (float)Screen.width / (float)Screen.height;
    //    float scale_height = window_aspect / target_aspect;
    //    Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

    //    if (1.0f > scale_height)
    //    {
    //        rect.x = 0;
    //        rect.y = (1.0f - scale_height) / 2.0f;
    //        rect.width = 1.0f;
    //        rect.height = scale_height;
    //    }
    //    else
    //    {
    //        float scale_width = 1.0f / scale_height;
    //        rect.x = (1.0f - scale_width) / 2.0f;
    //        rect.y = 0.0f;
    //        rect.width = scale_width;
    //        rect.height = 1.0f;
    //    }
    //    return rect;
    //}
    void Start()
    {
        Camera cam = gameObject.GetComponent<Camera>();

        // 理想の画面の比率
        float targetRatio = 10f / 16f;
        // 現在の画面の比率
        float currentRatio = Screen.width * 1f / Screen.height;
        // 理想と現在の比率
        float ratio = targetRatio / currentRatio;

        //カメラの描画開始位置をX座標にどのくらいずらすか
        float rectX = (1.0f - ratio) / 2f;
        //カメラの描画開始位置と表示領域の設定
        cam.rect = new Rect(rectX, 0f, ratio, 1f);
    }

}