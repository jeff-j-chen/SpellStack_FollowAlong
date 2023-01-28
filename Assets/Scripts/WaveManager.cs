using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour { 
    [SerializeField] private GameObject enemyBase;
    [SerializeField] private EnemyCreator enemyCreator;
    private void Start() {
        SpawnEnemy(enemyCreator.basicEnemy);
    }

    private void SpawnEnemy(EnemyData enemyData) {
        GameObject go = Instantiate(enemyBase, transform.position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.enemyData = enemyData;
    }
}