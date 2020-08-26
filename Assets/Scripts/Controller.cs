using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject towerPreviewPrefab;
    [SerializeField] private GameObject circlePrefab;

    [Header("Scene elements")]
    [SerializeField] private LineRenderer enemyRoute;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI lifesText;
    [SerializeField] private TMPro.TextMeshProUGUI killsText;
    [SerializeField] private TMPro.TextMeshProUGUI moneyText;
    [SerializeField] private GameObject gameOverScreen;
    public TowerPopUp popupTowerMenu;

    [Header("Other stuff")]
    public float skill = 1;
    public float initialPeriod = 2;
    public float periodFactor = 1;
    public float minPeriod = 0.2f;
    public int lifesCount = 5;
    public Camera mainCamera;
    public int kills = 0;
    public int initMoney = 150;
    public float removeCostRatio = 0.4f;

    private int _money;
    public int Money 
    {
        get
        {
            return _money;
        }
        set
        {
            _money = value;
            moneyText.text = $"Money: {Money}";
        }
    }

    /// <summary>
    /// List of all alive enemies
    /// </summary>
    public LinkedList<Enemy> SpawnedEnemies { get; set; }
    /// <summary>
    /// Singleton
    /// </summary>
    public static Controller Instance { get; set; }
    /// <summary>
    /// Index of tower that is being build now. -1 if no towers are being build
    /// </summary>
    public int IsBuilding { get; set; } = -1;
    public TowerPreview preview;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SpawnedEnemies = new LinkedList<Enemy>();
        } else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        enemyRoute.enabled = false; // Hide route line
        Money = initMoney; // Set money
        lifesText.text = $"Lifes left: {lifesCount}"; // Display lifes text
        killsText.text = $"Kills: {kills}"; // Display kills
        moneyText.text = $"Money: {Money}"; // Display money
        popupTowerMenu.ClosePopUp(); // Hide popup menu
    }

    public void StartWave()
    {
        StartCoroutine(Spawner(initialPeriod, periodFactor));
    }

    /// <summary>
    /// Spawner spawns enemies
    /// </summary>
    /// <param name="period">Delay between spawns</param>
    /// <param name="factor">Period is being multiplied by this factor after each spawn</param>
    private IEnumerator Spawner(float period, float factor)
    {
        while (true)
        {
            var newEnemy = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)], enemyRoute.GetPosition(0), Quaternion.identity).GetComponent<Enemy>();
            newEnemy.Route = enemyRoute;
            newEnemy.ListNode = SpawnedEnemies.AddLast(newEnemy);
            yield return new WaitForSeconds(period);
            if (period > minPeriod)
                period *= factor;
            else
                period = minPeriod;
            killsText.text = $"Kills: {kills}";
        }
    }

    public HealthBar PlaceHealthBar()
    {
        return Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
    }

    public void EnemyBreakthrough()
    {
        if (lifesCount == 0) return;
        lifesCount--;
        lifesText.text = $"Lifes left: {lifesCount}";
        if (lifesCount == 0)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        bool animationPlayed = false;
        for (float scale = 1; scale > 0.01f; scale -= 0.02f)
        {
            Time.timeScale = scale;
            if (scale < 0.3f && !animationPlayed)
            {
                animationPlayed = true;
                gameOverScreen.SetActive(true);
                gameOverScreen.GetComponent<Animator>().Play("GameOverScreen");
            }
            yield return new WaitForSeconds(0.01f);
        }
        Time.timeScale = 0;
    }
}
