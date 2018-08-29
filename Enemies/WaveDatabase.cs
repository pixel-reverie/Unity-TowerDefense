using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDatabase : MonoBehaviour
{
    [SerializeField]
    public List<Enemy> enemyPrefabs = new List<Enemy>();

    [SerializeField]
    public List<Wave> waves = new List<Wave>();

    [Serializable]
    public class Wave
    {
        [SerializeField]
        public List<WaveEnemy> waveEnemies = new List<WaveEnemy>();
    }

    [Serializable]
    public class WaveEnemy
    {
        public int index = 0;
        public float delay = 0.5f;
    }
}
