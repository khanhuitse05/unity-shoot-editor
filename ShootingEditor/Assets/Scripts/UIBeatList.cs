using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBeatList : MonoBehaviour
{
    public RectTransform _transContents;
    public GameObject prefabItem;
    private GameInfo[] _beatInfos;
    public delegate void InfoSelectedHandler(int index); 

    void Awake()
    {
        BuildList();
    }

    private void BuildList()
    {
        _beatInfos = Resources.LoadAll<GameInfo>(GameInfo._resourcePath);
        if (_beatInfos != null)
        {
            float y = -15.0f;
            for (int i = 0; i < _beatInfos.Length; ++i)
            {
                GameInfo beatInfo = _beatInfos[i];
                GameObject objItem = Instantiate(prefabItem) as GameObject;
                objItem.name = prefabItem.name + "_" + beatInfo.name;
                objItem.transform.SetParent(_transContents.transform, false);
                
                RectTransform trans = objItem.GetComponent<RectTransform>();
                trans.localScale = Vector3.one;
                trans.localPosition = new Vector3(
                    trans.localPosition.x, y - ((1.0f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
                y -= (trans.rect.height + 1);

                // 정보 지정
                UIBeatListItem item = objItem.GetComponent<UIBeatListItem>();
                item.SetBeatInfo(i, OnInfoSelected, beatInfo);
            }

            // 목록 전체 높이 재지정
            {
                Vector2 newSize = new Vector2(_transContents.rect.size.x, Mathf.Abs(y));
                Vector2 oldSize = _transContents.rect.size;
                Vector2 deltaSize = newSize - oldSize;
                _transContents.offsetMin = _transContents.offsetMin - new Vector2(deltaSize.x * _transContents.pivot.x, deltaSize.y * _transContents.pivot.y);
                _transContents.offsetMax = _transContents.offsetMax + new Vector2(deltaSize.x * (1.0f - _transContents.pivot.x), deltaSize.y * (1.0f - _transContents.pivot.y));
            }

        } // if (beatInfos != null)
    }

    private void OnInfoSelected(int index)
    {
        Game.GameSystem._loadGameInfo = _beatInfos[index];
        if (_beatInfos[index].name == "Demo")
        {
            SceneManager.LoadScene("Demo");
        }
        else
        {
            SceneManager.LoadScene(SceneName.stage);
        }
    }
}
