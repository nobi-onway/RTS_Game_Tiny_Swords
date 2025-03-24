using UnityEngine;

[System.Serializable]
public struct EnemyConfig
{
    public EnemyUnit EnemyPrefab;
    public float Probability;
}

[System.Serializable]
public struct SpawnWave
{
    public EnemyConfig[] Enemies;
    public float Frequency;
    public float Duration;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnWave[] m_spawnWaves;
    [SerializeField] private Transform[] m_spawnPoints;
    [SerializeField] private float m_delayBetweenWaves;

    private int m_currentWaveIndex;
    private float m_delayBetweenWavesTimer;
    private float m_waveDurationTimer;
    private float m_spawnFrequencyTimer;
    private EnumSystem<ESpawnState> m_stateSystem = new();

    public ESpawnState CurrentState => m_stateSystem.Value;

    public void StartUp()
    {
        m_stateSystem.SetValue(ESpawnState.WAITING);
        m_currentWaveIndex = 0;

        InitializeTimer();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case ESpawnState.FINISHED:
                Debug.Log("Spawn Finished");
                break;
            case ESpawnState.WAITING:
                HandleWaiting();
                break;
            case ESpawnState.SPAWNING:
                HandleSpawning();
                break;
        }
    }

    private void HandleWaiting()
    {
        m_delayBetweenWavesTimer -= Time.deltaTime;

        if (m_delayBetweenWavesTimer <= 0)
        {
            m_stateSystem.SetValue(ESpawnState.SPAWNING);
            Debug.Log("Fight wave " + m_currentWaveIndex + 1 + "!");
        }
    }

    private void HandleSpawning()
    {
        m_waveDurationTimer -= Time.deltaTime;
        m_spawnFrequencyTimer -= Time.deltaTime;

        if (m_waveDurationTimer <= 0)
        {
            m_currentWaveIndex++;

            if (m_currentWaveIndex >= m_spawnWaves.Length)
            {
                m_stateSystem.SetValue(ESpawnState.FINISHED);
            }
            else
            {
                m_stateSystem.SetValue(ESpawnState.WAITING);

                InitializeTimer();
            }
        }
        else if (m_spawnFrequencyTimer <= 0)
        {
            Spawn();
            m_spawnFrequencyTimer = m_spawnWaves[m_currentWaveIndex].Frequency;
        }
    }

    private void InitializeTimer()
    {
        m_delayBetweenWavesTimer = m_delayBetweenWaves;
        m_waveDurationTimer = m_spawnWaves[m_currentWaveIndex].Duration;
        m_spawnFrequencyTimer = m_spawnWaves[m_currentWaveIndex].Frequency;
    }

    private void Spawn()
    {
        float totalProbability = 0;

        foreach (EnemyConfig enemyConfig in m_spawnWaves[m_currentWaveIndex].Enemies)
        {
            totalProbability += enemyConfig.Probability;
        }

        float randomValue = Random.Range(0, totalProbability);

        foreach (EnemyConfig enemyConfig in m_spawnWaves[m_currentWaveIndex].Enemies)
        {
            if (randomValue <= enemyConfig.Probability)
            {
                Vector3 spawnPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].position;

                Instantiate(enemyConfig.EnemyPrefab, spawnPoint, Quaternion.identity);
                break;
            }

            randomValue -= enemyConfig.Probability;
        }
    }
}