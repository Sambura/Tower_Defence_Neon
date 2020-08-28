using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Sprite[] heartSprites;

    [Header("Scene elements")]
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI lifesText;
    [SerializeField] private TMPro.TextMeshProUGUI moneyText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompleteScreen;
    [SerializeField] private TimedButton skipDelayButton;
    [SerializeField] private Image heartSprite;
    public TowerPopUp popupTowerMenu;

    [Header("Other stuff")]
    public Camera mainCamera;
    public float removeCostRatio = 0.4f;
    public int levelSceneIndex = 1;

    private int lifesCount;

    public event System.Action<int> OnMoneyChanged;
    private int _money;
    private bool smoothMoneyDisplayInRun;
    public int Money 
    {
        get
        {
            return _money;
        }
        set
        {
            var lastMoney = _money;
            _money = value;
            if (!smoothMoneyDisplayInRun)
                StartCoroutine(DisplayMoneySmoothly(lastMoney));
            OnMoneyChanged?.Invoke(value);
        }
    }

    public bool drawDebugLines;

    public float CurrentRemoveRatio { get; set; } = 1;

    /// <summary>
    /// List of all alive enemies
    /// </summary>
    public LinkedList<Enemy> SpawnedEnemies { get; set; }
    /// <summary>
    /// Singleton
    /// </summary>
    public static Controller Instance { get; set; }

    private LevelSetup levelSetup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SpawnedEnemies = new LinkedList<Enemy>();
            SceneManager.LoadScene(levelSceneIndex, LoadSceneMode.Additive); // Load level
        } else
        {
            Destroy(this);
        }
    }

    private IEnumerator DisplayMoneySmoothly(float lastMoney)
    {
        smoothMoneyDisplayInRun = true;
        for (; lastMoney != Money; lastMoney += 0.1f * (Money - lastMoney))
        {
            if (Mathf.Abs(Money - lastMoney) < 0.5f) lastMoney = Money;
            moneyText.text = ((int)lastMoney).ToString();
            yield return new WaitForFixedUpdate();
        }
        smoothMoneyDisplayInRun = false;
    }

    public void ShowSkipButton(float time)
    {
        skipDelayButton.Show(time);
    }

    void Start()
    {
        popupTowerMenu.ClosePopUp(); // Hide popup menu
        levelSetup = GameObject.FindGameObjectWithTag("LevelData").GetComponent<LevelSetup>(); // Get level setup
        Money = levelSetup.initialMoney; // Set money
        lifesCount = levelSetup.initialLifes; // Set lifes
        lifesText.text = lifesCount.ToString(); // Display lifes text
        moneyText.text = Money.ToString(); // Display money
    }

    public void StartWave()
    {
        CurrentRemoveRatio = removeCostRatio;
        levelSetup.NextWave();
    }

    public void SkipDelay()
    {
        levelSetup.SkipDelay();
    }

    public void FinishLevel()
    {
        if (lifesCount == 0) return;
        StartCoroutine(LevelCompleted());
    }

    private IEnumerator LevelCompleted()
    {
        var towers = FindObjectsOfType<Tower>();
        yield return new WaitForSeconds(1);
        for (int i = 0; i < towers.Length; i++)
        {
            Tower.SelectedTower = towers[i];
            TowerManager.Instance.RemoveTower();
            yield return new WaitForSeconds(0.3f);
        }
        levelCompleteScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public HealthBar PlaceHealthBar()
    {
        return Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();
    }

    public void EnemyBreakthrough()
    {
        if (lifesCount == 0) return;
        lifesCount--;
        lifesText.text = lifesCount.ToString();
        if (lifesCount > levelSetup.initialLifes * 0.4f) heartSprite.sprite = heartSprites[1]; else heartSprite.sprite = heartSprites[2];

        if (lifesCount == 0)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        bool animationPlayed = false;
        skipDelayButton.Close();
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
