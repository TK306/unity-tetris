using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Core
{
    public class TetrisGrid : MonoBehaviour
    {
        [SerializeField]
        Vector2Int _gridSize = new Vector2Int(10, 20);
        // [SerializeField]
        // float _gridWidth = 1f;
        Cube[,] _gridArray;
        int _dropLineIndex;

        public int LineCount { get; private set; }

        List<GameObject> _destroyCollection;
        Mode _mode = Mode.Idle;
        enum Mode
        {
            Idle,
            CheckLine,
            LineDrop
        }

        // public float GridWidth
        // {
        //     get
        //     {
        //         return _gridWidth;
        //     }
        // }

        public void CheckLines()
        {
            _mode = Mode.CheckLine;
        }

        public void Reset()
        {
            _gridArray = new Cube[_gridSize.x, _gridSize.y];
            _destroyCollection = new List<GameObject>();
            LineCount = 0;
            foreach (Transform block in this.gameObject.transform.Find("Blocks"))
            {
                Destroy(block.gameObject);
            }
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            Vector3 output = this.gameObject.transform.TransformPoint(new Vector3(gridPos.x, -gridPos.y, 0f));
            return output;
        }

        public bool Get(int x, int y)
        {
            if (x < 0 || x > _gridSize.x - 1 || y < 0 || y > _gridSize.y - 1)
            {
                return true;
            }
            return _gridArray[x, y] != null;
        }

        public void Set(int x, int y, Cube cube)
        {
            if (x < 0 || x > _gridSize.x - 1 || y < 0 || y > _gridSize.y - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            _gridArray[x, y] = cube;
        }

        void Update()
        {
            switch (_mode)
            {
                case Mode.Idle:
                    _dropLineIndex = -1;
                    break;
                case Mode.CheckLine:
                    _dropLineIndex = -1;
                    CheckLineLoop();
                    break;
                case Mode.LineDrop:
                    LineDropLoop();
                    break;
                default:
                    break;
            }
        }

        void CheckLineLoop()
        {
            for (int j = _gridArray.GetLength(1) - 1; j >= 0; j--)
            {
                bool next = false;
                for (int i = 0; i < _gridArray.GetLength(0); i++)
                {
                    if (_gridArray[i, j] == null)
                    {
                        next = true;
                        break;
                    }
                };
                if (next)
                {
                    continue;
                }
                _dropLineIndex = j;
                LineCount += 1;
                for (int i = 0; i < _gridArray.GetLength(0); i++)
                {
                    if (_gridArray[i, j] != null)
                    {
                        Destroy(_gridArray[i, j].gameObject);
                        _gridArray[i, j] = null;
                    }
                };
                _mode = Mode.LineDrop;
                return;
            }
            _mode = Mode.Idle;
        }

        void LineDropLoop()
        {
            for (int j = _dropLineIndex; j >= 0; j--)
            {
                for (int i = 0; i < _gridArray.GetLength(0); i++)
                {
                    if (j == 0)
                    {
                        _gridArray[i, j] = null;
                    }
                    else
                    {
                        Cube upCube = _gridArray[i, j - 1];
                        if (upCube != null)
                        {
                            upCube.MoveDown();
                        }
                        _gridArray[i, j] = _gridArray[i, j - 1];
                    }
                }
            }
            _mode = Mode.CheckLine;
        }
    }
}
