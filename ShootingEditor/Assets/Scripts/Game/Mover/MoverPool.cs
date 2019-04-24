using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game
{
    // Mover 풀 모음
    public class MoverPoolManager
    {
        private Dictionary<Type, Stack<Mover>> _pools = new Dictionary<Type, Stack<Mover>>();

        /// <summary>
        /// </summary>
        private Stack<Mover> GetOrCreatePool<T>() where T : Mover, new()
        {
            Stack<Mover> pool = null;
            if (!_pools.TryGetValue(typeof(T), out pool))
            {
                // 새 풀 생성
                pool = new Stack<Mover>();
                _pools.Add(typeof(T), pool);
            }
            return pool;
        }

        /// <summary>
        /// </summary>
        private T CreateInstance<T>(Stack<Mover> pool) where T : Mover, new()
        {
            T instance = new T();
            instance.OnFirstCreatedInPool(typeof(T));
            return instance;
        }

        /// <summary>
        /// </summary>
        public void PoolStack<T>(int count) where T : Mover, new()
        {
            Stack<Mover> pool = GetOrCreatePool<T>();
            count -= pool.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    T instance = CreateInstance<T>(pool);
                    pool.Push(instance);
                }
            }
        }

        /// <summary>
        /// </summary>
        public T Create<T>() where T : Mover, new()
        {
            Stack<Mover> pool = GetOrCreatePool<T>();

            T instance = null;
            if (pool.Count > 0)
            {
                instance = pool.Pop() as T;
            }
            else
            {
                instance = CreateInstance<T>(pool);
            }

            return instance;
        }

        /// <summary>
        /// </summary>
        public void Delete(Mover instance)
        {
            if (instance == null)
            {
                return;
            }

            Stack<Mover> pool = null;
            if (_pools.TryGetValue(instance._poolKey, out pool))
            {
                pool.Push(instance);
            }
            else
            {
                instance = null;
            }
        }

        /// <summary>
        /// </summary>
        public void Clear(Type type)
        {
            Stack<Mover> pool = null;
            if (_pools.TryGetValue(type, out pool))
            {
                pool.Clear();
                _pools.Remove(type);
            }
        }

        public int GetCount(Type type)
        {
            Stack<Mover> pool = null;
            if (_pools.TryGetValue(type, out pool))
            {
                return pool.Count;
            }
            else
            {
                return 0;
            }
        }
    }
}
