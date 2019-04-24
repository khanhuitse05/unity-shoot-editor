using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class UIResult : MonoBehaviour
    {
        public Text _result;
        public Text _songTitle;
        public Text _score;
        public GameInfo _beatInfo;


        public void SetData(GameInfo beatInfo, bool cleared, int score)
        {
            _beatInfo = beatInfo;
            _result.text = cleared ? "Clear!" : "Game Over";
            _songTitle.text = _beatInfo._title;
            _score.text = score.ToString();
            Open();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnBeatListClicked()
        {
            SceneManager.LoadScene(SceneName.beatList);
        }
    }
}