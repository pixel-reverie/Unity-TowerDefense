using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyWave : MonoBehaviour
{
    Action OnWaveFinished;
    Action<Enemy> OnEnemyDie;
    Action<Enemy> OnEnemyReachGoal;

    List<Enemy> enemyPrefabs = new List<Enemy>();

    private List<Enemy> activeEnemies = new List<Enemy>();

    public void Initialise(Action onWaveFinished, Action<Enemy> onEnemyDie, Action<Enemy> onEnemyReachGoal, List<Enemy> enemyPrefabsRef)
    {
        OnWaveFinished = onWaveFinished;
        OnEnemyDie = onEnemyDie;
        OnEnemyReachGoal = onEnemyReachGoal;
        enemyPrefabs = enemyPrefabsRef;
    }

    public void OnUpdate()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            Enemy currentEnemy = activeEnemies[i];

            currentEnemy.OnUpdate();
        }
    }

    public void SpawnWave(WaveDatabase.Wave wave)
    {
        StartCoroutine(IEnum_SpawnWave(wave));
    }

    public IEnumerator IEnum_SpawnWave(WaveDatabase.Wave wave)
    {
        for (int i = 0; i < wave.waveEnemies.Count; i++)
        {
            WaveDatabase.WaveEnemy selWaveEnemy = wave.waveEnemies[i];
            SpawnEnemy(selWaveEnemy.index);
            yield return new WaitForSeconds(selWaveEnemy.delay);
        }
    }

    public void SpawnEnemy(int index)
    {
        if (index >= enemyPrefabs.Count) { Debug.Log("Invalid Enemy Index"); return; }

        Enemy enemy = Instantiate<Enemy>(enemyPrefabs[index]) as Enemy;

        enemy.transform.localPosition = Vector3.zero;
        enemy.transform.parent = this.transform;
        activeEnemies.Add(enemy);
        enemy.Initialise(
            //On Die Callback
            () =>
            {
                activeEnemies.Remove(enemy);
                OnEnemyDie.Invoke(enemy);
                CheckWaveCompletion();
            },
            //On Reach Goal Callback
            () =>
            {
                activeEnemies.Remove(enemy);
                OnEnemyReachGoal.Invoke(enemy);
                CheckWaveCompletion();
            });
    }

    private void CheckWaveCompletion()
    {
        if(activeEnemies.Count==0)
        {
            OnWaveFinished.Invoke();
        }
    }

    public void ForceRemove()
    {
        //Clean up
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = activeEnemies[i];
            activeEnemies.RemoveAt(i);
            GameObject.Destroy(enemy.gameObject);
        }
        GameObject.Destroy(this.gameObject);
    }
}