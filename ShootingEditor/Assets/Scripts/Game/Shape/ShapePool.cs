using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class ShapePoolManager
    {
        // key: subPath
        private Dictionary<string, Stack<Shape>> _pools = new Dictionary<string, Stack<Shape>>();

        /// <summary>
        /// </summary>
        private Stack<Shape> GetOrCreatePool(string subPath)
        {
            Stack<Shape> pool = null;
            if (!_pools.TryGetValue(subPath, out pool))
            {
                pool = new Stack<Shape>();
                _pools.Add(subPath, pool);
            }
            return pool;
        }

        /// <summary>
        /// </summary>
        private Shape CreateInstance(string subPath, Stack<Shape> pool)
        {
            string prefabPath = Shape._prefabRoot + "/" + subPath;
            Object prefab = Resources.Load(prefabPath);
            if (prefab != null)
            {
                GameObject obj = (Object.Instantiate(prefab) as GameObject);
                obj.name = subPath;
                Shape instance = obj.GetComponent<Shape>();
                instance.OnFirstCreatedInPool(subPath);
                return instance;
            }
            else
            {
                Debug.LogError("[ShapePoolManager] invalid path:" + subPath);
                return null;
            }
        }

        /// <summary>
        /// 인스턴스를 생성해 풀에 쌓아둔다.
        /// <para>이미 풀에 쌓여있는 수는 제외</para>
        /// </summary>
        public void PoolStack(string subPath, int count)
        {
            Stack<Shape> pool = GetOrCreatePool(subPath);
            count -= pool.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    Shape instance = CreateInstance(subPath, pool);
                    pool.Push(instance);
                }
            }
        }

        /// <summary>
        /// </summary>
        public Shape Create(string subPath)
        {
            Stack<Shape> pool = GetOrCreatePool(subPath);

            Shape shape = null;
            if (pool.Count > 0)
            {
                shape = pool.Pop();
            }
            else
            {
                shape = CreateInstance(subPath, pool);
            }

            // 생성 전 항상 지정할 정보
            if (shape != null)
            {
                shape.OnBeforeCreatedFromPool();
            }
            else
            {
                Debug.LogError("[ShapePoolManager] invalid path:" + subPath);
            }
            return shape;
        }

        /// <summary>
        /// </summary>
        public void Delete(Shape instance)
        {
            if (instance == null)
            {
                return;
            }

            Stack<Shape> pool = null;
            if (_pools.TryGetValue(instance._poolKey, out pool))
            {
                pool.Push(instance);
                instance.OnAfterDestroyedToPool();
            }
            else
            {
                Object.Destroy(instance);
            }
        }

        /// <summary>
        /// subPath
        /// </summary>
        public void Clear(string subPath)
        {
            Stack<Shape> pool = null;
            if (_pools.TryGetValue(subPath, out pool))
            {
                while(pool.Count > 0)
                {
                    Shape shape = pool.Pop();
                    Object.Destroy(shape);
                }
                _pools.Remove(subPath);
            }
        }

        public int GetCount(string subPath)
        {
            Stack<Shape> pool = null;
            if (_pools.TryGetValue(subPath, out pool))
            {
                return pool.Count;
            }
            else
            {
                return 0;
            }
        }

    } // class ShapePoolManager
}
