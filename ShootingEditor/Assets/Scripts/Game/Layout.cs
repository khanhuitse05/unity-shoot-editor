using UnityEngine;

namespace Game
{
    public class Layout : MonoBehaviour
    {
        public RectTransform _leftBox;
        public RectTransform _rightBox;
        public RectTransform _bottomBox;
        public RectTransform _gameArea;

        public void Initialize(float refDeviceWidth, float bottomBoxHeightScreenRate)
        {
            _bottomBox.anchorMax = new Vector2(0.5f, bottomBoxHeightScreenRate);
            UIUtil.SetWidth(_bottomBox, refDeviceWidth);

            _leftBox.offsetMax = new Vector2(-refDeviceWidth / 2.0f, 0.0f);
            _rightBox.offsetMin = new Vector2(refDeviceWidth / 2.0f, 0.0f);

            _gameArea.anchorMin = new Vector2(0.5f, bottomBoxHeightScreenRate);
            UIUtil.SetWidth(_gameArea, refDeviceWidth);
        }
    }
}