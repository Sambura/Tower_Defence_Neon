﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    public LineRenderer enemyRoute;
    public int initialMoney = 1000;
    public int initialLifes = 10;
    public List<EnemyWave> enemyWaves;

    private int waveIndex = 0;
    private float nextSpawn;
    private float waveEnd;
    private bool allSpawned;
    private EnemyWave wave;
    private bool waveStarted;

    private void Awake()
    {
        enemyRoute.gameObject.SetActive(false);
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
            nextSpawn = Time.time + 1 / wave.spawnRate + wave.startDelay;
            allSpawned = false;
            waveIndex++;
            InitializeWave();
            if (wave.startDelay > 0)
            {
                waveStarted = false;
                Controller.Instance.SetSkipButtonActive(true);
            }
            else waveStarted = true;
        }
    }

    public void SkipDelay()
    {
        waveStarted = true;
        nextSpawn = Time.time;
    }

    private void InitializeWave()
    {
        // Calculate each type of enemies count
        int enemiesToSpawn = wave.enemiesCount;
        float overallRatio = 0;
        foreach (var i in wave.enemies)
        {
            overallRatio += i.spawnRatio;
        }
        foreach (var i in wave.enemies)
        {
            i.SpawnCount = Mathf.Min(Mathf.RoundToInt(wave.enemiesCount * i.spawnRatio / overallRatio), enemiesToSpawn);
            enemiesToSpawn -= i.SpawnCount;
        }

        while (enemiesToSpawn > 0)
        {
            wave.enemies[Random.Range(0, wave.enemies.Count)].SpawnCount++;
            enemiesToSpawn--;
        }

        for (var i = 0; i < wave.enemies.Count; i++)
        {
            if (wave.enemies[i].SpawnCount <= 0)
            {
                wave.enemies.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update()
    {
        if (wave == null) return;
        while (Time.time >= nextSpawn && !allSpawned)
        {
            if (!waveStarted)
            {
                waveStarted = true;
                Controller.Instance.SetSkipButtonActive(false);
            }
            int enemyIndex = Random.Range(0, wave.enemies.Count);
            var enemy = Instantiate(wave.enemies[enemyIndex].enemyPrefab, enemyRoute.GetPosition(0), Quaternion.identity).GetComponent<Enemy>();
            enemy.Route = enemyRoute;
            enemy.ListNode = Controller.Instance.SpawnedEnemies.AddLast(enemy);
            wave.enemies[enemyIndex].SpawnCount--;
            if (wave.enemies[enemyIndex].SpawnCount <= 0) wave.enemies.RemoveAt(enemyIndex);
            nextSpawn += 1 / wave.spawnRate;
            if (wave.enemies.Count == 0)
            {
                allSpawned = true;
                waveEnd = Time.time + wave.endDelay;
            }
        }

        if (allSpawned && ((Time.time >= waveEnd && !wave.waitForElimination) || Controller.Instance.SpawnedEnemies.Count == 0)) NextWave();
    }
}

[System.Serializable]
public class EnemyWave
{
    /// <summary>
    /// Overall enemies count
    /// </summary>
    public int enemiesCount;
    /// <summary>
    /// Configuration for enemies
    /// </summary>
    public List<EnemyConfig> enemies;
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

    public bool Initialized { get; set; }
}

[System.Serializable]
public class EnemyConfig
{
    public GameObject enemyPrefab;
    public float spawnRatio;

    public int SpawnCount { get; set; }
}