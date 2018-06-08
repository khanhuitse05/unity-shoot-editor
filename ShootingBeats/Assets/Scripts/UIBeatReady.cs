using UnityEngine;
using UnityEngine.UI;

public class UIBeatReady : UIWindow
{
    [SerializeField]
    private Text _title;
    [SerializeField]
    private UIDifficultyIcon _difficulty;
    [SerializeField]
    private Text _length;
    [SerializeField]
    private BeatInfo _beatInfo;
    private const string _uiTitle = "Beat Ready";

    protected override void OnAwake()
    {
        base.OnAwake();
        AddHeaderPanel(_uiTitle, OnBackClicked);
    }

    public void Open(BeatInfo beatInfo)
    {
        // 정보 채우기
        _beatInfo = beatInfo;
        _title.text = _beatInfo._title;
        _difficulty.SetDifficulty(beatInfo._difficulty);
        _length.text = Define.ConverBeatLength(_beatInfo._length);

        // 활성화
        gameObject.SetActive(true);
    }

    public override bool OnKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
        return true;
    }

    public void OnBackClicked()
    {
        Close();
    }

    /// <summary>
    /// 창 비활성화
    /// </summary>
    private void Close()
    {
        gameObject.SetActive(false);
        _beatInfo = null;
    }

    // 게임 씬으로 이동
    public void OnPlayClicked()
    {
        if (GlobalSystem._Instance == null)
        {
            GlobalSystem.CreateInstance();
        }
        GlobalSystem._Instance._LoadingBeatInfo = _beatInfo;
        Application.LoadLevel(SceneName._Stage);
    }
}
