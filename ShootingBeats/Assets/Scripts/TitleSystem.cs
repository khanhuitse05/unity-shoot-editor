using UnityEngine;
using UnityEngine.UI;

public class TitleSystem : SceneSystem
{
    [SerializeField]
    private Button _btnStart;

    protected override void OnAwake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Define.SetFPS();
        if (GlobalSystem._Instance == null)
        {
            GlobalSystem.CreateInstance();
        }
    }

    protected override void OnUpdate()
    {
        if (_HasKeyInputFocus)
        {
            if (Input.GetButtonDown(ButtonName._start))
            {
                OnStartClicked();
            }
        }
    }

    /// <summary>
    /// </summary>
    private void LoadBeatListScene()
    {
        Application.LoadLevel(SceneName._BeatList);
    }

    public void OnStartClicked()
    {
        LoadBeatListScene();
    }

    public void OnOptionClicked()
    {
        _UISystem.OpenWindow(Define._uiOptionPath);
    }
}
