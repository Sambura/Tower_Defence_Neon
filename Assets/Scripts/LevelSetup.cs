using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public LineRenderer[] enemyRoutes;
    public int initialMoney = 1000;
    public int initialLifes = 10;
    public List<EnemyWave> enemyWaves;

    private int waveIndex = 0;
    private float nextSpawn;
    private float waveEnd;
    private bool allSpawned;
    private EnemyWave wave;
    private bool waveStarted;
    private List<float> routeLengths;

    private void Awake()
    {
        // Calculate routes' length
        routeLengths = new List<float>();
        foreach (var i in enemyRoutes)
        {
            i.gameObject.SetActive(false);
            float localLength = 0;
            for (int j = 1; j < i.positionCount; j++)
            {
                localLength += Vector2.Distance(i.GetPosition(j), i.GetPosition(j - 1));
            }
            routeLengths.Add(localLength);
        }
        // Initialize all waves
        InitializeWaves();
    }

    public void NextWave()
    {
        if (waveIndex == enemyWaves.Count)
        {
            wave = null;
            Controller.Instance.FinishLevel();
        }
        else
        {
            wave = enemyWaves[waveIndex];
            nextSpawn = Time.time + wave.startDelay;
            allSpawned = false;
            waveIndex++;
            if (wave.startDelay > 0)
            {
                waveStarted = false;
                Controller.Instance.ShowSkipButton(wave.startDelay);
            }
            else waveStarted = true;
        }
    }

    public void SkipDelay()
    {
        if (waveStarted) return;
        waveStarted = true;
        nextSpawn = Time.time;
    }

    private void Update()
    {
        if (wave == null) return;
        while (Time.time >= nextSpawn && !allSpawned)
        {
            waveStarted = true;
            int routesEnded = 0;
            for (var routeIndex = 0; routeIndex < wave.waves.Length; routeIndex++)
            {
                if (wave.waves[routeIndex].enemies.Count == 0)
                {
                    routesEnded++;
                    continue;
                }
                int enemyIndex = Random.Range(0, wave.waves[routeIndex].enemies.Count);
                var spawnPosition = enemyRoutes[routeIndex].GetPosition(0);
                var enemy = Instantiate(wave.waves[routeIndex].enemies[enemyIndex].enemyPrefab, 
                    (Vector2)spawnPosition, Quaternion.identity).GetComponent<Enemy>();
                enemy.Route = enemyRoutes[routeIndex];
                enemy.DistanceToFinish = routeLengths[routeIndex];
                enemy.ListNode = Controller.Instance.SpawnedEnemies.AddLast(enemy);
                wave.waves[routeIndex].enemies[enemyIndex].SpawnCount--;
                if (wave.waves[routeIndex].enemies[enemyIndex].SpawnCount <= 0)
                {
                    wave.waves[routeIndex].enemies.RemoveAt(enemyIndex);
                    if (wave.waves[routeIndex].enemies.Count == 0) routesEnded++;
                }
            }
            nextSpawn += 1 / wave.spawnRate;
            if (routesEnded == wave.waves.Length)
            {
                allSpawned = true;
                waveEnd = Time.time + wave.endDelay;
            }
        }

        if (allSpawned && ((Time.time >= waveEnd && !wave.waitForElimination) || Controller.Instance.SpawnedEnemies.Count == 0)) NextWave();
    }

    private void InitializeWaves()
    {
        // Initialize all waves
        foreach (var wave in enemyWaves)
        {
            // Initialize all routes
            foreach (var routeWave in wave.waves)
            {
                // Calculate each type of enemies count
                int enemiesToSpawn = routeWave.enemiesCount;
                // Calculate overall ratio (needed to calculate each type of enemy count)
                float overallRatio = 0;
                foreach (var i in routeWave.enemies)
                {
                    overallRatio += i.spawnRatio;
                }
                // Calculate enemies' count
                foreach (var i in routeWave.enemies)
                {
                    i.SpawnCount = Mathf.Min(Mathf.RoundToInt(routeWave.enemiesCount * i.spawnRatio / overallRatio), enemiesToSpawn);
                    enemiesToSpawn -= i.SpawnCount;
                }
                // If calculated nuber of enemies less then overall number, randomly distribute remaining enemies
                while (enemiesToSpawn > 0)
                {
                    routeWave.enemies[Random.Range(0, routeWave.enemies.Count)].SpawnCount++;
                    enemiesToSpawn--;
                }
                // In case there are enemies with count == 0, remove them from the list
                for (var i = 0; i < routeWave.enemies.Count; i++)
                {
                    if (routeWave.enemies[i].SpawnCount <= 0)
                    {
                        routeWave.enemies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class EnemyWave
{
    /// <summary>
    /// Count of waves should be equal to the count of routes in the level
    /// </summary>
    public RouteConfig[] waves;

    // Common parameters
    /// <summary>
    /// How many enemies should spawn in a second
    /// </summary>
    public float spawnRate;
    /// <summary>
    /// Delay before first enemy spawns
    /// </summary>
    public float startDelay;
    /// <summary>
    /// After all enemies are spawned, passes endDelay seconds and next wave starts. Next wave also starts immideately after all enemies eliminated
    /// </summary>
    public float endDelay;
    /// <summary>
    /// If set to true, wave will end if and only if all enemies are eliminated
    /// </summary>
    public bool waitForElimination;
}

[System.Serializable]
public class RouteConfig
{
    /// <summary>
    /// Overall enemies count
    /// </summary>
    public int enemiesCount;
    /// <summary>
    /// Configuration for enemies
    /// </summary>
    public List<EnemyConfig> enemies;
}

[System.Serializable]
public class EnemyConfig
{
    public GameObject enemyPrefab;
    public float spawnRatio;

    public int SpawnCount { get; set; }
}