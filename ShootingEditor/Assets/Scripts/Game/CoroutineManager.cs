/*
 * http://wiki.unity3d.com/index.php/CoroutineScheduler
 */

using System.Collections;
using UnityEngine;

namespace Game
{
    public class CoroutineNode
    {
        public CoroutineNode _listPrev = null; 
        public CoroutineNode _listNext = null;
        public IEnumerator _fiber; 
        public bool finished = false; 
        public int waitForFrame = -1; 
        public CoroutineNode waitForCoroutine;

        public CoroutineNode(IEnumerator fiber_)
        {
            this._fiber = fiber_;
        }
    }

    #region YieldCommand
    public abstract class YieldCommand
    {
    }

    public class WaitForFrames : YieldCommand
    {
        public int _frames;

        public WaitForFrames(int frames_)
        {
            _frames = frames_;
        }
    }

    public class WaitForAbsFrames : YieldCommand
    {
        public int _frames;

        public WaitForAbsFrames(int frames_)
        {
            _frames = frames_;
        }
    }
    #endregion //YieldCommand

    public class CoroutineManager
    {
        public static CoroutineManager instance = new CoroutineManager();
        public CoroutineManager()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private CoroutineNode _listFirst = null;
        private int _currentFrame = 0;
        public void ResetFrame()
        {
            _currentFrame = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="fiber"></param>
        public CoroutineNode StartCoroutine(IEnumerator fiber)
        {
            if (fiber == null)
            {
                return null;
            }

            CoroutineNode coroutine = new CoroutineNode(fiber);
            AddCoroutine(coroutine);
            UpdateCoroutine(coroutine);
            return coroutine;
        }

        /// <summary>
        /// </summary>
        public CoroutineNode RegisterCoroutine(IEnumerator fiber)
        {
            if (fiber == null)
            {
                return null;
            }

            CoroutineNode coroutine = new CoroutineNode(fiber);
            AddCoroutine(coroutine);
            return coroutine;
        }

        /// <summary>
        /// </summary>
        public void StopAllCoroutines()
        {
            _listFirst = null;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool HasCoroutines()
        {
            return _listFirst != null;
        }

        /// <summary>
        /// </summary>
        public void UpdateAllCoroutines()
        {
            CoroutineNode coroutine = this._listFirst;
            _currentFrame++;
            while (coroutine != null)
            {
                CoroutineNode listNext = coroutine._listNext;

                if (coroutine.waitForFrame >= 0)
                {
                    if (_currentFrame >= coroutine.waitForFrame)
                    {
                        coroutine.waitForFrame = -1;
                        UpdateCoroutine(coroutine);
                    }
                }
                else if (coroutine.waitForCoroutine != null)
                {
                    if (coroutine.waitForCoroutine.finished)
                    {
                        coroutine.waitForCoroutine = null;
                        UpdateCoroutine(coroutine);
                    }
                }
                else
                {
                    UpdateCoroutine(coroutine);
                }
                coroutine = listNext;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="coroutine"></param>
        private void UpdateCoroutine(CoroutineNode coroutine)
        {
            IEnumerator fiber = coroutine._fiber;
            if (coroutine._fiber.MoveNext())
            {
                object yieldCommand = fiber.Current;

                if (yieldCommand == null)
                {
                    coroutine.waitForFrame = _currentFrame + 1;
                }
                else if (yieldCommand is WaitForFrames)
                {
                    WaitForFrames cmd = yieldCommand as WaitForFrames;
                    coroutine.waitForFrame = _currentFrame + cmd._frames;
                }
                else if (yieldCommand is WaitForAbsFrames)
                {
                    WaitForAbsFrames cmd = yieldCommand as WaitForAbsFrames;
                    coroutine.waitForFrame = cmd._frames;
                }
                else if (yieldCommand is CoroutineNode)
                {
                    coroutine.waitForCoroutine = yieldCommand as CoroutineNode;
                }
                else
                {
                    throw new System.ArgumentException("[CoroutineManager] Unexpected coroutine yield type: " + yieldCommand.GetType());
                }
            }
            else
            {
                coroutine.finished = true;
                RemoveCoroutine(coroutine);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="coroutine"></param>
        private void AddCoroutine(CoroutineNode coroutine)
        {
            if (_listFirst != null)
            {
                coroutine._listNext = _listFirst;
                _listFirst._listPrev = coroutine;
            }
            _listFirst = coroutine;
        }

        /// <summary>
        /// </summary>
        /// <param name="coroutine"></param>
        private void RemoveCoroutine(CoroutineNode coroutine)
        {
            if (this._listFirst == coroutine)
            {
                this._listFirst = coroutine._listNext;
            }
            else
            {
                if (coroutine._listNext != null)
                {
                    coroutine._listPrev._listNext = coroutine._listNext;
                    coroutine._listNext._listPrev = coroutine._listPrev;
                }
                else if (coroutine._listPrev != null)
                {
                    coroutine._listPrev._listNext = null;
                }
            }

            coroutine._listPrev = null;
            coroutine._listNext = null;
        }
    } // CoroutineManager
}
