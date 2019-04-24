using UnityEngine;
using System.Collections;

namespace Game
{
    namespace Demo
    {
        public class GameLogic : Game.BaseGameLogic
        {
            public override IEnumerator LoadContext()
            {
                IEnumerator loadPlayer = LoadBasicPlayer();
                while (loadPlayer.MoveNext())
                {
                    yield return loadPlayer.Current;
                }

                CoroutineManager.instance.StopAllCoroutines();
                CoroutineManager.instance.RegisterCoroutine(Main()); 
            }

            public override void UpdatePlayContext()
            {
                CoroutineManager.instance.UpdateAllCoroutines();
            }

            private IEnumerator Main()
            {
                PlayerAlive player = GameSystem._Instance.CreatePlayer<PlayerAlive>();
                player.Init(PlayName.player_black, 0.0f, -0.7f, 0.0f);
                // YO!
                Boss boss = GameSystem._Instance.CreateEnemy<Boss>();
                boss.Init(BossName.boss_blue, 0.0f, GameSystem._Instance._MaxY + 0.1f, 0.0f);
                //
                EditorSystem editorSystem = Object.FindObjectOfType<EditorSystem>();
                editorSystem.setBoss(boss);
                yield return null;
            }
        }
    }
}