using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameplayController : MonoBehaviour {
    public static GameplayController instance = null;

    [SerializeField] int mazeWidth = 10;
    [SerializeField] int mazeHeight = 10;
    [SerializeField] int numberOfWalls = 60;
    [SerializeField] bool generateMazeOnStart;
    [SerializeField] MazeView mazeView;

    [Space]
    [SerializeField] int numberOfDots;
    [SerializeField] float dotRespawnDelay;
    [SerializeField] ObjectPool dotsPool;

    [Space]
    [SerializeField] ObjectPool enemyPool;

    [Space]
    [Header("UI")]
    [SerializeField] GameObject inGameUI;
    [SerializeField] Text scoreText;

    [SerializeField] GameObject gameOverUI;

    private List<Vector2Int> availableDotPositions;

    private int score = 0;

    void Awake() {
        instance = this;
    }

    void Start() {
        InitMaze();
        InitDots();
        InitEnemies();
        InitUI();
    }

    private void InitMaze() {
        if (generateMazeOnStart) {
            MazeBuilder builder = new MazeBuilder();
            builder.mazeWidth = mazeWidth;
            builder.mazeHeight = mazeHeight;
            builder.wallCount = numberOfWalls;
            mazeView.maze = builder.Build();
        }
    }

    private void InitDots() {
        Maze maze = mazeView.maze;

        availableDotPositions = new List<Vector2Int>(maze.width * maze.height);
        for (int x=0; x < maze.width; x++) {
            for (int y=0; y < maze.height; y++) {
                availableDotPositions.Add(new Vector2Int(x, y));
            }
        }
        for (int i=0; i < numberOfDots; i++) {
            SpawnDot();
        }
    }

    private void InitEnemies() {
        Maze maze = mazeView.maze;

        SpawnEnemy(mazeView.CellPosition(0, 0));
        SpawnEnemy(mazeView.CellPosition(maze.width - 1, 0));
        SpawnEnemy(mazeView.CellPosition(0, maze.height - 1));
        SpawnEnemy(mazeView.CellPosition(maze.width - 1, maze.height - 1));
    }

    private void InitUI() {
        score = 0;
        inGameUI.SetActive(true);
        UpdateScoreUI();
        gameOverUI.SetActive(false);
    }

    private void UpdateScoreUI() {
        scoreText.text = score.ToString();
    }

    public void GameOver() {
        Time.timeScale = 0;
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void Restart() {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void CollectDot(Dot dot) {
        availableDotPositions.Add(dot.cell);
        dotsPool.Put(dot.gameObject);

        score++;
        UpdateScoreUI();

        Invoke("SpawnDot", dotRespawnDelay);
    }

    private void SpawnDot() {
        int dotIdx = UnityEngine.Random.Range(0, availableDotPositions.Count);
        Vector2Int dotPosition = availableDotPositions[dotIdx];
        availableDotPositions.RemoveAt(dotIdx);

        GameObject dot = dotsPool.Get();
        dot.GetComponent<Dot>().cell = dotPosition;
        dot.transform.position = mazeView.CellPosition(dotPosition.x, dotPosition.y) + Vector3.up * 0.3f;
        dot.SetActive(true);
    }

    private void SpawnEnemy(Vector3 position) {
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = position;
        enemy.SetActive(true);
    }
}
