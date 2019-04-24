using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class MoveInputArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private RectTransform _rectTrans = null;
        private bool _touching = false;
        private int _pointerID;
        private Vector2 _lastPos;
        private Vector2 _curPos;

        private void Start()
        {
            _rectTrans = GetComponent<RectTransform>();
            _touching = false;
            _lastPos = Vector2.zero;
            _curPos = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (!_touching)
            {
                _touching = true;
                _pointerID = data.pointerId;

                Vector3 worldPos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTrans, data.position, data.pressEventCamera, out worldPos))
                {
                    _lastPos = worldPos;
                    _curPos = worldPos;
                }
                else
                {
                    _lastPos = Vector2.zero;
                    _curPos = Vector2.zero;
                }
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (_touching && data.pointerId == _pointerID)
            {
                Vector3 worldPos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTrans, data.position, data.pressEventCamera, out worldPos))
                {
                    _curPos = worldPos;
                }
                else
                {
                    _curPos = _lastPos;
                }
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (_touching && data.pointerId == _pointerID)
            {
                _touching = false;
            }
        }

        private void LateUpdate()
        {
            if (_touching)
            {
                _lastPos = _curPos;
            }
        }

        public Vector2 GetDelta()
        {
            if (_touching)
            {
                return (_curPos - _lastPos);
            }
            else
            {
                return Vector2.zero;
            }
        }

        public bool IsTouching()
        {
            return _touching;
        }
    }
}