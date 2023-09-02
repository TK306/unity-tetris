using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Core
{
    public class BlockBase : MonoBehaviour
    {
        public Vector2Int[] Shape = new Vector2Int[1] { new Vector2Int(0, 0) };
        public Cube[] Cubes;

        TetrisGrid _grid;
        [SerializeField]
        Vector2Int _gridPos;

        public Vector2Int GridPos
        {
            get
            {
                return _gridPos;
            }
            private set
            {
                _gridPos = value;
                Debug.Log($"Set {this.gameObject.name} Pos {_gridPos}");
                for (int i = 0; i < Shape.Length; i++)
                {
                    Cubes[i].GridPos = _gridPos + Shape[i];
                }
                // this.gameObject.transform.position = _grid.GridToWorld(_gridPos);
            }
        }

        public void InjectGrid(TetrisGrid grid)
        {
            _grid = grid;
            foreach (Cube cube in Cubes)
            {
                cube.InjectGrid(grid);
            }
        }

        public Cube GetCube(Vector2Int pos)
        {
            int index = System.Array.IndexOf(Shape, pos);
            return Cubes[index];
        }

        public void InitialGridPos(Vector2Int gridPos)
        {
            GridPos = gridPos;
        }

        void Update()
        {
            if (SuisideCheck())
            {
                Destroy(this.gameObject);
            }
        }

        bool SuisideCheck()
        {
            foreach (Cube cube in Cubes)
            {
                if (cube != null)
                {
                    return false;
                }
            }
            return true;
        }

        public void MoveDown()
        {
            GridPos += new Vector2Int(0, 1);
        }

        public void MoveLeft()
        {
            GridPos += new Vector2Int(-1, 0);
        }

        public void MoveRight()
        {
            GridPos += new Vector2Int(1, 0);
        }

        public Vector2Int[] GetRotatedShape()
        {
            Vector2Int[] output = new Vector2Int[Shape.Length];
            float[,] rotMatrix = new float[2, 2] { { 0f, -1f }, { 1f, 0f } };
            for (int i = 0; i < Shape.Length; i++)
            {
                output[i] = Vector2Int.RoundToInt(new Vector2(rotMatrix[0, 0] * Shape[i].x + rotMatrix[1, 0] * Shape[i].y, rotMatrix[0, 1] * Shape[i].x + rotMatrix[1, 1] * Shape[i].y));
            }
            return output;
        }

        public void Rotate()
        {
            Debug.Log("Rotate");
            float[,] rotMatrix = new float[2, 2] { { 0f, -1f }, { 1f, 0f } };
            Shape = GetRotatedShape();
            for (int i = 0; i < Shape.Length; i++)
            {
                Cubes[i].GridPos = Shape[i] + GridPos;
            }
        }

        public void Stack(Vector3 stackPos)
        {
            Debug.Log("Stack");
            GridPos = new Vector2Int(0, 0);
            int index = System.Array.IndexOf(Shape, new Vector2Int(0, 0));
            Vector3 refPosition = Cubes[index].gameObject.transform.position;
            for (int i = 0; i < Cubes.Length; i++)
            {
                Cubes[i].gameObject.transform.position -= refPosition;
            }
            this.gameObject.transform.position = stackPos;
        }
    }
}
