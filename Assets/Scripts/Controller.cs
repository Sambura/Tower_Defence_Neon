using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] towerPrefabs;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject healthBarPrefab;

    [Header("Scene elements")]
    [SerializeField] private Button[] towerButtons;
    [SerializeField] private LineRenderer enemyRoute;
    [SerializeField] private GameObject buildPoints;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI lifesText;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Controller settings")]
    [SerializeField] private Color defaultButtonColor;
    [SerializeField] private Color pressedButtonColor;

    [Header("Other stuff")]
    public float skill = 1;
    public float initialPeriod = 2;
    public float periodFactor = 1;
    public int lifesCount = 5;

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

    public void TowerButtonPressed(int towerIndex)
    {
        if (IsBuilding == -1) { // If not building, highlight button and set building index
            IsBuilding = towerIndex;
            var image = towerButtons[IsBuilding].GetComponent<Image>();
            image.color = pressedButtonColor;
            buildPoints.SetActive(true);
        } else // If already building
        {
            var image = towerButtons[IsBuilding].GetComponent<Image>(); // Unhighlight previous button (we should do it anyways)
            image.color = defaultButtonColor;
            bool sameButton = towerIndex == IsBuilding; // If we pressed the same button, we should stop building, else we should change building index
            IsBuilding = -1;
            if (!sameButton) // If we pressed not the same button, start building
                TowerButtonPressed(towerIndex);
            else
                buildPoints.SetActive(false);
        }
    }

    public void BuildTower(GameObject point)
    {
        point.SetActive(false);
        var tower = Instantiate(towerPrefabs[IsBuilding], point.transform.position, Quaternion.identity);
        tower.GetComponent<Tower>().point = point;
        var image = towerButtons[IsBuilding].GetComponent<Image>();
        image.color = defaultButtonColor;
        IsBuilding = -1;
        buildPoints.SetActive(false);
    }

    void Start()
    {
        foreach (var i in towerButtons) // Setting all buttons to defaut color
        {
            i.GetComponent<Image>().color = defaultButtonColor;
        }
        buildPoints.SetActive(false); // Hiding all building points
        enemyRoute.enabled = false; // Hide route line
        lifesText.text = $"Lifes left: {lifesCount}"; // Display lifes text
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
            var newEnemy = Instantiate(enemyPrefab, enemyRoute.GetPosition(0), Quaternion.identity).GetComponent<Enemy>();
            newEnemy.Route = enemyRoute;
            newEnemy.ListNode = SpawnedEnemies.AddLast(newEnemy);
            yield return new WaitForSeconds(period);
            period *= factor;
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
