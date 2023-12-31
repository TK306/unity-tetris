using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tetris.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        BlockBase[] _blockArray;
        [SerializeField]
        TetrisGrid _grid;
        [SerializeField]
        Transform _nextBlockPoint;
        [SerializeField]
        Transform _stackedBlockPoint;
        InputAction _moveRightAction;
        InputAction _moveLeftAction;
        InputAction _rotateAction;
        InputAction _fallAction;
        InputAction _stackAction;
        [SerializeField]
        float _tps = 1f;
        int _score;
        int _level;
        DateTime _preTickTime;
        DateTime _preInputTickTime;
        [SerializeField]
        BlockBase _nextBlock;
        [SerializeField]
        BlockBase _movingBlock;
        [SerializeField]
        BlockBase _stackedBlock;
        bool _isFalling;
        int _tickCount;
        bool _moveRightInput;
        bool _moveLeftInput;
        [SerializeField]
        float _inputTps = 10f;

        public bool IsPlaying { get; private set; } = false;

        public int Level
        {
            get { return _level; }
        }

        public int Score
        {
            get { return _score; }
        }

        void Awake()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            _moveRightAction = playerInput.currentActionMap.FindAction("MoveRight");
            _moveLeftAction = playerInput.currentActionMap.FindAction("MoveLeft");
            _rotateAction = playerInput.currentActionMap.FindAction("Rotate");
            _fallAction = playerInput.currentActionMap.FindAction("Fall");
            _stackAction = playerInput.currentActionMap.FindAction("Stack");
        }

        void Start()
        {
            Debug.Log("Start");
        }

        [ContextMenu("StartGame")]
        public void StartGame()
        {
            IsPlaying = true;
            InitializeGame();
            PrepareNextBlock();
        }

        void Update()
        {
            if (!IsPlaying)
            {
                return;
            }

            Input();
            if ((DateTime.Now - _preInputTickTime).TotalMilliseconds > 1000.0f / _inputTps)
            {
                InputTick();
            }

            UpdateTPS();

            FallUpdate();
            if ((DateTime.Now - _preTickTime).TotalMilliseconds > 1000.0f / _tps)
            {
                _preTickTime = DateTime.Now;
                Tick();
            }
        }

        void UpdateTPS()
        {
            float minTps = 1f;
            float maxTps = 5f;
            float maxLine = 100;
            _score = _grid.LineCount;
            _tps = (float)_grid.LineCount * (maxTps - minTps) / maxLine + minTps;
            _level = (int)Mathf.Floor((_tps - 1) * 10) + 1;
        }

        void Tick()
        {
            _tickCount += 1;
            Debug.Log($"Tick {_tickCount}");
            if (_movingBlock == null)
            {
                GenerateNewBlock();
            }
            else
            {
                if (CheckMoveable(new Vector2Int(0, 1)))
                {
                    _movingBlock.MoveDown();
                }
                else
                {
                    SetMovingToCube(_movingBlock, false);
                    FillGrid();
                    _movingBlock = null;
                }
            }
        }

        void InputTick()
        {
            _preInputTickTime = DateTime.Now;
            if (_moveRightInput)
            {
                if (CheckMoveable(new Vector2Int(1, 0)))
                {
                    Debug.Log("Right");
                    _movingBlock.MoveRight();
                }
                else
                {
                    Debug.Log("RightEdge");
                }
            }

            if (_moveLeftInput)
            {
                if (CheckMoveable(new Vector2Int(-1, 0)))
                {
                    Debug.Log("Left");
                    _movingBlock.MoveLeft();
                }
                else
                {
                    Debug.Log("LeftEdge");
                }
            }
        }

        void Input()
        {
            if (_moveRightAction.WasPressedThisFrame())
            {
                _moveRightInput = true;
                _moveLeftInput = false;
                InputTick();
            }

            if (_moveRightAction.WasReleasedThisFrame())
            {
                _moveRightInput = false;
            }

            if (_moveLeftAction.WasPressedThisFrame())
            {
                _moveRightInput = false;
                _moveLeftInput = true;
                InputTick();
            }

            if (_moveLeftAction.WasReleasedThisFrame())
            {
                _moveLeftInput = false;
            }

            if (_fallAction.WasPressedThisFrame())
            {
                Fall();
            }

            if (_rotateAction.WasPressedThisFrame())
            {
                if (CheckRotatable())
                {
                    _movingBlock.Rotate();
                }
                else
                {
                    Debug.Log("Can not Rotate");
                }
            }

            if (_stackAction.WasPressedThisFrame())
            {
                StackBlock();
            }
        }

        void Fall()
        {
            _isFalling = true;
        }

        void StackBlock()
        {
            if (_stackedBlock == null)
            {
                _stackedBlock = _movingBlock;
                SetMovingToCube(_stackedBlock, false);
                _movingBlock = null;
                GenerateNewBlock();
            }
            else
            {
                BlockBase buff = _movingBlock;
                _movingBlock = _stackedBlock;
                SetMovingToCube(_movingBlock, true);
                _stackedBlock = buff;
                SetMovingToCube(_stackedBlock, false);
                _movingBlock.InitialGridPos(buff.GridPos);
                Debug.Log($"Stacked: {_stackedBlock.name}");
                Debug.Log($"Moving: {_movingBlock.name}");
            }

            _stackedBlock.Stack(_stackedBlockPoint.position);
        }

        void FallUpdate()
        {
            if (!_isFalling)
            {
                return;
            }
            if (CheckMoveable(new Vector2Int(0, 1)))
            {
                _movingBlock.MoveDown();
            }
            else
            {
                FillGrid();
                _isFalling = false;
                SetMovingToCube(_movingBlock, false);
                _movingBlock = null;
            }
        }

        void GenerateNewBlock()
        {
            Debug.Log("GenerateNewBlock");
            _movingBlock = _nextBlock;
            SetMovingToCube(_movingBlock, true);
            _nextBlock = null;
            _movingBlock.gameObject.transform.position = new Vector3(0, 0, 0);
            _movingBlock.InitialGridPos(new Vector2Int(5, 0));
            PrepareNextBlock();
        }

        void PrepareNextBlock()
        {
            int i = UnityEngine.Random.Range(0, _blockArray.Length);
            _nextBlock = Instantiate(_blockArray[i], _nextBlockPoint.position, Quaternion.identity);
            _nextBlock.gameObject.transform.parent = _grid.gameObject.transform.Find("Blocks");
            _nextBlock.InjectGrid(_grid);
        }

        bool CheckMoveable(Vector2Int targetPos)
        {
            if (_movingBlock == null)
            {
                return false;
            }
            foreach (Vector2Int block in _movingBlock.Shape)
            {
                Vector2Int nextPos = _movingBlock.GridPos + targetPos + block;
                if (_grid.Get(nextPos.x, nextPos.y))
                {
                    return false;
                }
            }
            return true;
        }

        static void SetMovingToCube(BlockBase block, bool isMoving)
        {
            foreach (Cube cube in block.Cubes)
                if (cube != null)
                    cube.IsMoving = isMoving;
        }

        bool CheckRotatable()
        {
            if (_movingBlock == null)
            {
                return false;
            }
            foreach (Vector2Int rotatedBlock in _movingBlock.GetRotatedShape())
            {
                Vector2Int rotatedPos = rotatedBlock + _movingBlock.GridPos;
                if (_grid.Get(rotatedPos.x, rotatedPos.y))
                {
                    Debug.Log(rotatedPos);
                    return false;
                }
            }
            return true;
        }

        void GameOver()
        {
            IsPlaying = false;
        }

        void InitializeGame()
        {
            _preTickTime = DateTime.Now;
            _tickCount = 0;
            _grid.Reset();
        }

        void FillGrid()
        {
            if (_movingBlock == null)
            {
                return;
            }
            Debug.Log("Fill");
            foreach (Vector2Int block in _movingBlock.Shape)
            {
                Vector2Int pos = _movingBlock.GridPos + block;
                try
                {
                    _grid.Set(pos.x, pos.y, _movingBlock.GetCube(block));
                }
                catch (ArgumentOutOfRangeException)
                {
                    GameOver();
                }
            }
            _grid.CheckLines();
        }

        void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}
