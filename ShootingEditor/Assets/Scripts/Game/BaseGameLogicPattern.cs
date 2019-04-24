using System.Collections;
using UnityEngine;

namespace Game
{
    public partial class BaseGameLogic
    {

        #region Pattern 1
        /// 배열 패턴 탄
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
        public IEnumerator Pattern_01(Mover mover)
        {
            string shape = BulletName.blue;
            float angle = 0.75f;
            float speed = 0.02f;
            int interval = 10;
            byte[] pattern = _patternNote;
            int col = _patternNoteCol;
            int row = _patternNoteRow;
            float colSpace = 0.08f;

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
                        b.Init(shape, x, y, angle, speed);
                    }
                }

                if (r > 0)
                {
                    yield return new WaitForFrames(interval);
                }
            }
        }

        public IEnumerator Pattern_02(Mover mover)
        {
            // 1
            float angle = 0.875f;
            NWayBullet(mover, BulletName.blue, angle, 0.25f, 0.02f, 6);
            yield return new WaitForFrames(75);
            // 2
            NWayBullet(mover, BulletName.blue, angle, 0.25f, 0.02f, 6);
            yield return new WaitForFrames(75);
            // 4 Way
            {
                float x = 1.0f;
                angle = 0.5f;
                for (int i = 0; i < 4; ++i)
                {
                    Bullet b = GameSystem._Instance.CreateBullet<Bullet>();
                    b.Init(BulletName.red, x, 0.5f - 1.5f / 3.0f * i, angle, 0.01f);
                    yield return new WaitForFrames(25);
                }
            }
            // 연타
            CircleBullet(mover, BulletName.blue, 0.0f, 0.02f, 12, false);
            yield return new WaitForFrames(26);
            CircleBullet(mover, BulletName.blue, 0.0f, 0.02f, 12, true);
        }

        // RotateCrossTwice
        public IEnumerator Pattern_03(Mover mover, bool left = true)
        {
            const int repeatCount = 4;
            const int interval = 105;
            float angleRate = left ? 0.005f : -0.005f;
            StartCoroutine(RotateCrossTwice_DirectionShot(mover, false, repeatCount, interval));
            yield return StartCoroutine(SpiralBullets(mover, BulletName.blue, 0.125f, angleRate, 0.05f, 4, 2, 410));
        }

        public IEnumerator Pattern_04(Mover mover)
        {
            const int interval = 5;
            StartCoroutine(AimingLineBullets(mover, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            StartCoroutine(AimingLineBullets(mover, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            StartCoroutine(AimingNWayLineBullets(mover, BulletName.red, 0.02f, interval, 4, 0.125f, 3));
            yield return new WaitForFrames(25);
            StartCoroutine(AimingNWayLineBullets(mover, BulletName.red, 0.02f, interval, 9, 0.125f, 3));
        }


        public IEnumerator Pattern_05(Mover mover)
        {
            const int interval = 5;
            StartCoroutine(AimingLineBullets(mover, BulletName.red, 0.02f, interval, 5));
            yield return new WaitForFrames(50);
            StartCoroutine(AimingNWayLineBullets(mover, BulletName.red, 0.02f, interval, 9, 0.125f, 3));
            yield return new WaitForFrames(50);
            StartCoroutine(RollingAimingBullets(mover, BulletName.red, 0.02f, 18, 0.5f, 1, 2));
        }


        public IEnumerator Pattern_06(Mover mover)
        {
            const float rollingAngleRate = 0.02f;
            const float rollingAngleRange = 0.22f;
            const int rollingRepeatCount = 9;
            const float rollingAngleOffset = (rollingAngleRate * rollingRepeatCount) / 2.0f;
            const int rollingCount = 5;

            // 뿌리기
            RandomSpreadBullet(mover, BulletName.red, 0.2f, 0.02f, 0.02f, 24);

            // 반시계 회전
            yield return new WaitForFrames(100);
            StartCoroutine(RollingNWayBullets(mover, BulletName.blue
                , 0.75f - rollingAngleOffset, rollingAngleRange, rollingAngleRate, 0.02f, rollingCount, 1, 5, rollingRepeatCount));

            // 뿌리기
            yield return new WaitForFrames(140);
            RandomSpreadBullet(mover, BulletName.red, 0.2f, 0.02f, 0.02f, 24);

            // 시계 회전
            yield return new WaitForFrames(100);
            StartCoroutine(RollingNWayBullets(mover, BulletName.blue
                , 0.75f + rollingAngleOffset, rollingAngleRange, -rollingAngleRate, 0.02f, rollingCount, 1, 5, rollingRepeatCount));

        }

        public IEnumerator Pattern_07(Mover mover)
        {
            const float radius = 0.5f;
            const int repeatCount = 4;
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
                DropCircleBullet(mover, BulletName.blue, angle, speed1_Cirlce1, countPerCircle, false, moveDuaraion, stopDuaraion, angle2, speed2);
                yield return new WaitForFrames(placeInterval);
                DropCircleBullet(mover, BulletName.red, angle, speed1_Cirlce2, countPerCircle, true, moveDuaraion, stopDuaraion - placeInterval, angle2, speed2);

                // 마지막 뿌리기 후에는 이동하지 않음
                if (i < repeatCount - 1)
                {
                    yield return new WaitForFrames(60 - placeInterval);
                    Vector2 nextPos = new Vector2(
                        GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinX + radius + 0.1f, GameSystem._Instance._MaxX - radius - 0.1f)
                        , GameSystem._Instance.GetRandomRange(GameSystem._Instance._MinY * 0.2f + radius + 0.1f, GameSystem._Instance._MaxY - radius - 0.1f));
                    StartCoroutine(MoveDamp(mover, nextPos, 30, 0.1f));
                    yield return new WaitForFrames(60);
                }
            }
        }


        private int _patternDPartDuration = 60 * 14;    // 패턴 D의 파트별 지속시간
        public IEnumerator Pattern_08(Mover mover)
        {
            // 안전선 발사
            PatternD_2_SafetyLine();
            // 보스 기준위치로 이동
            yield return StartCoroutine(MoveConstantVelocity(mover, new Vector2(0.0f, 0.75f), 240));
            yield return new WaitForFrames(240);
            // 반원 발사
            const float speed1 = 0.05f;
            const float speed2 = 0.01f;
            const int phase1Duration = 30;
            const int count = 5;
            const float angleRange = 100.0f / 360.0f;
            const float startAngleOffset = (angleRange / (float)(count - 1)) / 2.0f;
            const int interval = 30;
            const string shape = BulletName.blue;

            for (int frame = 0; frame < (_patternDPartDuration / 2); ++frame)
            {
                if (frame % interval == 0)
                {
                    float startAngle = 0.75f + GameSystem._Instance.GetRandomRange(-startAngleOffset, startAngleOffset);
                    for (int i = 0; i < count; ++i)
                    {
                        PlacedBullet b = GameSystem._Instance.CreateBullet<PlacedBullet>();
                        b.InitNoStop(shape, mover._X, mover._Y, startAngle + angleRange * ((float)i / (count - 1) - 0.5f), speed1
                            , phase1Duration, 0.75f, speed2);
                    }
                }
                yield return null;
            }
        }

        // sup
        /// 이후 패턴을 위한 안전선
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
        #endregion //Coroutine
    }
}