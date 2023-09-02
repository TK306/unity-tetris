using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Core
{
    public class Cube : MonoBehaviour
    {
        [SerializeField]
        Vector2Int _gridPos;
        TetrisGrid _grid;
        public Vector2Int GridPos
        {
            get
            {
                return _gridPos;
            }
            set
            {
                Debug.Log($"Set Cube Pos {value}");
                _gridPos = value;
                Vector3 localPos = _grid.GridToWorld(_gridPos);
                this.gameObject.transform.position = localPos;
            }
        }

        public void InjectGrid(TetrisGrid grid)
        {
            _grid = grid;
        }

        public void MoveDown()
        {
            GridPos += new Vector2Int(0, 1);
        }
    }
}
