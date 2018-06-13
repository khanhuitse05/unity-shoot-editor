using UnityEngine;
using System.Collections;

namespace Game
{
    namespace IevanPolkka
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
                _Logic._coroutineManager.RegisterCoroutine(MoveMain());
            }

            public override void Move()
            {
                _Logic._coroutineManager.UpdateAllCoroutines();
            }

            // 메인 코루틴
            private IEnumerator MoveMain()
            {
                // 등장
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 30));
                yield return new WaitForFrames(30);

                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormBorder(0, 0.02f, 30, 2));
                yield return new WaitForFrames(60);
                //
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormBorder(1, 0.02f, 30, 2));
                yield return new WaitForFrames(60);
                // 
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(-0.8f, 0.75f), 120));
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormBorder(2, 0.02f, 30, 2));
                yield return new WaitForFrames(120);
                // 
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.8f, 0.75f), 240));
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormBorder(3, 0.02f, 30, 2));
                yield return new WaitForFrames(120);
                //
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormCorner(0.02f, 30, 2));
                yield return new WaitForFrames(120);
                //
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 120));
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormCorner(0.02f, 30, 2));
                yield return new WaitForFrames(120);
                //
                yield return new WaitForFrames(120);
                {
                    // 마마마 린간 덴간 린간 덴간
                    const float pivotX = 0.0f;
                    const float pivotY = 0.75f;
                    const float pivotOffset = 0.1f;
                    // 마마마 린간 덴간
                    _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(pivotX + pivotOffset, pivotY), 30));
                    yield return new WaitForFrames(75);
                    // 린간 덴간
                    _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(pivotX - pivotOffset, pivotY), 30));
                    yield return new WaitForFrames(75);
                    // 린간
                    _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(pivotX + pivotOffset, pivotY), 15));
                    yield return new WaitForFrames(30);
                    // 린간
                    _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(pivotX, pivotY), 15));
                    yield return new WaitForFrames(30);

                    const float shakeOffset = 0.03f;
                    const int shakeInterval = 2;
                    for (int i = 0; i < 8; ++i)
                    {
                        _pos = new Vector2(pivotX + shakeOffset, pivotY);
                        yield return new WaitForFrames(shakeInterval);
                        _pos = new Vector2(pivotX, pivotY + shakeOffset);
                        yield return new WaitForFrames(shakeInterval);
                        _pos = new Vector2(pivotX - shakeOffset, pivotY);
                        yield return new WaitForFrames(shakeInterval);
                        _pos = new Vector2(pivotX, pivotY - shakeOffset);
                        yield return new WaitForFrames(shakeInterval);
                    }
                    _pos = new Vector2(pivotX, pivotY);
                }

                // 아야챠챠
                yield return new WaitForAbsFrames(600); //5250
                _Logic._coroutineManager.StartCoroutine(_Logic.CircleBullets(this, BulletName.blue, 0.25f, 0.01f, 20, true, 60, 8));
                yield return new WaitForFrames(30);
                _Logic._coroutineManager.StartCoroutine(_Logic.CircleBullets(this, BulletName.red, 0.25f, 0.02f, 20, false, 60, 8));

                // 야바린간
                yield return new WaitForAbsFrames(1080);
                _Logic._coroutineManager.StartCoroutine(_Logic.CircleBullets(this, BulletName.blue, 0.25f, 0.01f, 20, true, 60, 8));
                _Logic._coroutineManager.StartCoroutine(_Logic.ShotFormCorner(0.02f, 60, 2));

                // 아야챠챠
                yield return new WaitForAbsFrames(1590);
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, Vector2.zero, 120, 0.05f));
                yield return new WaitForFrames(120);
                _Logic._coroutineManager.StartCoroutine(_Logic.CustomGapBullets(this, BulletName.blue, 0.95f, 0.005f, 100
                    , 120, new float[] { 0.36f, 0.20f, 0.95f, 0.25f, 0.5f, 0.36f, 0.75f }));

                // 마지막 간주
                yield return new WaitForAbsFrames(2550);
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.75f), 60));
                ///
                yield return new WaitForAbsFrames(2650);
                _Logic._coroutineManager.StartCoroutine(_Logic.Simple3Wave(this, true));

                yield return new WaitForAbsFrames(2690);
                _Logic._coroutineManager.StartCoroutine(_Logic.Simple3Wave(this, false));

                yield return new WaitForAbsFrames(2800);
                _Logic._coroutineManager.StartCoroutine(_Logic.Simple4Wave());
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveConstantVelocity(this, new Vector2(0.0f, 0.5f), 75));

                yield return new WaitForAbsFrames(2900);
                _Logic._coroutineManager.StartCoroutine(_Logic.SimpleCircles(this));

                yield return new WaitForAbsFrames(3020);
                _Logic._coroutineManager.StartCoroutine(_Logic.AimAfterSimpleCircles(this));

                yield return new WaitForAbsFrames(3350);
                // 왼쪽 코너로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(-0.6f, 0.75f), 30, 0.1f));

                yield return new WaitForAbsFrames(3440);
                _Logic._coroutineManager.StartCoroutine(_Logic.CornerWaves(this, true));

                yield return new WaitForAbsFrames(3780);
                // 오른쪽 코너로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(0.6f, 0.75f), 60, 0.1f));

                yield return new WaitForAbsFrames(3870);
                _Logic._coroutineManager.StartCoroutine(_Logic.CornerWaves(this, false));

                yield return new WaitForAbsFrames(4210);
                // 중앙으로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(0.0f, 0.0f), 40, 0.1f));

                yield return new WaitForAbsFrames(4300);
                _Logic._coroutineManager.StartCoroutine(_Logic.RotateCrossTwice1(this));

                yield return new WaitForAbsFrames(5130);
                _Logic._coroutineManager.StartCoroutine(_Logic.BackwardStep(this));

                yield return new WaitForAbsFrames(5575);
                _Logic._coroutineManager.StartCoroutine(_Logic.PigeonSolo(this));

                yield return new WaitForAbsFrames(7250);
                // 왼쪽 코너로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(-0.6f, 0.75f), 30, 0.1f));

                yield return new WaitForAbsFrames(7280);
                _Logic._coroutineManager.StartCoroutine(_Logic.CornerWaves(this, true));

                yield return new WaitForAbsFrames(7620);
                // 오른쪽 코너로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(0.6f, 0.75f), 60, 0.1f));

                yield return new WaitForAbsFrames(7710);
                _Logic._coroutineManager.StartCoroutine(_Logic.CornerWaves(this, false));

                yield return new WaitForAbsFrames(8050);
                // 중앙으로 이동
                _Logic._coroutineManager.StartCoroutine(_Logic.MoveDamp(this, new Vector2(0.0f, 0.0f), 40, 0.1f));

                yield return new WaitForAbsFrames(8140);
                _Logic._coroutineManager.StartCoroutine(_Logic.RotateCrossTwice2(this));
                ///

                yield return new WaitForAbsFrames(8960);
                _Logic.CircleBullet(this, BulletName.red, 0.25f, 0.025f, 20, false);
                {
                    Effect crashEffect = GameSystem._Instance.CreateEffect<Effect>();
                    crashEffect.Init(BossEffectName.blue, _X, _Y, 0.0f);
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
    } // IevanPolkka
} // Game