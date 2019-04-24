using UnityEngine;
using UnityEngine.UI;

// 노래 목록의 한 요소
public class UIBeatListItem : MonoBehaviour
{
    public Text _title;
    public Text _length;
    private int _index; // 목록 내 인덱스
    private UIBeatList.InfoSelectedHandler _selectedHandler;

    public void SetBeatInfo(int index_, UIBeatList.InfoSelectedHandler selectedHandler_, GameInfo beatInfo)
    {
        _index = index_;
        _selectedHandler = selectedHandler_;

        _title.text = beatInfo._title;
        _length.text = UIUtil.ConverBeatLength(beatInfo._length);
    }

    public void OnClicked()
    {
        if (_selectedHandler != null)
        {
            _selectedHandler(_index);
        }
    }
}
