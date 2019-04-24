using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
    namespace Demo
    {
        // 보스
        public class Boss : Enemy
        {
            private const int _score = 10;
            public Boss() : base()
            {
            }

            public override void Init(string shapeSubPath, float x, float y, float angle)
            {
                base.Init(shapeSubPath, x, y, angle);
                CoroutineManager.instance.RegisterCoroutine(MoveMain());
                CoroutineManager.instance.ResetFrame();
            }

            public override void Move()
            {
                CoroutineManager.instance.UpdateAllCoroutines();
            }
            void Log(string message)
            {
                GameSystem.SetNote(message);
            }
            // 메인 코루틴
            private IEnumerator MoveMain()
            {
                StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 30));
                yield return new WaitForFrames(100);

                while (true)
                {
                    if (stateStack.Count > 0)
                    {
                        IShot shot = stateStack.Dequeue();
                        Log(shot.ToString());
                        yield return StartCoroutine(shot.Shot(this));
                    }
                    yield return new WaitForFrames(30);
                }
            }

            Queue<IShot> stateStack = new Queue<IShot>();
            ShotFactory shotFactory = new ShotFactory();
            public void AddShotBuilder(ShotName name)
            {
                IShot shot = shotFactory.CreateShot(name);
                if (shot != null)
                {
                    stateStack.Enqueue(shot);
                }
            }
            public void AddShotBuilder(List<ShotName> names)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    AddShotBuilder(names[i]);
                }
            }
            // 피격시
            public override void OnHit()
            {
                GameSystem._Instance.SetScoreDelta(_score);
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                CoroutineManager.instance.StopAllCoroutines();
            }

            private GameLogic _Logic
            {
                get { return GameSystem._Instance.GetLogic<GameLogic>(); }
            }

            CoroutineNode StartCoroutine(IEnumerator fiber)
            {
                return CoroutineManager.instance.StartCoroutine(fiber);
            }
        } // Boss
    } // IevanPolkka
} // Game