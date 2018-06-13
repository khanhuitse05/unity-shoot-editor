using UnityEngine;
using System.Collections;

namespace Game
{
    namespace DemoBeatInfo
    {
        // "이에반 폴카" 진행정보
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
                GameSystem._Instance.PoolStackShape(BossName.blue, 1);
                GameSystem._Instance.PoolStackMover<Boss>(1);
                GameSystem._Instance.PoolStackShape(BossEffectName.blue, 1);
                GameSystem._Instance.PoolStackMover<Effect>(1);

                // 탄 로딩 ///////////////////
                // 외양 로딩
                GameSystem._Instance._UILoading.SetProgress("Loading Bullets");
                yield return null;
                GameSystem._Instance.PoolStackShape(BulletName.blue, 270);
                GameSystem._Instance.PoolStackShape(BulletName.red, 27);
                GameSystem._Instance.PoolStackMover<Bullet>(270);

                // UI
                GameSystem._Instance._MoveInputArea.SetVisible(false);

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
                // YO!
                // 보스 생성
                Boss boss = GameSystem._Instance.CreateEnemy<Boss>();
                boss.Init(BossName.blue, 0.0f, GameSystem._Instance._MaxY + 0.1f, 0.0f);

                yield return null;
            }
           
        } // GameLogic
    } // Ievan Polkka
} // Game