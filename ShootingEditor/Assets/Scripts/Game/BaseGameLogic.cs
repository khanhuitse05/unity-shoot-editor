using UnityEngine;
using System.Collections;

namespace Game
{
    public abstract partial class BaseGameLogic
    {
        public abstract IEnumerator LoadContext();

        public abstract void UpdatePlayContext();

        protected IEnumerator LoadBasicPlayer()
        {
            GameSystem._Instance._uiLoading.SetProgress("Loading Shots");
            yield return null;
            GameSystem._Instance.PoolStackShape(PlayName.obj_shot, 30);
            yield return null;
            GameSystem._Instance.PoolStackShape(BulletName.blue, 30);
            yield return null;
            GameSystem._Instance.PoolStackShape(BulletName.red, 30);
            yield return null;
            GameSystem._Instance.PoolStackMover<Bullet>(50);
            yield return null;
            GameSystem._Instance.PoolStackMover<TeamShot>(36);
        }

        CoroutineNode StartCoroutine(IEnumerator fiber)
        {
            return CoroutineManager.instance.StartCoroutine(fiber);
        }
        #region Util
        // Get Angle between 2 point
        public float GetAngle(float x, float y, float targetX, float targetY)
        {
            return Mathf.Atan2(targetY - y, targetX - x) / Mathf.PI / 2.0f;
        }
        // Angle with player
        public float GetPlayerAngle(float x, float y)
        {
            Vector2 target = GameSystem._Instance.player._pos;
            return GetAngle(x, y, target.x, target.y);
        }
        public float GetPlayerAngle(Mover startMover)
        {
            return GetPlayerAngle(startMover._X, startMover._Y);
        }
        #endregion Util

        #region Move
        /// <summary>
        /// Move to pos
        /// </summary>
        public IEnumerator MoveConstantVelocity(Mover mover, Vector2 arrivePos, int duration)
        {
            Vector2 delta = (arrivePos - mover._pos) / (float)duration;
            for (int i = 0; i < duration - 1; ++i)
            {
                mover._pos += delta;
                yield return null;
            }
            // Finish
            mover._pos = arrivePos;
        }

        /// <summary>
        /// Move to pos with acceleration
        /// </summary>
        public IEnumerator MoveDamp(Mover mover, Vector2 arrivePos, int duration, float damp)
        {
            for (int i = 0; i < duration - 1; ++i)
            {
                mover._pos = Vector2.Lerp(mover._pos, arrivePos, damp);
                yield return null;
            }

            // Finish
            mover._pos = arrivePos;
        }
        #endregion

        #region Base
        /// <summary>
        /// Shot form 
        /// </summary>
        /// <param name="dir">Direction. 0:Up, 1:Down, 2:Left, 3:Right</param>
        public IEnumerator ShotFormBorder(int dir, float speed, int interval, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                float x, y, angle;
                if (dir == 0)
                {
                    x = GameSystem._Instance.player._X;
                    y = GameSystem._Instance._MaxY;
                    angle = 0.75f;
                }
                else if (dir == 1)
                {
                    x = GameSystem._Instance.player._X;
                    y = GameSystem._Instance._MinY;
                    angle = 0.25f;
                }
                else if (dir == 2)
                {
                    x = GameSystem._Instance._MinX;
                    y = GameSystem._Instance.player._Y;
                    angle = 0.0f;
                }
                else
                {
                    x = GameSystem._Instance._MaxX;
                    y = GameSystem._Instance.player._Y;
                    angle = 0.50f;
                }

                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, x, y, angle, speed);

                if (i < count - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// Shot form 
        /// </summary>
        /// <param name="roundCount">4 shot per time</param>
        public IEnumerator ShotFormCorner(float speed, int interval, int roundCount)
        {
            for (int i = 0; i < roundCount; ++i)
            {
                // 0: 우상, 1: 좌상, 2: 좌하, 3: 우하
                for (int dir = 0; dir < 4; ++dir)
                {
                    float x, y;
                    if (dir == 0)
                    {
                        x = GameSystem._Instance._MaxX;
                        y = GameSystem._Instance._MaxY;
                    }
                    else if (dir == 1)
                    {
                        x = GameSystem._Instance._MinX;
                        y = GameSystem._Instance._MaxY;
                    }
                    else if (dir == 2)
                    {
                        x = GameSystem._Instance._MinX;
                        y = GameSystem._Instance._MinY;
                    }
                    else
                    {
                        x = GameSystem._Instance._MaxX;
                        y = GameSystem._Instance._MinY;
                    }

                    float angle = GetPlayerAngle(x, y);
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(BulletName.red, x, y, angle, speed);

                    if (!((i == (roundCount - 1) && (dir == (4 - 1)))))
                    {
                        yield return new WaitForFrames(interval);
                    }
                }
            }
        }
        
        /// <summary>
        /// 원형탄
        /// </summary>
        public void CircleBullet(Mover mover, string shape, float angle, float speed, int count, bool halfAngleOffset)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), speed);
            }
        }

        public IEnumerator CircleBullets(Mover mover, string shape, float angle, float speed, int count, bool halfAngleOffset
            , int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                CircleBullet(mover, shape, angle, speed, count, halfAngleOffset);
                yield return new WaitForFrames(interval);
            }
        }

        /// <summary>
        /// 설치 원형탄
        /// </summary>
        public void DropCircleBullet(Mover mover, string shape, float angle,
            float speed, int count, bool halfAngleOffset
            , int moveDuration, int stopDuration, float angle2, float speed2)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), speed, moveDuration, stopDuration, angle2, speed2);
            }
        }

        // 랜덤 원형탄
        public IEnumerator RandomCircleBullets(Mover mover, string shape, float speed, int count, int interval, int duration)
        {
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(shape, mover._X, mover._Y, GameSystem._Instance.GetRandom01(), speed);
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// 시작각도 랜덤인 원형탄
        /// </summary>
        public IEnumerator RandomAngleCircleBullets(Mover mover, string shape, float speed, int count, int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                CircleBullet(mover, shape, GameSystem._Instance.GetRandom01(), speed, count, true);
                yield return new WaitForFrames(interval);
            }
        }

        /// <summary>
        /// N-Way 탄
        /// </summary>
        public void NWayBullet(Mover mover, string shape, float angle, float angleRange, float speed, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angle + angleRange * ((float)i / (count - 1) - 0.5f), speed);
            }
        }

        /// <summary>
        /// 무작위로 구멍 뚫린 N-Ways
        /// </summary>
        public IEnumerator RandomNWayBullets(Mover mover, string shape, float angleRange, float speed, int count
            , int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                float angle = GameSystem._Instance.GetRandom01();
                NWayBullet(mover, shape, angle, angleRange, speed, count);
                yield return new WaitForFrames(interval);
            }
        }

        /// <summary>
        /// Rotate spiral
        /// </summary>
        void SpiralBullet(Mover mover, string shape, float angle, float angleRate, float speed, int interval, int duration)
        {
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(shape, mover._X, mover._Y, angle, speed);

                    angle += angleRate;
                    angle -= Mathf.Floor(angle);
                }
            }
        }

        /// <summary>
        /// 다방향 소용돌이탄
        /// </summary>
        public IEnumerator SpiralBullets(Mover mover, string shape, float angle, float angleRate, float speed, int count, int interval, int duration)
        {
            float shotAngle = angle;
            
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(shape, mover._X, mover._Y, shotAngle + ((float)i / count), speed);
                    }
                    shotAngle += angleRate;
                    shotAngle -= Mathf.Floor(shotAngle);
                }
                yield return null;
            }
        }

        /// <summary>
        /// 양회전 소용돌이탄
        /// </summary>
        public IEnumerator BiDirectionalSpiralBullets(Mover mover, string shape
            , float angle, float angleRate1, float angleRate2
            , float speed, int count, int interval, int duration)
        {
            const int directionCount = 2;   // 회전방향의 수
            float[] shotAngle = new float[directionCount] { angle, angle };
            float[] shotAngleRate = new float[directionCount] { angleRate1, angleRate2 };

            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    // 회전이 다른 2종류의 소용돌이탄 발사
                    for (int j = 0; j < directionCount; ++j)
                    {
                        // 지정된 발사 수 만큼 발사
                        for (int i = 0; i < count; ++i)
                        {
                            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                            b.Init(shape, mover._X, mover._Y, shotAngle[j] + ((float)i / count), speed);
                        }

                        shotAngle[j] += shotAngleRate[j];
                        shotAngle[j] -= Mathf.Floor(shotAngle[j]);
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// </summary>
        public void AimimgBullet(Mover mover, string shape, float speed)
        {
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(shape, mover._X, mover._Y, GetPlayerAngle(mover), speed);
        }

        /// 조준 직선탄
        public IEnumerator AimingLineBullets(Mover mover, string shape, float speed, int interval, int repeatCount)
        {
            // 발사 시작 시 플레이어와의 각도 계산
            float angle = GetPlayerAngle(mover);

            for (int i = 0; i < repeatCount; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angle, speed);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 조준 N-Way 직선탄
        /// </summary>
        public IEnumerator AimingNWayLineBullets(Mover mover, string shape
            , float speed, int interval, int shotCount, float angleRange, int wayCount)
        {
            // 발사 시작 시 플레이어와의 각도 계산
            float angle = GetPlayerAngle(mover);

            for (int i = 0; i < shotCount; ++i)
            {
                NWayBullet(mover, shape, angle, angleRange, speed, wayCount);

                if (i < shotCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 회전 N-Way 탄
        /// </summary>
        public IEnumerator RollingNWayBullets(Mover mover, string shape
            , float angle, float angleRange, float angleRate
            , float speed, int count, int groupCount, int interval
            , int repeatCount)
        {
            for (int repeat = 0; repeat < repeatCount; ++repeat)
            {
                // 그룹 수만큼 n-way 탄 발사
                for (int group = 0; group < groupCount; ++group)
                {
                    // 360도를 n-way 수로 등분하여 n-way탄 발사 방향 결정
                    float nwayAngle = angle + (float)group / groupCount;
                    NWayBullet(mover, shape, nwayAngle, angleRange, speed, count);
                }

                // 발사 각속도 변화
                angle += angleRate;
                angle -= Mathf.Floor(angle);

                if (repeat < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 랜덤 확산탄
        /// </summary>
        public void RandomSpreadBullet(Mover mover, string shape
            , float angleRange, float speed, float speedRange, int count)
        {
            // 한 번에 뿌리지만 속도가 달라 여러번 나눠서 뿌리는 것 같은 효과
            float angle = GetPlayerAngle(mover);
            for (int i = 0; i < count; ++i)
            {
                // 탄 별로 각도와 속도를 랜덤으로 설정
                float bulletAngle = angle + angleRange * (GameSystem._Instance.GetRandom01() - 0.5f);
                float bulletSpeed = speed + speedRange * GameSystem._Instance.GetRandom01();
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, bulletAngle, bulletSpeed);
            }
        }

        /// <summary>
        /// 회전하며 조준탄 뿌리기
        /// </summary>
        public IEnumerator RollingAimingBullets(Mover mover, string shape
            , float speed, int count, float radius, int interval, int repeatCount)
        {
            float angle = GetPlayerAngle(mover) + 0.25f;    // 시작 각도는 플레이어 방향과 직각
            for (int repeat = 0; repeat < repeatCount; ++repeat)
            {
                for (int i = 0; i < count; ++i)
                {
                    // 발사 위치
                    float spawnAngleRadian = (2.0f * Mathf.PI) * (angle - (1.0f / count * i));  // 반시계방향
                    float spawnX = mover._X + radius * Mathf.Cos(spawnAngleRadian);
                    float spawnY = mover._Y + radius * Mathf.Sin(spawnAngleRadian);
                    // 발사위치로부터 플레이어 방향
                    float bulletAngle = GetPlayerAngle(spawnX, spawnY);

                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(shape, spawnX, spawnY, bulletAngle, speed);

                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// Rotates the cross twice direction shot.
        /// </summary>
        /// <returns>The cross twice direction shot.</returns>
        /// <param name="mover">Mover.</param>
        /// <param name="clockwise">If set to <c>true</c> clockwise.</param>
        /// <param name="repeatCount">Repeat count.</param>
        /// <param name="interval">Interval.</param>
        public IEnumerator RotateCrossTwice_DirectionShot(Mover mover, bool clockwise, int repeatCount, int interval)
        {
            const int directionCount = 4;
            const float startAngle = 0.75f;
            const float angleRange = 1.0f - 1.0f / directionCount;
            for (int i = 0; i < repeatCount; ++i)
            {
                float angle = startAngle + (1.0f / (float)repeatCount) * (float)i * (clockwise ? -1.0f : 1.0f);
                NWayBullet(mover, BulletName.red, angle, angleRange, 0.01f, directionCount);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        #endregion Base


    }
}