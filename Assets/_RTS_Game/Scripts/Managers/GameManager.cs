using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingletonManager<GameManager>
{
    [Header("Audio")]
    [SerializeField] private AudioSettingsSO m_backgroundAudioSettings;
    [SerializeField] private AudioSettingsSO m_winAudioSettings;
    [SerializeField] private AudioSettingsSO m_loseAudioSettings;

    [Space, Header("UI")]
    [SerializeField] private GameOverLayout m_gameOverLayout;
    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;
    private BuildingUnit m_activeBuildingUnit;
    private List<Unit> m_playerUnits = new();
    private List<Unit> m_enemyUnits = new();
    private List<Unit> m_kingUnits = new();
    private List<StorageUnit> m_structureUnits = new();

    private Dictionary<EUnitClass, List<Unit>> UnitListLookUp;

    [SerializeField] private Transform m_treeContainer;
    [SerializeField] private GoldMine m_activeGoldMine;
    private Tree[] m_trees = new Tree[0];
    public Unit ActiveUnit => m_activeUnit;
    public BuildingUnit BuildingUnit => m_activeBuildingUnit;
    public GoldMine ActiveGoldMine => m_activeGoldMine;

    private EnumSystem<EGameState> m_stateSystem = new();
    public EGameState CurrentState => m_stateSystem.Value;


    [Header("Spawner"), SerializeField]
    private EnemySpawner m_enemySpawner;

    private bool HasAnyEnemy => UnitListLookUp[EUnitClass.ENEMY].Count > 0;
    private bool IsVictory => !HasAnyEnemy && m_enemySpawner.CurrentState == ESpawnState.FINISHED;

    protected override void Awake()
    {
        base.Awake();

        UnitListLookUp = new Dictionary<EUnitClass, List<Unit>>() {
            { EUnitClass.PLAYER, m_playerUnits },
            { EUnitClass.ENEMY, m_enemyUnits },
            { EUnitClass.KING, m_kingUnits },
        };
    }

    private void OnEnable()
    {
        m_stateSystem.OnValueChange += HandleOnStateChange;
        m_gameOverLayout.OnRestartClicked += RestartGame;
        m_gameOverLayout.OnQuitClicked += GoToMenu;
    }

    private void OnDisable()
    {
        m_stateSystem.OnValueChange -= HandleOnStateChange;
        m_gameOverLayout.OnRestartClicked -= RestartGame;
        m_gameOverLayout.OnQuitClicked -= GoToMenu;
    }

    private void Start()
    {
        PlayerResourceManager.Instance.ResetResource();
        PlayerResourceManager.Instance.AddResource(50, 50);
        m_enemySpawner.StartUp();

        AudioManager.Instance.PlayerMusic(m_backgroundAudioSettings);

        ChangeGameState(EGameState.PLAYING);
    }

    private void HandleOnStateChange(EGameState eGameState)
    {
        switch (eGameState)
        {
            case EGameState.PLAYING:
                Time.timeScale = 1;
                break;
            case EGameState.PAUSE:
                Time.timeScale = 0;
                break;
            case EGameState.VICTORY:
                m_gameOverLayout.ShowGameOver(true);
                AudioManager.Instance.PlayerMusic(m_winAudioSettings);
                Time.timeScale = 0;
                break;
            case EGameState.GAME_OVER:
                m_gameOverLayout.ShowGameOver(false);
                AudioManager.Instance.PlayerMusic(m_loseAudioSettings);
                Time.timeScale = 0;
                break;
        }
    }

    private void ChangeGameState(EGameState eGameState)
    {
        m_stateSystem.SetValue(eGameState);
    }

    public void PlaceActiveBuildingUnit()
    {
        if (m_activeBuildingUnit == null) return;

        if (!m_activeBuildingUnit.TryStartBuildProgress(m_activeUnit as WorkerUnit))
        {
            Destroy(m_activeBuildingUnit.gameObject);
        }

        m_activeBuildingUnit = null;
    }
    public void SelectNewBuildingUnit(BuildingUnit buildingUnit)
    {
        m_activeBuildingUnit = buildingUnit;
    }
    public void ExecuteActiveUnit(Vector2 position)
    {
        if (!m_hasActiveUnit) return;

        m_activeUnit.DoActionAt(position);
    }

    private Unit FindClosestUnit(List<Unit> units, Vector3 originalPosition, float maxDistance)
    {
        Unit closestUnit = null;
        float closestDistance = float.MaxValue;

        foreach (Unit unit in units)
        {
            if (!unit.Enable) continue;

            float distance = Vector3.Distance(originalPosition, unit.transform.position);
            if (distance <= maxDistance && distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }

    public Unit FindClosestUnit(Vector3 originalPosition, float maxDistance, EUnitClass unitClass)
    {
        List<Unit> units = UnitListLookUp[unitClass];

        return FindClosestUnit(units, originalPosition, maxDistance);
    }
    public StorageUnit FindClosestStorageUnit(Vector3 originalPosition, float maxDistance, Func<StorageUnit, bool> sortedCondition)
    {
        List<Unit> units = m_structureUnits.Where(storage => sortedCondition(storage)).Cast<Unit>().ToList();

        return FindClosestUnit(units, originalPosition, maxDistance) as StorageUnit;
    }

    public Tree FindClosestUnClaimedTree(Vector3 originalPosition)
    {
        Tree closestTree = null;
        float closestDistance = float.MaxValue;

        if (m_trees.Length == 0)
        {
            m_trees = new Tree[m_treeContainer.childCount];

            for (int i = 0; i < m_treeContainer.childCount; i++)
            {
                m_trees[i] = m_treeContainer.GetChild(i).GetComponent<Tree>();
            }
        }

        foreach (Tree tree in m_trees)
        {
            if (tree.IsExploited) continue;

            float distance = Vector3.Distance(originalPosition, tree.transform.position);
            if (distance < closestDistance)
            {
                closestTree = tree;
                closestDistance = distance;
            }
        }

        return closestTree;
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == m_activeUnit)
        {
            CancelActiveUnit();
            return;
        }

        if (!m_hasActiveUnit || !m_activeUnit.TryInteractWithOtherUnit(unit))
        {
            SelectNewUnit(unit);
        }
    }
    public void SendWorkerToChop(Tree tree)
    {
        if (!m_hasActiveUnit) return;
        if (m_activeUnit is not WorkerUnit workerUnit) return;

        tree.StartExploitBy(workerUnit);
    }
    public void SendWorkerToMine(GoldMine goldMine)
    {
        if (!m_hasActiveUnit) return;
        if (m_activeUnit is not WorkerUnit workerUnit) return;

        workerUnit.SendToMine(goldMine);
    }
    public void RegisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Add(unit);
    }
    public void UnregisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Remove(unit);

        if (unit == m_activeUnit) CancelActiveUnit();

        if (IsVictory) ChangeGameState(EGameState.VICTORY);
        if (unit.Class == EUnitClass.KING) ChangeGameState(EGameState.GAME_OVER);
    }
    public void RegisterUnit(StorageUnit unit)
    {
        m_structureUnits.Add(unit);
    }

    public void UnregisterUnit(StorageUnit unit)
    {
        m_structureUnits.Remove(unit);

        if (unit == m_activeUnit) CancelActiveUnit();
    }

    public List<Unit> GetUnits(EUnitClass eUnitClass) => UnitListLookUp[eUnitClass];

    private void SelectNewUnit(Unit unit)
    {
        if (!IsRegisteredUnit(unit)) return;
        if (!unit.TryGetComponent(out SelectableUnit selectableUnit)) return;

        CancelActiveUnit();

        SetActiveUnit(unit);
        selectableUnit.Select();
    }

    private void CancelActiveUnit()
    {
        if (!m_hasActiveUnit) return;
        DeselectUnit(m_activeUnit);
    }

    public void CancelActiveUnit(Unit unit)
    {
        if (unit != m_activeUnit) return;

        DeselectUnit(unit);
    }

    private void DeselectUnit(Unit unit)
    {
        if (!unit.TryGetComponent(out SelectableUnit selectableUnit)) return;

        selectableUnit.Deselect();
        SetActiveUnit(null);
    }

    private void SetActiveUnit(Unit unit)
    {
        m_activeUnit = unit;
    }

    private bool IsRegisteredUnit(Unit unit) => UnitListLookUp[unit.Class].Contains(unit) || m_structureUnits.Contains(unit);

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}