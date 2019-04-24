using UnityEngine;
using System.Collections;

namespace Game
{
    namespace Round_00
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

                // 
                CoroutineManager.instance.StopAllCoroutines();
                CoroutineManager.instance.RegisterCoroutine(Main()); 
            }

            // 특화 정보 갱신
            public override void UpdatePlayContext()
            {
                CoroutineManager.instance.UpdateAllCoroutines();
            }

            private IEnumerator Main()
            {
                // 플레이어 생성
                PlayerAlive player = GameSystem._Instance.CreatePlayer<PlayerAlive>();
                player.Init(PlayName.player_black, 0.0f, -0.7f, 0.0f);
                // YO!
                // 보스 생성
                Boss boss = GameSystem._Instance.CreateEnemy<Boss>();
                boss.Init(BossName.boss_blue, 0.0f, GameSystem._Instance._MaxY + 0.1f, 0.0f);

                yield return null;
            }
           
        } // GameLogic
    } //
} // Game