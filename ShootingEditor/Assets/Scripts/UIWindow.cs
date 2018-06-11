using UnityEngine;

/// <summary>
/// </summary>
public class UIWindow : MonoBehaviour
{
    private UISystem _uiSystem;
    protected UISystem _UISystem { get { if (_uiSystem == null) { _uiSystem = FindObjectOfType<UISystem>(); } return _uiSystem; } }

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }

    private void OnEnable()
    {
        if (_UISystem != null)
        {
            _UISystem.OnWindowEnabled(this);
        }
    }

    private void OnDisable()
    {
        if (_UISystem != null)
        {
            _UISystem.OnWindowDisabled(this);
        }
    }

    public int GetSiblingIndex()
    {
        return transform.GetSiblingIndex();
    }

    /// <summary>
    /// 키입력 처리. 상위 윈도우부터 호출함
    /// </summary>
    /// <returns>true: 하위 윈도우까지 키입력 처리를 전달하지 않는다.</returns>
    public virtual bool OnKeyInput()
    {
        return false;
    }

    /// <summary>
    /// Sibling Index 로 윈도우간 비교
    /// <para>Index 클수록 큼</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int CompareBySiblingIndex(UIWindow a, UIWindow b)
    {
        if (a == null)
        {
            if (b == null)
            {
                return 0;
            }
            else
            {
                return -1; // a < b
            }
        }
        else
        {
            if (b == null)
            {
                return 1; // a > b
            }
            else
            {
                return a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
            }
        }
    }

    /// <summary>
    /// CompareBySiblingIndex() 의 반대
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static int CompareBySiblingIndexReverse(UIWindow a, UIWindow b)
    {
        return CompareBySiblingIndex(a, b) * -1;
    }

    protected void AddHeaderPanel(string title, UnityEngine.Events.UnityAction onBackClicked)
    {
        Object prefab = Resources.Load(Define._uiHeaderPanel);
        GameObject obj = Instantiate(prefab) as GameObject;
        UIHeaderPanel header = obj.GetComponent<UIHeaderPanel>();
        header._Trans.SetParent(transform, false);
        header._RectTrans.localScale = Vector3.one;
        header.Initialize(title, onBackClicked);
    }
}
