using UnityEngine;
using System.Collections;

namespace Game
{
    namespace TurkishMarch
    {
        // 보스
        public class Boss : Enemy
        {
            private int _score = 10; 
            public Boss() : base()
            {
            }

            public override void Init(string shapeSubPath, float x, float y, float angle)
            {
                base.Init(shapeSubPath, x, y, angle);
                _Logic._coroutineManager.RegisterCoroutine(MoveMain());
            }

            public override void Move()
            {
                _Logic._coroutineManager.UpdateAllCoroutines();
            }

            // 메인 코루틴
            private IEnumerator MoveMain()
            {
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 40));
                
                yield return new WaitForAbsFrames(90);
                _Logic._coroutineManager.StartCoroutine(_Logic.Pattern_A_11(this));
                yield return new WaitForAbsFrames(511);
                _Logic._coroutineManager.StartCoroutine(_Logic.Pattern_A_11(this));
                yield return new WaitForAbsFrames(935);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternB(this));
                yield return new WaitForAbsFrames(1370);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternA_22(this));
                yield return new WaitForAbsFrames(1800);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternB(this));
                yield return new WaitForAbsFrames(2235);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternA_22(this));
                yield return new WaitForAbsFrames(2670);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternC_1(this));
                yield return new WaitForAbsFrames(3590);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternD_1(this));
                yield return new WaitForAbsFrames(5270);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternD_2(this));
                yield return new WaitForAbsFrames(6170);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternC_2(this));
                
                yield return new WaitForAbsFrames(7100);
                // 패턴E 빠져나올 수 있을만큼 위로
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.94f), 60));
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternE_1());
                
                // 시간 때우기
                yield return new WaitForAbsFrames(7880);
                _Logic._coroutineManager.StartCoroutine(_Logic.AimingLineBullets(this, BulletName.red, 0.02f, 5, 5));
                yield return new WaitForFrames(70);
                _Logic._coroutineManager.StartCoroutine(_Logic.Aiming2LineBullets(this, BulletName.red, 0.02f, 0.1f, 5, 5));
                yield return new WaitForFrames(50);
                _Logic._coroutineManager.StartCoroutine(_Logic.AimingLineBullets(this, BulletName.red, 0.02f, 5, 9));
                yield return new WaitForFrames(110);
                _Logic._coroutineManager.StartCoroutine(_Logic.RandomAngleCircleBullets(this, BulletName.red, 0.02f, 12, 15, 5));

                yield return new WaitForAbsFrames(8250);
                _Logic._coroutineManager.StartCoroutine(_Logic.PatternE_2());

                // 폭발
                yield return new WaitForAbsFrames(8870);
                {
                    Effect crashEffect = GameSystem._Instance.CreateEffect<Effect>();
                    crashEffect.Init(BossEffectName.red, _X, _Y, 0.0f);
                }
                _alive = false;
            }

            // 피격시
            public override void OnHit()
            {
                GameSystem._Instance.SetScoreDelta(_score);
            }

            public override void OnDestroy()
            {
                base.OnDestroy();
                _Logic._coroutineManager.StopAllCoroutines();
            }
           
            private GameLogic _Logic
            {
                get { return GameSystem._Instance.GetLogic<GameLogic>(); }
            }
        } // Boss
    } // TurkishMarch
} // Game