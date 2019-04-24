using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game
{
    public abstract class Mover
    {
        public Shape _shape;
        public Vector2 _pos;
        public float _X
        {
            get { return _pos.x; }
            set { _pos.x = value; }
        }
        public float _Y
        {
            get { return _pos.y; }
            set { _pos.y = value; }
        }
        public float _angle; //[0.0f~1.0f]
        public bool _alive;
        public Type _poolKey { get; private set; } // 이 인스턴스가 되돌아갈 풀의 구분자

        public Mover()
        {
        }

        /// <summary>
        /// </summary>
        public void OnFirstCreatedInPool(Type poolKey_)
        {
            _poolKey = poolKey_;
        }

        /// <summary>
        /// </summary>
        public virtual void Init(string shapeSubPath, float x, float y, float angle)
        {
            SetShape(shapeSubPath);
            _X = x;
            _Y = y;
            _angle = angle;
            _alive = true;

            _shape.SetPosition(_pos, _angle);
        }

        /// <summary>
        /// </summary>
        private void ClearShape()
        {
            if (_shape != null)
            {
                GameSystem._Instance.DeleteShape(_shape);
                _shape = null;
            }
        }

        private void SetShape(string shapeSubPath)
        {
            ClearShape();
            _shape = GameSystem._Instance.CreateShape(shapeSubPath);
        }
        
        // 이동
        public abstract void Move();

        // 그리기
        public void Draw(int order)
        {
            _shape.SetPosition(_pos, _angle);
            _shape.SetSortingOrder(order);
        }

        public bool IsHit(Mover mover)
        {
            // 대상 또는 자신의 hit 영역이 없다면 Hit 발생하지 않음
            if (_shape._hit > 0.0f && mover._shape._hit > 0.0f)
            {
                float dx = mover._X - _X;
                float dy = mover._Y - _Y;
                float hit = mover._shape._hit + _shape._hit;
                return (dx * dx + dy * dy) < (hit * hit);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        public T IsHit<T>(List<T> movers) where T : Mover
        {
            foreach (T mover in movers)
            {
                if (IsHit(mover))
                {
                    return mover;
                }
            }
            return null;
        }

        public bool IsInStage()
        {
            if ((_X + _shape._size) <= GameSystem._Instance._MinX
                || (_X - _shape._size) >= GameSystem._Instance._MaxX
                || (_Y + _shape._size) <= GameSystem._Instance._MinY
                || (_Y - _shape._size) >= GameSystem._Instance._MaxY)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual void OnDestroy()
        {
            ClearShape();
        }
    }
}