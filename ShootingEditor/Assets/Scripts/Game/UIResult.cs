using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    // 게임 결과
    public class UIResult : UIWindow
    {
        private const string _resultClear = "Clear!";
        private const string _resultGameOver = "Game Over";

        [SerializeField]
        private Text _result;
        [SerializeField]
        private Text _songTitle;
        [SerializeField]
        private Text _score;
        [SerializeField]
        private BeatInfo _beatInfo;

        public override bool OnKeyInput()
        {
            if (Input.GetButtonDown(ButtonName.start) || Input.GetKeyDown(KeyCode.Escape))
            {
                OnRetryClicked();
            }
            return true;
        }

        public void SetData(BeatInfo beatInfo, bool cleared, int score)
        {
            _beatInfo = beatInfo;
            _result.text = cleared ? _resultClear : _resultGameOver;
            _songTitle.text = _beatInfo._title;
            _score.text = score.ToString();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 스테이지 재시작
        /// </summary>
        public void OnRetryClicked()
        {
            Close();
            if (GameSystem._Instance != null)
            {
                GameSystem._Instance.Retry();
            }
        }

        /// <summary>
        /// 음악 목록으로
        /// </summary>
        public void OnBeatListClicked()
        {
            Application.LoadLevel(SceneName.beatList);
        }
    }
}