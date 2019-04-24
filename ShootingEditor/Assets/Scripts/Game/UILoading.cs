using UnityEngine.UI;
using UnityEngine;

namespace Game
{
    public class UILoading : MonoBehaviour
    {
        public Text _progress; 

        public void SetProgress(string text)
        {
            _progress.text = text;
        }

        public void Open()
        {
            _progress.text = string.Empty;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}