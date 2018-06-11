using UnityEngine;

public static class SceneName
{
    public const string title = "Title";
    public const string beatList = "BeatList";
    public const string stage = "Stage";
}

public static class ButtonName
{
    public const string start = "Start";
}

public static class PlayName
{
    public const string black = "Common/Player_Black";
}
public static class PlayEffectName
{
    public const string black = "Common/Player_Crash";
}
public static class ShotName
{
    public const string black = "Common/Shot_Black";
}
public static class BossName
{
    public const string blue = "Common/Boss_Blue";
    public const string red = "Common/Boss_Red";
}
public static class BossEffectName
{
    public const string blue = "Common/Effect_BossCrashBlue";
    public const string red = "Common/Effect_BossCrashRed";
}
public static class BulletName
{
    public const string red = "Common/Bullet_Red";
    public const string redLarge = "Common/Bullet_RedLarge";
    public const string redSmall = "Common/Bullet_RedSmall";
    public const string blue = "Common/Bullet_Blue";
    public const string blueLarge = "Common/Bullet_BlueLarge";
    public const string blueSmall = "Common/Bullet_BlueSmall";
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
    public const string _uiLetterBoxBottom = "UI/LetterBoxBottom";
    public const string _uiLetterBoxLeft = "UI/LetterBoxLeft";
    public const string _uiLetterBoxRight = "UI/LetterBoxRight";
    public const string _uiStageText = "UI/UIStageText";
    public const string _uiHeaderPanel = "UI/UIHeaderPanel";

}