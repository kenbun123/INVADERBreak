using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour {

    CanvasGroup m_canvasGroupEntity;

    CanvasGroup m_canvasGroup
    {
        get
        {
            if (m_canvasGroupEntity == null)
            {
                m_canvasGroupEntity = GetComponent<CanvasGroup>();
                if (m_canvasGroupEntity == null)
                {
                    m_canvasGroupEntity = gameObject.AddComponent<CanvasGroup>();
                }
            }
            return m_canvasGroupEntity;
        }
    }

    public float Alpha
    {
        get
        {
            return m_canvasGroup.alpha;
        }
        set
        {
            m_canvasGroup.alpha = value;
        }
    }

    //フェードの状態
    enum FadeState
    {
        None, FadeIn, FadeOut
    }

    private FadeState m_fadeState = FadeState.None;

    //フェードしているか
    public bool IsFading
    {
        get { return m_fadeState != FadeState.None; }
    }

    //フェード時間
    [SerializeField]
    float m_duration;
    public float Duration { get { return m_duration; } }

    //タイムスケールを無視するか
    [SerializeField]
    private bool m_ignoreTimeScale = true;

    //フェード終了後のコールバック
    private event Action m_onFinished = null;

    // Use this for initialization
    void Start () {
		
	}

    void Update()
    {
        if (!IsFading)
        {
            return;
        }

        float fadeSpeed = 1.0f / m_duration;
        if (m_ignoreTimeScale){
            fadeSpeed *= Time.unscaledDeltaTime;
        }else{
            fadeSpeed *= Time.deltaTime;
        }

        Alpha += fadeSpeed * (m_fadeState == FadeState.FadeIn ? 1.0f : -1.0f);

        //フェード終了判定
        if (Alpha > 0 && Alpha < 1)
        {
            return;
        }

        m_fadeState = FadeState.None;
        this.enabled = false;

        if (m_onFinished != null)
        {
            m_onFinished();
        }
    }


    //=================================================================================
    //開始
    //=================================================================================

    /// <summary>
    /// 対象のオブジェクトのフェードを開始する
    /// </summary>
    public static void Begin(GameObject target, bool isFadeOut, float duration)
    {
        CanvasFader canvasFader = target.GetComponent<CanvasFader>();
        if (canvasFader == null)
        {
            canvasFader = target.AddComponent<CanvasFader>();
        }
        canvasFader.enabled = true;


        canvasFader.Play(isFadeOut, duration);
    }

    /// <summary>
    /// フェードを開始する
    /// </summary>
    public void Play(bool isFadeOut, float duration, bool ignoreTimeScale = true, Action onFinished = null)
    {
        this.enabled = true;

        m_ignoreTimeScale = ignoreTimeScale;
        m_onFinished = onFinished;

        Alpha = isFadeOut ? 1 : 0;
        m_fadeState = isFadeOut ? FadeState.FadeOut : FadeState.FadeIn;

        m_duration = duration;
    }

    //=================================================================================
    //停止
    //=================================================================================

    /// <summary>
    /// フェード停止
    /// </summary>
    public void Stop()
    {
        m_fadeState = FadeState.None;
        this.enabled = false;
    }
}
