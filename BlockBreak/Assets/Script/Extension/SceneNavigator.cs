

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// シーンの遷移を実行、管理するクラス
/// </summary>
public class SceneNavigator : SingletonMonoBehaviour<SceneNavigator>
{

    //フェード中か否か
    public bool IsFading
    {
        get { return m_fader.IsFading || m_fader.Alpha != 0; }
    }

    //一個前と現在、次のシーン名
    private string m_beforeSceneName = "";
    public string BeforeSceneName
    {
        get { return m_beforeSceneName; }
    }

    private string m_currentSceneName = "";
    public string CurrentSceneName
    {
        get { return m_currentSceneName; }
    }

    private string m_nextSceneName = "";
    public string NextSceneName
    {
        get { return m_nextSceneName; }
    }

    //フェード後のイベント
    public event Action FadeOutFinished = delegate { };
    public event Action FadeInFinished = delegate { };

    //フェード用クラス
    [SerializeField]
    private CanvasFader m_fader = null;

    //フェード時間
    public const float FADE_TIME = 0.5f;
    private float _fadeTime = FADE_TIME;

    //=================================================================================
    //初期化
    //=================================================================================

    /// <summary>
    /// 初期化(Awake時かその前の初アクセス時、どちらかの一度しか行われない)
    /// </summary>
    protected override void Init()
    {
        base.Init();

        //実機上やエディタを実行している時にはAddした場合はResetが実行されないので、Initから実行
        if (m_fader == null)
        {
            Reset();
        }

        //最初のシーン名設定
        m_currentSceneName = SceneManager.GetSceneAt(0).name;

        //永続化し、フェード用のキャンバスを非表示に
        DontDestroyOnLoad(gameObject);
        m_fader.gameObject.SetActive(false);
    }

    //コンポーネント追加時に自動で実行される(実機上やエディタを実行している時には動作しない)
    private void Reset()
    {
        //オブジェクトの名前を設定
        gameObject.name = "SceneNavigator";

        //フェード用のキャンバス作成
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        fadeCanvas.transform.SetParent(transform);
        fadeCanvas.SetActive(false);

        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        fadeCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        fadeCanvas.AddComponent<GraphicRaycaster>();
        m_fader = fadeCanvas.AddComponent<CanvasFader>();
        m_fader.Alpha = 0;

        //フェード用の画像作成
        GameObject imageObject = new GameObject("Image");
        imageObject.transform.SetParent(fadeCanvas.transform, false);
        imageObject.AddComponent<Image>().color = Color.black;
        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 2000);
    }

    //=================================================================================
    //シーンの変更
    //=================================================================================

    /// <summary>
    /// シーンの変更
    /// </summary>
    public void Change(string sceneName, float fadeTime = FADE_TIME)
    {
        if (IsFading)
        {
            Debug.Log("フェード中です！");
            return;
        }

        //次のシーン名とフェード時間を設定
        m_nextSceneName = sceneName;
        _fadeTime = fadeTime;

        //フェードアウト
        m_fader.gameObject.SetActive(true);
        m_fader.Play(isFadeOut: false, duration: _fadeTime, onFinished: OnFadeOutFinish);
    }

    //フェードアウト終了
    private void OnFadeOutFinish()
    {
        FadeOutFinished();

        //シーン読み込み、変更
        SceneManager.LoadScene(m_nextSceneName);

        //シーン名更新
        m_beforeSceneName = m_currentSceneName;
        m_currentSceneName = m_nextSceneName;

        //フェードイン開始
        m_fader.gameObject.SetActive(true);
        m_fader.Alpha = 1;
        m_fader.Play(isFadeOut: true, duration: _fadeTime, onFinished: OnFadeInFinish);
    }

    //フェードイン終了
    private void OnFadeInFinish()
    {
        m_fader.gameObject.SetActive(false);
        FadeInFinished();
    }

}