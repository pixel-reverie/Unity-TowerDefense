using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

public class GameCore : MonoBehaviour
{
    #region Serialised Variables
    [Header("~Scene References~")]
    [SerializeField]
    private GameObject startMenuPanel;
    [SerializeField]
    private GameObject playingPanel;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject scoreboardPanel;
    [SerializeField]
    private ScoreboardManager scoreboardManager;

    [SerializeField]
    private WaveDatabase waveDatabase;

    [SerializeField]
    private List<TowerSlot> activeTowerSlots = new List<TowerSlot>();
    [SerializeField]
    private GridLayoutGroup towerButtonParent;
    [SerializeField]
    private GameObject towerPreview;
    [SerializeField]
    private MeshRenderer towerPreviewMeshRenderer;
    [SerializeField]
    private Button SpawnButton;

    [SerializeField]
    private Text moneyText;
    [SerializeField]
    private Text lifeText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text gameOverScoreText;

    [Header("~Prefab References~")]
    [SerializeField]
    private List<Tower> towerPrefabs = new List<Tower>();
    [SerializeField]
    private Enemy enemyPrefab;
    [SerializeField]
    private TowerButton towerButtonPrefab;

    [Header("~Options~")]
    [SerializeField]
    private int startingMoney = 1;
    [SerializeField]
    private int startingLives = 3;
    [SerializeField]
    private Color towerPreviewDefaultColour;
    [SerializeField]
    private Color towerPreviewBlockedColour;
    #endregion

    #region Self Assigned Variables
    private List<TowerButton> towerButtons = new List<TowerButton>();
    private EnemyWave activeEnemyWave;
    #endregion

    public void Start()
    {
        ChangeState(GameFlowState.StartMenu);
        Screen.SetResolution(1000, 500, false);
    }

    #region Game Flow Management
    enum GameFlowState { StartMenu, Playing, GameOver, ScoreBoard }
    GameFlowState currentGameFlowState = GameFlowState.StartMenu;

    #region Public State Change Methods
    public void StartGame()
    {
        ChangeState(GameFlowState.Playing);
    }

    public void GameOver()
    {
        ChangeState(GameFlowState.GameOver);
    }

    public void GoToMenu()
    {
        ChangeState(GameFlowState.StartMenu);
    }

    public void DisplayScoreBoard()
    {
        ChangeState(GameFlowState.ScoreBoard);
    }
    #endregion

    private void ChangeState(GameFlowState newState)
    {
        switch (currentGameFlowState)
        {
            case GameFlowState.StartMenu:
                ExitStartMenuState();
                break;
            case GameFlowState.Playing:
                ExitPlayingState();
                break;
            case GameFlowState.GameOver:
                ExitGameOverState();
                break;
            case GameFlowState.ScoreBoard:
                ExitScoreBoardState();
                break;
            default:
                break;
        }
        switch (newState)
        {
            case GameFlowState.StartMenu:
                EnterStartMenuState();
                break;
            case GameFlowState.Playing:
                EnterPlayingState();
                break;
            case GameFlowState.GameOver:
                EnterGameOverState();
                break;
            case GameFlowState.ScoreBoard:
                EnterScoreBoardState();
                break;
            default:
                break;
        }
    }


    private void EnterStartMenuState()
    {
        currentGameFlowState = GameFlowState.StartMenu;
        startMenuPanel.gameObject.SetActive(true);
    }
    private void ExitStartMenuState()
    {
        startMenuPanel.gameObject.SetActive(false);
    }
    private void EnterPlayingState()
    {
        currentGameFlowState = GameFlowState.Playing;
        playingPanel.gameObject.SetActive(true);
        this.Initialise();
    }
    private void ExitPlayingState()
    {
        playingPanel.gameObject.SetActive(false);
    }
    private void EnterGameOverState()
    {
        currentGameFlowState = GameFlowState.GameOver;
        gameOverPanel.gameObject.SetActive(true);
        gameOverScoreText.text = currentScore.ToString();

        string name = Regex.Replace(System.Environment.UserName, @"\s+", "");
        
        scoreboardManager.PostScore(name, currentScore);
    }
    private void ExitGameOverState()
    {
        gameOverPanel.gameObject.SetActive(false);
    }
    private void EnterScoreBoardState()
    {
        currentGameFlowState = GameFlowState.ScoreBoard;
        scoreboardPanel.gameObject.SetActive(true);
        RetrieveScoresFromServer();
    }
    private void ExitScoreBoardState()
    {
        scoreboardPanel.gameObject.SetActive(false);
    }
    #endregion

    #region Money Life and Score
    private int currentMoney = 0;
    private int CurrentMoney
    {
        get { return currentMoney; }
        set
        {
            currentMoney = value >= 0 ? value : 0;
            moneyText.text = currentMoney.ToString();
            OnMoneyChange();
        }
    }
    private void OnMoneyChange()
    {
        for (int i = 0; i < towerButtons.Count; i++)
        {
            TowerButton towerButton = towerButtons[i];
            towerButton.SetActiveState(towerPrefabs[i].cost <= CurrentMoney);
        }
    }

    private int currentLife = 0;
    private int CurrentLife
    {
        get { return currentLife; }
        set
        {
            currentLife = value;
            lifeText.text = currentLife.ToString();
            OnLifeChange();
        }
    }
    private void OnLifeChange()
    {
        if(CurrentLife < 0) { GameOver(); }
    }

    private int currentScore = 0;
    private int CurrentScore
    {
        get { return currentScore; }
        set
        {
            currentScore = value >= 0 ? value : 0;
            scoreText.text = currentScore.ToString();
            OnScoreChange();
        }
    }
    private void OnScoreChange()
    {
        //TODO: Anything
    }

    private void RetrieveScoresFromServer()
    {
        scoreboardManager.GetScores((l) => scoreboardManager.PopulateScoreTable(l));
    }

    #endregion

    #region Initialisation
    private void Initialise()
    {
        CleanUpEnemyWave();
        CreateTowerButtons();
        CreateTowerSlots();
        CreatePath();
        CreateGates();
        //Needs to be last in order to proc relevant callbacks
        InitialiseGameValues();
    }

    private void CreateTowerButtons()
    {
        //Clean up
        for (int i = towerButtons.Count - 1; i >= 0; i--)
        {
            TowerButton towerButton = towerButtons[i];
            towerButtons.RemoveAt(i);
            GameObject.Destroy(towerButton.gameObject);
        }

        //Create
        towerButtonParent.constraintCount = towerPrefabs.Count;
        for (int i = 0; i < towerPrefabs.Count; i++)
        {
            TowerButton newTowerButton = GameObject.Instantiate<TowerButton>(towerButtonPrefab);
            newTowerButton.transform.SetParent(towerButtonParent.transform);
            int index = i;
            newTowerButton.Initialise(towerPrefabs[i], () => StartPlacingTower(index));

            towerButtons.Add(newTowerButton);
        }
    }

    private void CreateTowerSlots()
    {
        //TODO: Instantiate based on 2D design
        /*
        for (int i = 0; i < 10; i++)
        {
            TowerSlot newTowerSlot = GameObject.Instantiate(towerSlotPrefab);
            activeTowerSlots.Add(newTowerSlot);


            TowerSlot newTowerSlot;

            newTowerSlot.Initialise(
                () => activeTowerSlots.Remove(newTowerSlot),
                () => currSelTowerSlotIndex = activeTowerSlots.IndexOf(newTowerSlot));
        }
        */

        //These are assigned in Editor for now so in this case we simply rely on the slot initialisation for Clean up
        //Clean up
        
        //Create
        for (int i = 0; i < activeTowerSlots.Count; i++)
        {
            TowerSlot selTowerSlot = activeTowerSlots[i];

            selTowerSlot.Initialise(() => activeTowerSlots.Remove(selTowerSlot));
        }
    }

    private void CreatePath()
    {
        //TODO: Doesn't matter now. Later can be used to create various path layouts from input array
    }

    private void CreateGates()
    {
        //TODO: Doesn't matter now. Later can be used to create various path layouts from input array
    }

    private void CleanUpEnemyWave()
    {
        if(activeEnemyWave)
        {
            activeEnemyWave.ForceRemove();
        }
    }

    private void InitialiseGameValues()
    {
        CurrentMoney = startingMoney;

        CurrentLife = startingLives;

        CurrentScore = 0;

        spawnIndex = 0;

        SpawnButton.interactable = true;
    }
    #endregion

    #region GameFlow

    private void Update()
    {
        if(currentGameFlowState == GameFlowState.Playing)
        {
            UpdateEnemyWave();
            UpdateTowerSlots();
            UpdateTowerPlacement();
        }
    }

    private void UpdateEnemyWave()
    {
        if (activeEnemyWave) { activeEnemyWave.OnUpdate(); }
    }

    private void UpdateTowerSlots()
    {
        for (int i = 0; i < activeTowerSlots.Count; i++)
        {
            TowerSlot currTowerSlot = activeTowerSlots[i];

            currTowerSlot.OnUpdate();
        }
    }
    #endregion

    #region Tower Placement
    TowerSlot currSelTowerSlot
    {
        get { return activeTowerSlots[currSelTowerSlotIndex]; }
    }

    private int currSelTowerSlotIndex = 0;
    private bool placingTower = false;
    private int towerIndex = 0;

    public void StartPlacingTower(int index)
    {
        towerIndex = index;
        placingTower = true;
        towerPreview.SetActive(true);
    }

    public void StopPlacingTower()
    {
        placingTower = false;
        towerPreview.SetActive(false);
    }

    public void PlaceTower()
    {
        currSelTowerSlot.PlaceTower(towerPrefabs[towerIndex], () => CurrentMoney -= towerPrefabs[towerIndex].cost);

        StopPlacingTower();
    }

    private void UpdateTowerPlacement()
    {
        if (placingTower)
        {
            TowerSlot hitTowerSlot = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                hitTowerSlot = hit.collider.gameObject.GetComponent<TowerSlot>();
                if (hitTowerSlot) { break; }
            }

            if (hitTowerSlot)
            {
                currSelTowerSlotIndex = activeTowerSlots.IndexOf(hitTowerSlot);
            }

            towerPreview.transform.position = currSelTowerSlot.transform.position;

            //ChangePreviewColour
            if (currSelTowerSlot.tower)
            {
                towerPreviewMeshRenderer.material.color = towerPreviewBlockedColour;
            }
            else
            {
                towerPreviewMeshRenderer.material.color = towerPreviewDefaultColour;
            }

            if(Input.GetMouseButtonDown(0))
            {
                if(hitTowerSlot)
                {
                    PlaceTower();  
                }

                StopPlacingTower();
            }
        }
    }
    #endregion

    #region Spawn Wave
    int spawnIndex = 0;
    bool waveIsActive = false;
    public void SpawnWave()
    {
        if (spawnIndex >= waveDatabase.waves.Count) { GameOver();  return; } ;

        //Create Wave
        activeEnemyWave = Instantiate(new GameObject("EnemyWave")).AddComponent<EnemyWave>();
        activeEnemyWave.transform.position = Vector3.zero;
        activeEnemyWave.transform.parent = this.transform;

        activeEnemyWave.Initialise(
            //OnWaveFinishedCallback
            () =>
            {
                spawnIndex++;
                waveIsActive = false;
                SpawnButton.interactable = true;
                activeEnemyWave.ForceRemove();
            },
            //OnEnemyDieCallback
            (enemy) =>
            {
                CurrentMoney += enemy.wealth;
                CurrentScore += enemy.score;
            },
            //OnEnemyReachGoalCallback
            (enemy) =>
            {
                CurrentLife -= 1;
            },
            //enemy prefab ref
            waveDatabase.enemyPrefabs
            );
        waveIsActive = true;
        SpawnButton.interactable = false;

        activeEnemyWave.SpawnWave(waveDatabase.waves[spawnIndex]);
    }
    #endregion
}
