using UnityEngine;

public static class SceneName
{
    public const string _Title = "Title";
    public const string _BeatList = "BeatList";
    public const string _Stage = "Stage";
}

public static class ButtonName
{
    public const string _start = "Start";
}

public enum Difficulty
{
    Normal, Hard, Extreme,
}

public static class Define
{
    public const int _fps = 60;
    public static void SetFPS()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _fps;
    }

    // 초단위 길이를 문자열로 변경
    public static string ConverBeatLength(int sec)
    {
        int lengthMin = sec / 60; // 분
        int lengthSec = sec % 60; // 초
        return (lengthMin.ToString() + ":" + lengthSec.ToString("00"));
    }

    // UI 프리팹 리소스 경로
    public const string _uiMessageBoxPath = "UI/UIMessageBox";
    public const string _uiOptionPath = "UI/UIOption";
    public const string _uiAboutPath = "UI/UIAbout";
    public const string _uiLetterBoxBottom = "UI/LetterBoxBottom";
    public const string _uiLetterBoxLeft = "UI/LetterBoxLeft";
    public const string _uiLetterBoxRight = "UI/LetterBoxRight";
    public const string _uiStageText = "UI/UIStageText";
    public const string _uiHeaderPanel = "UI/UIHeaderPanel";

}