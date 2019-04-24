using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Game
{
    public partial class GameSystem : MonoBehaviour
    {
        public static GameInfo _loadGameInfo;
        public static GameSystem _Instance { get; private set; }

        private enum StateType
        {
            Invalid,
            Load,
            Play,
            Result,
        }
        CanvasScaler _CanvasScaler;
        private StateType stateType = StateType.Invalid;
        public Player player { get; private set; }
        List<Player> players { get; set; }
        public void SetPlayer(Player _player)
        {
            player = _player;
            players = new List<Player>();
            if (player != null)
            {
                players.Add(_player);
            }
        }

        public List<TeamShot> _Shots { get; private set; }
        public List<Enemy> _Enemys { get; private set; }
        public List<Bullet> _Bullets { get; private set; }
        public List<Effect> _Effects { get; private set; }

        // UI //////////////////////////
        public UILoading _uiLoading;
        public Layout _layout;
        public RectTransform _LayoutGameArea { get { return _layout._gameArea; } }
        public MoveInputArea _moveInputArea;
        public UIResult _uiResult;

        // 음악별 설정 //////////////////////////
        private GameInfo _beatInfo;
        private BaseGameLogic _logic;

        public Text _scoreBoard;
        private int _score = 0;
        private System.Random _random = null;
        private const int _gameOverResultDelay = 120;

        // Const
        public float _MaxX { get { return 1.0f; } }
        public float _MinX { get { return -1.0f; } }
        public float _MaxY { get { return 1.3f; } }
        public float _MinY { get { return -1.3f; } }
        private float _Width { get { return (_MaxX - _MinX); } }
        private float _Height { get { return (_MaxY - _MinY); } }

        // 기준으로 삼는 화면비
        private const float _refDeviceWidthRatio = 9.0f;
        private const float _refDeviceHeightRatio = 16.0f;

        void Awake()
        {
            _Instance = this;
            _CanvasScaler = FindObjectOfType<CanvasScaler>();
            _Shots = new List<TeamShot>();
            _Enemys = new List<Enemy>();
            _Bullets = new List<Bullet>();
            _Effects = new List<Effect>();
            StartCoroutine(Loading());
        }

        private void OnDestroy()
        {
            _Instance = null;
        }

        void Update()
        {
            switch (stateType)
            {
                case StateType.Play:
                    {
                        UpdatePlay();
                        break;
                    }
            }
        }

        private void SetState(StateType nextState)
        {
            stateType = nextState;
        }

        #region Loading


        private IEnumerator Loading()
        {
            // 상태 변경
            SetState(StateType.Load);

            // 로딩 UI
            _uiLoading.Open();

            InitBeatInfo();
            SetScore(0);
            InitRandom();
            InitCamera();

            // 특화 정보 로딩
            RemoveAllMover();
            yield return StartCoroutine(_logic.LoadContext());

            // 여유 프레임
            _uiLoading.Close();
            yield return null;

            // 로딩 끝
            StartPlay();
        }


        private void InitBeatInfo()
        {
            if (_beatInfo == null)
            {
                _beatInfo = _loadGameInfo;
                if (_beatInfo == null)
                {
                    Debug.LogError("[GameSystem] Invalid BeatInfo");
                    SceneManager.LoadScene(SceneName.beatList);
                }
            }

            if (_logic == null)
            {
                _logic = Activator.CreateInstance(System.Type.GetType("Game." + _beatInfo._namespace + ".GameLogic")) as Game.BaseGameLogic;
                if (_logic == null)
                {
                    Debug.LogError("[GameSystem] Invalid namespcae:" + _beatInfo._namespace);
                }
            }
        }

        private void InitCamera()
        {

            // 기준 기기화면 상단에 가로가 가득차도록 게임영역 배치
            float gameHeightRatio = _refDeviceWidthRatio * (_Height / _Width); // 기기에서 게임화면 높이의 비
            float bottomBoxHeightRatio = _refDeviceHeightRatio - gameHeightRatio;
            float bottomBoxHeightScreenRate = bottomBoxHeightRatio / _refDeviceHeightRatio;
            float refResolutionHeight = _CanvasScaler.referenceResolution.y;
            float refDeviceWidth = refResolutionHeight * (_refDeviceWidthRatio / _refDeviceHeightRatio);

            _layout.Initialize(refDeviceWidth, bottomBoxHeightScreenRate);

            // 메인 카메라를 게임 카메라로 사용
            Camera gameCam = Camera.main;

            // 게임월드높이와 레터박스 월드높이의 합을 camH라고 할 떄
            float camH = (_refDeviceHeightRatio / gameHeightRatio) * _Height;
            gameCam.orthographicSize = camH / 2.0f;

            float diffH = camH - _Height;
            Vector3 camPos = gameCam.transform.position;
            camPos.y = -1.0f * diffH / 2.0f;
            gameCam.transform.position = camPos;

        }
        #endregion // Loading

        public void StartPlay()
        {
            SetState(StateType.Play);
        }

        public void UpdatePlay()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnExitGame();
                return;
            }

            _logic.UpdatePlayContext();

            UpdateMoverList(players);
            UpdateMoverList(_Shots);
            UpdateMoverList(_Enemys);
            UpdateMoverList(_Bullets);
            UpdateMoverList(_Effects);

        }

        /// <summary>
        /// 게임오버 발생 지정
        /// </summary>
        public void SetGameOver()
        {
            DoResult(false);
        }
        public void SetGameWin()
        {
            DoResult(true);
        }

        #region Pool

        private ShapePoolManager shapePoolManager = new ShapePoolManager();
        private MoverPoolManager moverPoolManager = new MoverPoolManager();

        public void PoolStackShape(string subPath, int count)
        {
            shapePoolManager.PoolStack(subPath, count);
        }

        public Shape CreateShape(string subPath)
        {
            return shapePoolManager.Create(subPath);
        }

        public void DeleteShape(Shape shape)
        {
            shapePoolManager.Delete(shape);
        }

        public void PoolStackMover<T>(int count) where T : Mover, new()
        {
            moverPoolManager.PoolStack<T>(count);
        }

        private void UpdateMoverList<T>(List<T> movers) where T : Mover
        {
            if (movers == null)
            {
                return;
            }
            for (int i = 0; i < movers.Count; ++i)
            {
                movers[i].Move();
            }

            for (int i = movers.Count - 1; i >= 0; --i)
            {
                if (!movers[i]._alive)
                {
                    T mover = movers[i];
                    mover.OnDestroy();
                    movers.RemoveAt(i);
                    moverPoolManager.Delete(mover);
                }
            }

            int order = 0;
            for (int i = 0; i < movers.Count; ++i)
            {
                movers[i].Draw(order);
                order += movers[i]._shape._SpriteOrderCount;
            }
        }

        private void RemoveMoverList<T>(List<T> movers) where T : Mover
        {
            if (movers != null)
            {
                for (int i = movers.Count - 1; i >= 0; --i)
                {
                    T mover = movers[i];
                    mover.OnDestroy();
                    moverPoolManager.Delete(mover);
                }
                movers.Clear();
            }
        }

        /// <summary>
        /// </summary>
        private void RemoveAllMover()
        {
            RemoveMoverList(players);
            RemoveMoverList(_Shots);
            RemoveMoverList(_Enemys);
            RemoveMoverList(_Bullets);
            RemoveMoverList(_Effects);
        }
        #endregion //Mover

        #region Fatory Create
        public T CreatePlayer<T>() where T : Player, new()
        {
            T player = moverPoolManager.Create<T>();
            return player;
        }

        public T CreateShot<T>() where T : TeamShot, new()
        {
            T shot = moverPoolManager.Create<T>();
            _Shots.Add(shot);
            return shot;
        }

        public T CreateEnemy<T>() where T : Enemy, new()
        {
            T enemy = moverPoolManager.Create<T>();
            _Enemys.Add(enemy);
            return enemy;
        }

        public T CreateBullet<T>() where T : Bullet, new()
        {
            T bullet = moverPoolManager.Create<T>();
            _Bullets.Add(bullet);
            return bullet;
        }

        public T CreateEffect<T>() where T : Effect, new()
        {
            T effect = moverPoolManager.Create<T>();
            _Effects.Add(effect);
            return effect;
        }
        #endregion

        #region Score
        private void SetScore(int score)
        {
            _score = score;
            _scoreBoard.text = _score.ToString();
        }

        public void SetScoreDelta(int delta)
        {
            SetScore(_score + delta);
        }
        #endregion // Score

        #region Random
        private void InitRandom()
        {
            _random = new System.Random(1);
        }

        public float GetRandom01()
        {
            return (float)_random.NextDouble();
        }

        public float GetRandomRange(float min, float max)
        {
            if (min >= max)
            {
                return min;
            }
            else
            {
                return min + GetRandom01() * (max - min);
            }
        }
        #endregion // Random

        public void OnExitGame()
        {
            SceneManager.LoadScene(SceneName.beatList);
        }


        private void DoResult(bool isWin)
        {
            SetState(StateType.Result);
            RemoveAllMover();

            // Show UI
            _uiResult.SetData(_beatInfo, isWin, _score);
        }



        public T GetLogic<T>() where T : BaseGameLogic
        {
            return _logic as T;
        }

        #region Debug
        private void OnDrawGizmos()
        {
            Vector2 lt = new Vector2(_MinX, _MaxY);
            Vector2 lb = new Vector2(_MinX, _MinY);
            Vector2 rb = new Vector2(_MaxX, _MinY);
            Vector2 rt = new Vector2(_MaxX, _MaxY);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(lt, lb);
            Gizmos.DrawLine(lb, rb);
            Gizmos.DrawLine(rb, rt);
            Gizmos.DrawLine(rt, lt);
        }
        #endregion // Debug


        public Text _txtNote;
        public static void SetNote(string _message)
        {
            _Instance._txtNote.text = _message;
        }
    }
}