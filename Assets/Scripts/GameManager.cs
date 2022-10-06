using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI scoreTextGO;
    [SerializeField] Animator gameoverAnimator;
    [SerializeField] IslandGenerator islandGenerator;


    List<Enemy> spawnedEnemies;
    public Camera mainCamera;
    public Player player;

    float spawnTimer;
    int score;
    float enemySpawn;

    private void Awake() {
        Instance = this;
        spawnedEnemies = new List<Enemy>();
        enemySpawn = PlayerPrefs.GetFloat("EnemySpawn");

    }

    public void IncreaseScore(int amount) {
        score += amount;
        scoreText.text = "Score: " + score;
        scoreTextGO.text = "Score: " + score;
    }

    public void GameOver() {
        gameoverAnimator.Play("Open", 0, 0);
    }


    private void Update() {
        spawnTimer += Time.deltaTime;
        if(spawnTimer > enemySpawn) {
            spawnTimer -= enemySpawn;
            SpawnEnemy();
        }

        spawnedEnemies = spawnedEnemies.Where(e => e != null).ToList();
    }

    public List<Enemy> GetSpawnedEnemies() {
        return spawnedEnemies;
    }

    public void SpawnEnemy() {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector2 viewportPosition = GenerateOutsideViewport();
        Vector2 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

        while(islandGenerator.PerlinValue(worldPosition) > 0.5f) {
            viewportPosition = GenerateOutsideViewport();
            worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);
        }

        Vector2 directionToPlayer = ((Vector2)player.transform.position - worldPosition).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        spawnedEnemies.Add(Instantiate(enemyPrefab, worldPosition, Quaternion.Euler(0, 0, angle)).GetComponent<Enemy>());
    }

    public Vector2 GenerateOutsideViewport() {
        Vector2 viewportPosition = new Vector2(
                Random.Range(-0.5f, 1.5f),
                Random.Range(-0.5f, 1.5f)
            );

        while (new Rect(0, 0, 1, 1).Contains(viewportPosition)) {
            viewportPosition = new Vector2(
                Random.Range(-0.5f, 1.5f),
                Random.Range(-0.5f, 1.5f)
            );
        }

        return viewportPosition;
    }
}
