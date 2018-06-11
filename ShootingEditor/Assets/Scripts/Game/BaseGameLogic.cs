using UnityEngine;
using System.Collections;

namespace Game
{
    /// <summary>
    /// 음악별 다른 동작 정의
    /// </summary>
    public abstract class BaseGameLogic
    {
        public CoroutineManager _coroutineManager = new CoroutineManager();
        // 특화 정보 로딩
        public abstract IEnumerator LoadContext();

        // 특화 정보 갱신
        public abstract void UpdatePlayContext();

        protected Enemy boss;

        // 기본 플레이어기 로딩
        protected IEnumerator LoadBasicPlayer()
        {
            // 플레이어기 로딩 /////////////////
            GameSystem._Instance._UILoading.SetProgress("Loading Player");
            yield return null;
            GameSystem._Instance.PoolStackShape(PlayName.black, 1);
            GameSystem._Instance.PoolStackMover<PlayerAlive>(1);
            GameSystem._Instance.PoolStackShape(PlayEffectName.black, 1);
            GameSystem._Instance.PoolStackMover<PlayerCrash>(1);

            // 샷 로딩 ///////////////////////
            GameSystem._Instance._UILoading.SetProgress("Loading Shots");
            yield return null;
            GameSystem._Instance.PoolStackShape(ShotName.black, 36);
            GameSystem._Instance.PoolStackMover<Shot>(36);
        }

        #region Util
        /// <summary>
        /// 지정한 좌표로부터 플레이어로 향하는 각도 구하기
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetPlayerAngle(float x, float y)
        {
            // Atan2 의 결과가 라디안이므로 0~1로 변경
            Vector2 playerPos = GameSystem._Instance._Player._pos;
            return Mathf.Atan2(playerPos.y - y, playerPos.x - x) / Mathf.PI / 2.0f;
        }

        /// <summary>
        /// 지정한 무버로부터 플레이어로 향하는 각도 구하기
        /// </summary>
        /// <param name="startMover"></param>
        /// <returns></returns>
        public float GetPlayerAngle(Mover startMover)
        {
            return GetPlayerAngle(startMover._X, startMover._Y);
        }
        #endregion Util

        #region Move
        /// <summary>
        /// 무버를 지정한 위치까지 등속 이동
        /// </summary>
        public IEnumerator MoveConstantVelocity(Mover mover, Vector2 arrivePos, int duration)
        {
            Vector2 delta = (arrivePos - mover._pos) / (float)duration; // 한 프레임에 움직일 거리

            for (int i = 0; i < duration - 1; ++i)
            {
                mover._pos += delta;
                yield return null;
            }

            // 마지막 프레임
            mover._pos = arrivePos;
        }

        /// <summary>
        /// 무버를 지정한 위치까지 비례감속 이동
        /// </summary>
        public IEnumerator MoveDamp(Mover mover, Vector2 arrivePos, int duration, float damp)
        {
            for (int i = 0; i < duration - 1; ++i)
            {
                mover._pos = Vector2.Lerp(mover._pos, arrivePos, damp);
                yield return null;
            }

            // 마지막 프레임
            mover._pos = arrivePos;
        }

        #endregion

        #region Base
        /// <summary>
        /// 사면에서 직각방향으로 발사되는 시작위치 조준탄
        /// </summary>
        /// <param name="dir">시작방향. 0:상, 1:하, 2:좌, 3: 우</param>
        public IEnumerator SideAim(int dir, float speed, int interval, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                float x, y, angle;
                if (dir == 0)
                {
                    x = GameSystem._Instance._Player._X;
                    y = GameSystem._Instance._MaxY;
                    angle = 0.75f;
                }
                else if (dir == 1)
                {
                    x = GameSystem._Instance._Player._X;
                    y = GameSystem._Instance._MinY;
                    angle = 0.25f;
                }
                else if (dir == 2)
                {
                    x = GameSystem._Instance._MinX;
                    y = GameSystem._Instance._Player._Y;
                    angle = 0.0f;
                }
                else
                {
                    x = GameSystem._Instance._MaxX;
                    y = GameSystem._Instance._Player._Y;
                    angle = 0.50f;
                }

                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, x, y, angle, 0.0f, speed, 0.0f);

                if (i < count - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 모서리에서 플레이어 방향으로 발사되는 조준탄
        /// </summary>
        /// <param name="interval">탄별 간격</interval>
        /// <param name="roundCount">4모서리 순회를 몇 번 할 것인가</param>
        public IEnumerator CornerAim(float speed, int interval, int roundCount)
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
                    b.Init(BulletName.red, x, y, angle, 0.0f, speed, 0.0f);

                    if (!((i == (roundCount - 1) && (dir == (4 - 1)))))
                    {
                        yield return new WaitForFrames(interval);
                    }
                }
            }
        }
        
        /// <summary>
        /// N-Way 탄
        /// </summary>
        public void NWayBullet(Mover mover, string shape, float angle, float angleRange, float speed, int count)
        {
            if (count > 1)
            {
                for (int i = 0; i < count; ++i)
                {
                    // (angle - angleRange / 2) ~ (angle + angleRange / 2) 범위에서 count 만큼 생성
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(shape, mover._X, mover._Y, angle + angleRange * ((float)i / (count - 1) - 0.5f), 0.0f, speed, 0.0f);
                }
            }
            else if (count == 1)
            {
                // 탄 수가 하나일 때는 발사 각도로 1개 발사
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angle, 0.0f, speed, 0.0f);
            }
        }

        /// <summary>
        /// 무작위로 구멍 뚫린 N-Way
        /// </summary>
        public IEnumerator GapBullets(Mover mover, string shape, float angleRange, float speed, int count
            , int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                float angle = GameSystem._Instance.GetRandom01();
                NWayBullet(mover, shape, angle, angleRange, speed, count);
                yield return new WaitForFrames(interval);
            }
        }

        public IEnumerator CustomGapBullets(Mover mover, string shape, float angleRange, float speed, int count
            , int interval, float[] angles)
        {
            for (int i = 0; i < angles.Length; ++i)
            {
                NWayBullet(mover, shape, angles[i], angleRange, speed, count);
                yield return new WaitForFrames(interval);
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
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), 0.0f, speed, 0.0f);
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
        public void PlacedCircleBullet(Mover mover, string shape, float angle, float speed, int count, bool halfAngleOffset
            , int moveDuration, int stopDuration, float angle2, float speed2)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), speed, moveDuration, stopDuration, angle2, speed2);
            }
        }

        /// <summary>
        /// 선회가속 원형탄
        /// </summary>
        public void BentCircleBullet(Mover mover, string shape, float angle, float speed, int count, float bulletAngleRate, float bulletSpeedRate, bool halfAngleOffset)
        {
            float angleStart = angle + ((halfAngleOffset) ? (1.0f / count / 2.0f) : 0.0f);
            for (int i = 0; i < count; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angleStart + (1.0f / count * i), bulletAngleRate, speed, bulletSpeedRate);
            }
        }

        public IEnumerator BentCircleBullets(Mover mover, string shape, float angle, float speed, int count, float bulletAngleRate, float bulletSpeedRate, bool halfAngleOffset
            , int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                BentCircleBullet(mover, shape, angle, speed, count, bulletAngleRate, bulletSpeedRate, halfAngleOffset);
                yield return new WaitForFrames(interval);
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
                        b.Init(shape, mover._X, mover._Y, GameSystem._Instance.GetRandom01()
                            , 0.0f, speed, 0.0f);
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
        /// 소용돌이탄
        /// </summary>
        public IEnumerator SpiralBullets(Mover mover, string shape, float angle, float angleRate, float speed, int interval, int duration)
        {
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(shape, mover._X, mover._Y, angle, 0.0f, speed, 0.0f);

                    angle += angleRate;
                    angle -= Mathf.Floor(angle);
                }
                yield return null;
            }
        }

        /// <summary>
        /// 다방향 소용돌이탄
        /// </summary>
        public IEnumerator MultipleSpiralBullets(Mover mover, string shape, float angle, float angleRate, float speed, int count, int interval, int duration)
        {
            float shotAngle = angle;
            
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    // 지정된 발사 수 만큼 발사
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(shape, mover._X, mover._Y, shotAngle + ((float)i / count)
                            , 0.0f, speed, 0.0f);
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
                            b.Init(shape, mover._X, mover._Y, shotAngle[j] + ((float)i / count)
                                , 0.0f, speed, 0.0f);
                        }

                        shotAngle[j] += shotAngleRate[j];
                        shotAngle[j] -= Mathf.Floor(shotAngle[j]);
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// 선회가속 소용돌이탄
        /// </summary>
        public IEnumerator BentSpiralBullets(Mover mover, string shape
            , float angle, float angleRate, float speed, int count, int interval
            , float bulletAngleRate, float bulletSpeedRate, int duration)
        {
            float shotAngle = angle;
            
            for (int frame = 0; frame < duration; ++frame)
            {
                if ((frame % interval) == 0)
                {
                    // 지정된 발사 수 만큼 발사
                    for (int i = 0; i < count; ++i)
                    {
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(shape, mover._X, mover._Y, shotAngle + ((float)i / count)
                            , bulletAngleRate, speed, bulletSpeedRate);
                    }

                    shotAngle += angleRate;
                    shotAngle -= Mathf.Floor(shotAngle);
                }
                yield return null;
            }
        }

        /// <summary>
        /// 직선탄
        /// </summary>
        public IEnumerator LineBullets(Vector2 pos, string shape, float angle, float speed, int interval, int repeatCount)
        {
            for (int i = 0; i < repeatCount; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, pos.x, pos.y, angle, 0.0f, speed, 0.0f);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 조준 직선탄
        /// </summary>
        public IEnumerator AimingLineBullets(Mover mover, string shape, float speed, int interval, int repeatCount)
        {
            // 발사 시작 시 플레이어와의 각도 계산
            float angle = GetPlayerAngle(mover);

            for (int i = 0; i < repeatCount; ++i)
            {
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(shape, mover._X, mover._Y, angle, 0.0f, speed, 0.0f);

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
                b.Init(shape, mover._X, mover._Y, bulletAngle, 0.0f, bulletSpeed, 0.0f);
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
                    b.Init(shape, spawnX, spawnY, bulletAngle, 0.0f, speed, 0.0f);

                    yield return new WaitForFrames(interval);
                }
            }
        }

        /// <summary>
        /// 배열 패턴 탄
        /// </summary>
        public IEnumerator PatternBullets(Mover mover, string shape
            , float angle, float speed, int interval, byte[] pattern, int col, int row, float colSpace)
        {
            // 한줄씩 발사
            float rad = angle * Mathf.PI * 2.0f;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            float centerCol = ((float)col - 1.0f) / 2.0f;   // 가운데 칸의 인덱스 (예: 2칸 -> [0] "[0.5]" [1])
            
            // 아랫줄 부터 출력
            for (int r = row - 1; r >= 0; --r)
            {
                // 이번 줄 첫번째 인덱스
                int startIndex = col * r;

                // 왼쪽 탄을 위에 보이도록 하기 위해 오른쪽부터 읽음
                for (int i = col - 1; i >= 0; --i)
                {
                    // 0 외의 부분에 탄 발사
                    int index = startIndex + i;
                    if (pattern[index] != 0)
                    {
                        // 화면 아래로 향할 때(0.75, 270도) 패턴 그대로 출력
                        // 즉, 0도일 떄는 colOffset이 Y축으로 작동
                        float xOffset = 0.0f;
                        float yOffset = ((float)i - centerCol) * colSpace; // 가운데 칸으로부터 얼마나 떨어졌는가?
                        float x = mover._X + (cos * xOffset + -1.0f * sin * yOffset); // 벡터 회전공식 참고. http://en.wikipedia.org/wiki/Rotation_(mathematics)
                        float y = mover._Y + (sin * xOffset + cos * yOffset);
                        Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                        b.Init(shape, x, y, angle, 0.0f, speed, 0.0f);
                    }
                }

                if (r > 0)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        public void AimimgBullet(Mover mover, string shape, float speed)
        {
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(shape, mover._X, mover._Y, GetPlayerAngle(mover), 0.0f, speed, 0.0f);
        }
        #endregion Pattern

        #region Pattern 1
        // 단순 3파
        public IEnumerator Simple3Wave(bool leftToRight)
        {
            // 1파
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, boss._X, boss._Y, 0.75f
                , 0.0f, 0.02f, 0.0f);
            yield return new WaitForFrames(11);

            // 2파
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, boss._X - 0.2f, boss._Y, 0.75f
                , 0.0f, 0.02f, 0.0f);
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, boss._X + 0.2f, boss._Y, 0.75f
                , 0.0f, 0.02f, 0.0f);
            yield return new WaitForFrames(12);

            // 3파
            const float angle1 = 0.625f;
            const float angle2 = 0.875f;
            float startAngle = (leftToRight) ? angle1 : angle2;
            float endAngle = (leftToRight) ? angle2 : angle1;
            const int count = 7;
            for (int i = 0; i < count; ++i)
            {
                b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.blue, boss._X, boss._Y, (startAngle + (endAngle - startAngle) / count * i)
                    , 0.0f, 0.02f, 0.0f);

                if (i < count - 1)
                {
                    yield return new WaitForFrames(7);
                }
            }
        }

        // 단순 4파
        public IEnumerator Simple4Wave()
        {
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, -0.2f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, 0.2f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            yield return new WaitForFrames(25);

            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, -0.4f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, 0.4f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            yield return new WaitForFrames(25);

            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, -0.6f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, 0.6f, 1.3f, 0.75f
                , 0.0f, 0.03f, 0.0f);
            yield return new WaitForFrames(25);

            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, -0.8f, 1.3f, 0.75f, 0.0f, 0.03f, 0.0f);
            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, 0.8f, 1.3f, 0.75f, 0.0f, 0.03f, 0.0f);
        }

        // 단순 원형 연속
        public IEnumerator SimpleCircles()
        {
            const int count = 9;
            for (int i = 0; i < count; ++i)
            {
                bool halfAngleOffset = (i % 2) != 0;
                CircleBullet(boss, BulletName.blue, 0.0f, 0.005f, 20, halfAngleOffset);

                if (i < count - 1)
                {
                    yield return new WaitForFrames(13);
                }
            }
        }

        public IEnumerator AimAfterSimpleCircles()
        {
            const int count = 6;
            for (int i = 0; i < count; ++i)
            {
                float playerAngle = GetPlayerAngle(boss);
                Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                b.Init(BulletName.red, boss._X, boss._Y, playerAngle
                    , 0.0f, 0.02f, 0.0f);

                if (i < count - 1)
                {
                    yield return new WaitForFrames(55);
                }
            }
        }

        // 각 모서리에서 진행할 웨이브들
        public IEnumerator CornerWaves(bool leftCorner)
        {
            // 2파 1
            yield return _coroutineManager.StartCoroutine(CornerWaves_2Wave(leftCorner));
            // 3파 2
            yield return _coroutineManager.StartCoroutine(CornerWaves_2Wave(leftCorner));
            // 4파
            {
                float x = (leftCorner) ? 1.0f : -1.0f;
                float angle = (leftCorner) ? 0.5f : 0.0f;
                for (int i = 0; i < 4; ++i)
                {
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(BulletName.red, x, 0.5f - 1.5f / 3.0f * i, angle
                        , 0.0f, 0.01f, 0.0f);
                    yield return new WaitForFrames(25);
                }
            }
            // 연타
            CircleBullet(boss, BulletName.blue, 0.0f, 0.02f, 12, false);
            yield return new WaitForFrames(26);
            CircleBullet(boss, BulletName.blue, 0.0f, 0.02f, 12, true);
        }

        public IEnumerator CornerWaves_2Wave(bool leftCorner)
        {
            // 1탄
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.blue, boss._X, boss._Y, 0.75f
                , 0.0f, 0.02f, 0.0f);
            yield return new WaitForFrames(32);

            float angle = ((leftCorner) ? 0.875f : 0.625f);
            NWayBullet(boss, BulletName.blue, angle, 0.25f, 0.02f, 6);
            yield return new WaitForFrames(75);
        }

        public IEnumerator RotateCrossTwice1()
        {
            const int repeatCount = 4;
            const int interval = 105;
            _coroutineManager.StartCoroutine(RotateCrossTwice_DirectionShot(false, repeatCount, interval));
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.125f, 0.005f, 0.05f, 4, 2, 410));
            _coroutineManager.StartCoroutine(RotateCrossTwice_DirectionShot(true, repeatCount, interval));
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.125f, -0.005f, 0.05f, 4, 2, 410));
        }

        public IEnumerator RotateCrossTwice2()
        {
            const int repeatCount = 4 * 2;
            const int interval = 105 / 2;
            _coroutineManager.StartCoroutine(RotateCrossTwice_DirectionShot(false, repeatCount, interval));
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.125f, 0.005f, 0.05f, 4, 2, 410));
            _coroutineManager.StartCoroutine(RotateCrossTwice_DirectionShot(true, repeatCount, interval));
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.125f, -0.005f, 0.05f, 4, 2, 410));
        }

        public IEnumerator RotateCrossTwice_DirectionShot(bool clockwise, int repeatCount, int interval)
        {
            const int directionCount = 4;
            const float startAngle = 0.75f;
            const float angleRange = 1.0f - 1.0f / directionCount;
            for (int i = 0; i < repeatCount; ++i)
            {
                float angle = startAngle + (1.0f / (float)repeatCount) * (float)i * (clockwise ? -1.0f : 1.0f);
                NWayBullet(boss, BulletName.red, angle, angleRange, 0.01f, directionCount);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        public IEnumerator BackwardStep()
        {
            // 뒷걸음질 치기는 병렬로 수행
            _coroutineManager.StartCoroutine(MoveConstantVelocity(boss, new Vector2(0.0f, 0.75f), 360));

            // 첫 탄 발사 전 딜레이
            const int interval = 45;
            yield return new WaitForFrames(interval);

            const float speed = 0.01f;
            const float angle = 0.75f;
            const float nwayAngleRange = 0.125f;
            Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, boss._X, boss._Y, angle, 0.0f, speed, 0.0f);
            yield return new WaitForFrames(interval);

            NWayBullet(boss, BulletName.red, angle, nwayAngleRange, speed, 2);
            yield return new WaitForFrames(interval);

            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, boss._X, boss._Y, angle, 0.0f, speed, 0.0f);
            yield return new WaitForFrames(interval);

            NWayBullet(boss, BulletName.red, angle, nwayAngleRange, speed, 2);
            yield return new WaitForFrames(interval);

            b = GameSystem._Instance.CreateBullet<Bullet>();
            b.Init(BulletName.red, boss._X, boss._Y, angle, 0.0f, speed, 0.0f);
            yield return new WaitForFrames(interval);

            NWayBullet(boss, BulletName.red, angle, nwayAngleRange, speed, 2);
            yield return new WaitForFrames(interval);
        }

        // 비둘기 솔로
        public IEnumerator PigeonSolo()
        {
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.0f, 0.02f, 0.01f, 4, 5, 193));
            yield return _coroutineManager.StartCoroutine(MultipleSpiralBullets(boss, BulletName.blue, 0.0f, -0.02f, 0.01f, 4, 5, 193));
            yield return _coroutineManager.StartCoroutine(BiDirectionalSpiralBullets(boss, BulletName.blue, 0.0f, 0.03f, -0.02f, 0.01f, 4, 5, 400));
            yield return new WaitForFrames(90);
            yield return _coroutineManager.StartCoroutine(BentSpiralBullets(boss, BulletName.red, 0.0f, 0.02f, 0.0f, 10, 10, -0.003f, 0.0002f, 400));
            yield return new WaitForFrames(20);
            // 중앙으로 이동
            yield return _coroutineManager.StartCoroutine(MoveConstantVelocity(boss, new Vector2(0.0f, 0.0f), 180));
            yield return new WaitForFrames(30);
            // 랜덤 뿌리기
            yield return _coroutineManager.StartCoroutine(RandomCircleBullets(boss, BulletName.blue, 0.01f, 3, 3, 150));
        }
        #endregion //Coroutine

        #region Pattern 2
        private int _patternDPartDuration = 60 * 14;    // 패턴 D의 파트별 지속시간
        private readonly byte[] _patternNote =  // 음표모양 패턴
        {
                0, 0, 0, 1, 1, 0, 0,
                0, 0, 0, 1, 0, 1, 0,
                0, 0, 0, 1, 0, 0, 1,
                0, 0, 0, 1, 0, 1, 0,
                0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 1, 0, 0, 0,
                0, 1, 1, 1, 0, 0, 0,
                1, 1, 1, 1, 1, 0, 0,
                0, 1, 1, 1, 0, 0, 0,
            };
        private readonly int _patternNoteCol = 7;
        private readonly int _patternNoteRow = 9;

        public IEnumerator PatternA_11()
        {
            yield return _coroutineManager.StartCoroutine(PatternA_a1());
            yield return new WaitForFrames(100);
            yield return _coroutineManager.StartCoroutine(PatternA_b1());
        }

        public IEnumerator PatternA_22()
        {
            yield return _coroutineManager.StartCoroutine(PatternA_a2());
            yield return new WaitForFrames(110);
            yield return _coroutineManager.StartCoroutine(PatternA_b2());
        }

        public IEnumerator PatternA_a1()
        {
            const int interval = 5;
            _coroutineManager.StartCoroutine(AimingLineBullets(boss, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            _coroutineManager.StartCoroutine(AimingLineBullets(boss, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            _coroutineManager.StartCoroutine(AimingNWayLineBullets(boss, BulletName.red, 0.02f, interval, 4, 0.125f, 3));
            yield return new WaitForFrames(25);
            _coroutineManager.StartCoroutine(AimingNWayLineBullets(boss, BulletName.red, 0.02f, interval, 9, 0.125f, 3));
        }

        public IEnumerator PatternA_b1()
        {
            const int circleInterval = 28;
            _coroutineManager.StartCoroutine(RandomAngleCircleBullets(boss, BulletName.blue, 0.02f, 12, circleInterval, 7));
            yield return new WaitForFrames(circleInterval / 2);
            _coroutineManager.StartCoroutine(RandomAngleCircleBullets(boss, BulletName.blue, 0.02f, 12, circleInterval, 6));
        }

        public IEnumerator Aiming2LineBullets(Mover mover, string shape, float speed, float gap, int interval, int repeatCount)
        {
            // 발사 시작 시 플레이어와의 각도 계산
            float angle = GetPlayerAngle(mover);
            float rad = angle * Mathf.PI * 2.0f;
            float sin = Mathf.Sin(rad);
            float cos = Mathf.Cos(rad);
            float xOffset = 0.0f;
            float yOffset1 = gap;
            float yOffset2 = gap * -1.0f;
            float x1 = mover._X + (cos * xOffset + -1.0f * sin * yOffset1);
            float y1 = mover._Y + (sin * xOffset + cos * yOffset1);
            float x2 = mover._X + (cos * xOffset + -1.0f * sin * yOffset2);
            float y2 = mover._Y + (sin * xOffset + cos * yOffset2);

            for (int i = 0; i < repeatCount; ++i)
            {
                Bullet b1 = GameSystem._Instance.CreateBullet<Bullet>();
                b1.Init(shape, x1, y1, angle, 0.0f, speed, 0.0f);
                Bullet b2 = GameSystem._Instance.CreateBullet<Bullet>();
                b2.Init(shape, x2, y2, angle, 0.0f, speed, 0.0f);

                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        public IEnumerator PatternA_a2()
        {
            const int interval = 5;
            _coroutineManager.StartCoroutine(Aiming2LineBullets(boss, BulletName.red, 0.02f, 0.15f, interval, 5));
            yield return new WaitForFrames(50);
            _coroutineManager.StartCoroutine(AimingLineBullets(boss, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            _coroutineManager.StartCoroutine(Aiming2LineBullets(boss, BulletName.red, 0.02f, 0.15f, interval, 4));
            yield return new WaitForFrames(25);
            _coroutineManager.StartCoroutine(AimingNWayLineBullets(boss, BulletName.red, 0.02f, interval, 9, 0.125f, 3));
        }

        public IEnumerator PatternA_b2()
        {
            const int circleInterval = 28;
            _coroutineManager.StartCoroutine(RandomAngleCircleBullets(boss, BulletName.blue, 0.02f, 12, circleInterval, 4));
            yield return new WaitForFrames(circleInterval / 2);
            _coroutineManager.StartCoroutine(RandomAngleCircleBullets(boss, BulletName.blue, 0.02f, 12, circleInterval, 4));
            yield return new WaitForFrames(circleInterval * 4);

            _coroutineManager.StartCoroutine(PatternBullets(boss, BulletName.blue, 0.75f, 0.02f, 4, _patternNote, _patternNoteCol, _patternNoteRow, 0.08f));
        }

        public IEnumerator PatternB()
        {
            const float rollingAngleRate = 0.02f;
            const float rollingAngleRange = 0.22f;
            const int rollingRepeatCount = 9;
            const float rollingAngleOffset = (rollingAngleRate * rollingRepeatCount) / 2.0f;
            const int rollingCount = 5;

            // 뿌리기
            RandomSpreadBullet(boss, BulletName.red, 0.2f, 0.02f, 0.02f, 24);

            // 반시계 회전
            yield return new WaitForFrames(100);
            _coroutineManager.StartCoroutine(RollingNWayBullets(boss, BulletName.blue
                , 0.75f - rollingAngleOffset, rollingAngleRange, rollingAngleRate, 0.02f, rollingCount, 1, 5, rollingRepeatCount));

            // 뿌리기
            yield return new WaitForFrames(140);
            RandomSpreadBullet(boss, BulletName.red, 0.2f, 0.02f, 0.02f, 24);

            // 시계 회전
            yield return new WaitForFrames(100);
            _coroutineManager.StartCoroutine(RollingNWayBullets(boss, BulletName.blue
                , 0.75f + rollingAngleOffset, rollingAngleRange, -rollingAngleRate, 0.02f, rollingCount, 1, 5, rollingRepeatCount));
        }

        /// <summary>
        /// 회전하며 조준탄 뿌린 후 랜덤이동
        /// </summary>
        public IEnumerator PatternC_1()
        {
            const float radius = 0.5f;
            const int repeatCount = 8;
            for (int i = 0; i < repeatCount; ++i)
            {
                _coroutineManager.StartCoroutine(RollingAimingBullets(boss, BulletName.red, 0.02f, 18, radius, 1, 2));

                // 마지막 뿌리기 후에는 이동하지 않음
                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(60);
                    Vector2 nextPos = new Vector2(
                        GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinX + radius + 0.1f, GameSystem._Instance._MaxX - radius - 0.1f)
                        , GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinY * 0.2f + radius + 0.1f, GameSystem._Instance._MaxY - radius - 0.1f));
                    _coroutineManager.StartCoroutine(MoveDamp(boss, nextPos, 30, 0.1f));
                    yield return new WaitForFrames(60);
                }
            }
        }

        public IEnumerator PatternC_2()
        {
            const float radius = 0.5f;
            const int repeatCount = 8;
            const int countPerCircle = 10;
            const float speed1_Cirlce1 = 0.04f;
            const float speed1_Cirlce2 = 0.03f;
            const float angle2 = 0.75f;
            const float speed2 = 0.02f;
            const int moveDuaraion = 12;
            const int stopDuaraion = 24;
            const int placeInterval = 12;
            for (int i = 0; i < repeatCount; ++i)
            {
                // 설치 원형탄 2개
                float angle = 0.75f;
                PlacedCircleBullet(boss, BulletName.blue, angle, speed1_Cirlce1, countPerCircle, false, moveDuaraion, stopDuaraion, angle2, speed2);
                yield return new WaitForFrames(placeInterval);
                PlacedCircleBullet(boss, BulletName.red, angle, speed1_Cirlce2, countPerCircle, true, moveDuaraion, stopDuaraion - placeInterval, angle2, speed2);

                // 마지막 뿌리기 후에는 이동하지 않음
                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(60 - placeInterval);
                    Vector2 nextPos = new Vector2(
                        GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinX + radius + 0.1f, GameSystem._Instance._MaxX - radius - 0.1f)
                        , GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinY * 0.2f + radius + 0.1f, GameSystem._Instance._MaxY - radius - 0.1f));
                    _coroutineManager.StartCoroutine(MoveDamp(boss, nextPos, 30, 0.1f));
                    yield return new WaitForFrames(60);
                }
            }
        }

        public IEnumerator PatternD_1()
        {
            yield return _coroutineManager.StartCoroutine(PatternD_1_Follow());
            _coroutineManager.StartCoroutine(PatternD_1_Circle()); // 원형탄과 함께 따라다니기
            yield return _coroutineManager.StartCoroutine(PatternD_1_Follow());
        }

        /// <summary>
        /// 캐릭터 따라가며 탄 설치
        /// </summary>
        public IEnumerator PatternD_1_Follow()
        {
            const float speed = 0.007f;
            const float maxAngleRate = 0.01f; // 최대 선회 각속도
            const int interval = 10; // 발사 간격
            const string shape = BulletName.blue;
            float angle = GetPlayerAngle(boss);

            for (int i = 0; i < _patternDPartDuration; ++i)
            {
                // 현재 위치에 패턴 종료시까지 움직이지 않는 탄 생성
                if (i % interval == 0)
                {
                    AwayBullet b = GameSystem._Instance.CreateBullet<AwayBullet>();
                    b.Init(shape, boss._X, boss._Y, 0.0f, 0.0f, 0.0f, 0.0f, _patternDPartDuration - i - 1, 0.03f);
                }

                // 선회 각속도 계산
                float angleRate = GetPlayerAngle(boss) - angle;
                // 선회 각속도를 0~1 범위로 제한
                angleRate -= Mathf.Floor(angleRate);

                // 선회 각속도가 최대 선회 각속도 이하면
                // 선회 각속도로 선회
                if (angleRate <= maxAngleRate || (1.0f - angleRate) <= maxAngleRate)
                {
                    angle += angleRate;
                }
                // 선회 각속도가 최대 선회 각속도보다 크면
                // 최대 선회 각속도로 선회
                else
                {
                    angle += (angleRate < 0.5f) ? maxAngleRate : -maxAngleRate;
                }
                angle -= Mathf.Floor(angle);

                // 계산한 각도를 사용해 좌표 갱신
                float rad = angle * Mathf.PI * 2.0f;
                boss._X += speed * Mathf.Cos(rad);
                boss._Y += speed * Mathf.Sin(rad);

                yield return null;
            }
        }

        /// <summary>
        /// 주기적으로 원형탄 발사
        /// </summary>
        public IEnumerator PatternD_1_Circle()
        {
            for (int i = 0; i < 14; ++i)
            {
                CircleBullet(boss, BulletName.red, GameSystem._Instance.GetRandom01(), 0.01f, 6, true);
                yield return new WaitForFrames(60);
            }
        }

        public IEnumerator PatternD_2()
        {
            // 안전선 발사
            PatternD_2_SafetyLine();
            // 보스 기준위치로 이동
            yield return _coroutineManager.StartCoroutine(MoveConstantVelocity(boss, new Vector2(0.0f, 0.75f), 240));
            yield return new WaitForFrames(240);
            // 반원 발사
            yield return _coroutineManager.StartCoroutine(PatternD_2_HalfCirclePlaced());
        }

        /// <summary>
        /// 이후 패턴을 위한 안전선
        /// </summary>
        public void PatternD_2_SafetyLine()
        {
            const float speed1 = 0.0045f;
            const float speed2 = 0.01f;
            const int phase1Duration = 480;
            const int count = 10;
            const string shape = BulletName.red;

            float startX = GameSystem._Instance._MinX;
            float gapX = (GameSystem._Instance._MaxX - GameSystem._Instance._MinX) / (count - 1);
            float y = GameSystem._Instance._MaxY;

            for (int i = 0; i < count; ++i)
            {
                // 아래로 내려오다가
                // 페이즈 2 때 절반은 왼쪽으로, 절반은 오른쪽으로 사라짐
                PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                b.InitNoStop(shape, startX + (i * gapX), y, 0.75f, speed1
                    , phase1Duration, (i < (count / 2) ? 0.5f : 0.0f), speed2);
            }
        }

        /// <summary>
        /// 반원 배치로 2단계 탄 발사
        /// </summary>
        public IEnumerator PatternD_2_HalfCirclePlaced()
        {
            // 반원 배치로 빠르게 진행하다가 하단으로 천천히 떨어짐
            const float speed1 = 0.05f;
            const float speed2 = 0.01f;
            const int phase1Duration = 30;
            const int count = 12;
            const float angleRange = 100.0f / 360.0f;
            const float startAngleOffset = (angleRange / (float)(count - 1)) / 2.0f;
            const int interval = 20;
            const string shape = BulletName.blue;

            for (int frame = 0; frame < (_patternDPartDuration / 2); ++frame)
            {
                if (frame % interval == 0)
                {
                    float startAngle = 0.75f + GameSystem._Instance.GetRandomRange(-startAngleOffset, startAngleOffset);
                    for (int i = 0; i < count; ++i)
                    {
                        PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                        b.InitNoStop(shape, boss._X, boss._Y, startAngle + angleRange * ((float)i / (count - 1) - 0.5f), speed1
                            , phase1Duration, 0.75f, speed2);
                    }
                }
                yield return null;
            }
        }

        public IEnumerator PatternE_1()
        {
            const int shooterCount = 2;
            const int cycle = 700;
            SpiralPlacedShooterBullet[] bs = new SpiralPlacedShooterBullet[shooterCount];
            for (int i = 0; i < shooterCount; ++i)
            {
                string shape = (i % 2 == 0) ? BulletName.blueLarge : BulletName.redLarge;
                string bulletShape = (i % 2 == 0) ? BulletName.blueSmall : BulletName.redSmall;
                float orbitAngle = 1.0f * ((float)i / (float)shooterCount);

                bs[i] = GameSystem._Instance.CreateBullet<SpiralPlacedShooterBullet>();
                bs[i].Init(shape, orbitAngle, 0.002f, 0.9f
                    , 30, 30, 3, cycle
                    , bulletShape, 0.01f, 8);
            }

            // 사이클 후 슈터 삭제
            yield return new WaitForFrames(cycle - 1);
            for (int i = 0; i < shooterCount; ++i)
            {
                bs[i]._alive = false;
            }
        }

        public IEnumerator PatternE_2()
        {
            const int shooterCount = 4;
            const int cycle = 620;
            SpiralPlacedShooterBullet[] bs = new SpiralPlacedShooterBullet[shooterCount];
            for (int i = 0; i < shooterCount; ++i)
            {
                bool isBlue = (i % 2 == 0);
                string shape = (isBlue) ? BulletName.blueLarge : BulletName.redLarge;
                string bulletShape = (isBlue) ? BulletName.blueSmall : BulletName.redSmall;
                float orbitAngle = ((float)1 / (float)shooterCount / 2.0f) + ((float)i / (float)shooterCount);
                float orbitAngleRate = 0.002f;
                float orbitRadius = (isBlue) ? 0.9f : 0.8f;
                int shotTime = (isBlue) ? 20 : 25;
                int waitTime = (isBlue) ? 40 : 35;
                float bulletSpeed = (isBlue) ? 0.01f : 0.008f;

                bs[i] = GameSystem._Instance.CreateBullet<SpiralPlacedShooterBullet>();
                bs[i].Init(shape, orbitAngle, orbitAngleRate, orbitRadius
                    , shotTime, waitTime, 4, 700
                    , bulletShape, bulletSpeed, 6);
            }

            // 사이클 후 슈터 삭제
            yield return new WaitForFrames(cycle - 1);
            for (int i = 0; i < shooterCount; ++i)
            {
                bs[i]._alive = false;
            }
        }
        #endregion //Coroutine

    }
}