﻿using UnityEngine;
using System.Collections;

namespace Game
{
    namespace TurkishMarch
    {
        // "터키 행진곡" 진행정보
        public class GameLogic : Game.BaseGameLogic
        {

            // 특화 정보 로딩
            public override IEnumerator LoadContext()
            {
                IEnumerator loadPlayer = LoadBasicPlayer();
                while (loadPlayer.MoveNext())
                {
                    yield return loadPlayer.Current;
                }

                // 적기 로딩 /////////////////////
                GameSystem._Instance.PoolStackShape(BossName.red, 1);
                GameSystem._Instance.PoolStackMover<Boss>(1);
                GameSystem._Instance.PoolStackShape(BossEffectName.red, 1);
                GameSystem._Instance.PoolStackMover<Effect>(1);

                // 탄 로딩 ///////////////////
                // 외양 로딩
                GameSystem._Instance._UILoading.SetProgress("Loading Bullets 1/3");
                yield return null;
                GameSystem._Instance.PoolStackShape(BulletName.red, 41);
                GameSystem._Instance.PoolStackShape(BulletName.blue, 86);
                GameSystem._Instance.PoolStackShape(BulletName.blueLarge, 2);
                GameSystem._Instance.PoolStackShape(BulletName.redLarge, 2);
                GameSystem._Instance._UILoading.SetProgress("Loading Bullets 2/3");
                yield return null;
                GameSystem._Instance.PoolStackShape(BulletName.blueSmall, 90);
                GameSystem._Instance._UILoading.SetProgress("Loading Bullets 3/3");
                yield return null;
                GameSystem._Instance.PoolStackShape(BulletName.redSmall, 114);
                
                // 클래스 로딩
                GameSystem._Instance.PoolStackMover<Bullet>(60);
                GameSystem._Instance.PoolStackMover<AwayBullet>(86);
                GameSystem._Instance.PoolStackMover<PlacedBullet>(160);
                GameSystem._Instance.PoolStackMover<SpiralPlacedShooterBullet>(4);

                // 코루틴
                _coroutineManager.StopAllCoroutines();
                _coroutineManager.RegisterCoroutine(Main());    // 메인 코루틴 등록
            }

            // 특화 정보 갱신
            public override void UpdatePlayContext()
            {
                _coroutineManager.UpdateAllCoroutines();
            }

            private IEnumerator Main()
            {
                // 플레이어 생성
                PlayerAlive player = GameSystem._Instance.CreatePlayer<PlayerAlive>();
                player.Init(PlayName.black, 0.0f, -0.7f, 0.0f);

                // 보스 생성
                Boss boss = GameSystem._Instance.CreateEnemy<Boss>();
                boss.Init(BossName.red, -0.5f, 1.3f, 0.0f);

                yield return null;
            }
        } // GameLogic
    } // TurkishMarch
} // Game