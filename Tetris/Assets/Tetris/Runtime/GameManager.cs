using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tetris
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
        DateTime _preTickTime;
        BlockBase _nextBlock;
        BlockBase _movingBlock;
        BlockBase _stackedBlock;
        bool _isFalling;
        int _tickCount;

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
            _preTickTime = DateTime.Now;
            _tickCount = 0;
            PrepareNextBlock();
        }

        void Update()
        {
            Input();
            Level();

            FallUpdate();
            if ((DateTime.Now - _preTickTime).TotalMilliseconds > 1000.0f / _tps)
            {
                _preTickTime = DateTime.Now;
                Tick();
            }
        }

        void Level()
        {
            float minTps = 1f;
            float maxTps = 5f;
            float maxLine = 100;
            _tps = (float)_grid.LineCount * (maxTps - minTps) / maxLine + minTps;
            Debug.Log($"LEVEL: {_tps}");
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
                    FillGrid();
                    _movingBlock = null;
                }
            }
        }

        void Input()
        {
            if (_moveRightAction.WasPressedThisFrame())
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

            if (_moveLeftAction.WasPressedThisFrame())
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
                _movingBlock = null;
                GenerateNewBlock();
            }
            else
            {
                BlockBase buff = _movingBlock;
                _movingBlock = _stackedBlock;
                _stackedBlock = buff;
                _movingBlock.InitialGridPos(buff.GridPos);
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
                _movingBlock = null;
            }
        }

        void GenerateNewBlock()
        {
            Debug.Log("GenerateNewBlock");
            _movingBlock = _nextBlock;
            _nextBlock = null;
            _movingBlock.gameObject.transform.position = new Vector3(0, 0, 0);
            _movingBlock.InitialGridPos(new Vector2Int(5, 0));
            PrepareNextBlock();
        }

        void PrepareNextBlock()
        {
            int i = UnityEngine.Random.Range(0, _blockArray.Length);
            _nextBlock = Instantiate(_blockArray[i], _nextBlockPoint.position, Quaternion.identity);
            _nextBlock.gameObject.transform.parent = _grid.gameObject.transform;
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
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
    }
}
