using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
[RequireComponent(typeof(NavMeshSurface))]
public class MazeView : MonoBehaviour {
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;

    protected Vector2 cellSize;
    protected Vector3 cellOffset;

    [HideInInspector]
    [SerializeField] private Maze _maze;
    public Maze maze {
        get { return _maze; }
        set { _maze = value; UpdateMaze(); }
    }

    void OnEnable() {        
        UpdateMaze();
    }

    private void UpdateMaze() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        if (maze is null)
            return;

        Vector3 floorPrefabSize = GetGameObjectBounds(floorPrefab).size;
        cellSize = new Vector2(floorPrefabSize.x, floorPrefabSize.z);
        cellOffset = new Vector3(-cellSize.x * (maze.width - 1) / 2f, 0, -cellSize.y * (maze.height - 1) / 2f);
        
        // Horizontal maze boundaries
        for (int x=0; x < maze.width; x++) {
            AddHorizontalWall(CellPosition(x, -1));
            AddHorizontalWall(CellPosition(x, maze.height-1));
        }

        // Vertical maze boundaries
        for (int y=0; y < maze.height; y++) {
            AddVerticalWall(CellPosition(-1, y));
            AddVerticalWall(CellPosition(maze.width-1, y));
        }

        // Maze floor
        for (int x=0; x < maze.width; x++) {
            for (int y=0; y < maze.height; y++) {
                AddFloor(CellPosition(x, y));
            }
        }

        // Inner maze walls
        for (int x=0; x < maze.width; x++) {
            for (int y=0; y < maze.height; y++) {
                if (maze.cells[x, y] == (Maze.Passage.Right | Maze.Passage.Top))
                    continue;

                Vector3 cellPosition = CellPosition(x, y);

                if (x < maze.width - 1 && (maze.cells[x, y] & Maze.Passage.Right) == Maze.Passage.None) {
                    AddVerticalWall(cellPosition);
                }
                
                if (y < maze.height - 1 && (maze.cells[x, y] & Maze.Passage.Top) == Maze.Passage.None) {
                    AddHorizontalWall(cellPosition);
                }
            }
        }

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public Vector3 CellPosition(int x, int y) {
        return cellOffset + Vector3.right * cellSize.x * x + Vector3.forward * cellSize.y * y;
    }

    private void AddFloor(Vector3 cellPosition) {
        Instantiate(floorPrefab, cellPosition, Quaternion.identity, transform);
    }

    private void AddHorizontalWall(Vector3 cellPosition) {
        Instantiate(wallPrefab, cellPosition + Vector3.forward * cellSize.y / 2f, Quaternion.identity, transform);
    }

    private void AddVerticalWall(Vector3 cellPosition) {
        Instantiate(wallPrefab, cellPosition + Vector3.right * cellSize.x / 2f, Quaternion.AngleAxis(90, Vector3.up), transform);
    }

    private Bounds GetGameObjectBounds(GameObject obj) {
        Renderer renderer = obj.GetComponent<Renderer>();
        Bounds bounds = renderer?.bounds ?? new Bounds();
        foreach (Transform child in obj.transform) {
            bounds.Encapsulate(GetGameObjectBounds(child.gameObject));
        }
        return bounds;
    }
}
