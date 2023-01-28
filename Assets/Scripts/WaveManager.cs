using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour { 
    [SerializeField] private EnemyCreator enemyCreator;
    private void Start() {
        enemyCreator.SpawnEnemy(enemyCreator.basicEnemy);
        enemyCreator.SpawnEnemy(enemyCreator.fastEnemy);
    }
}