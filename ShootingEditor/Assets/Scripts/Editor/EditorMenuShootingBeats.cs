using UnityEngine;
using UnityEditor;

public class EditorMenuShootingBeats
{
    [MenuItem("ShootingEditor/Create Round")]
    private static void CreateBeatInfo()
    {
        string path = "Assets/Resources/" + GameInfo._resourcePath + "/NewRound.asset";
        GameInfo info = ScriptableObject.CreateInstance<GameInfo>();
        AssetDatabase.CreateAsset(info, path);
        Debug.Log("GameInfo is created in " + path);
    }
}