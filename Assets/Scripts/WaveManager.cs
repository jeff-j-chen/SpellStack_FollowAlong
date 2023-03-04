using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour { 
    [SerializeField] private GameObject enemyBase;
    [SerializeField] private EnemyCreator enemyCreator;
    private void Start() {
        SpawnEnemy(enemyCreator.basicEnemy);
        SpawnEnemy(enemyCreator.fastEnemy);
    }

    private void SpawnEnemy(EnemyData enemyData) {
        GameObject go = Instantiate(enemyBase, transform.position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.data = enemyData;
        enemy.attackFunc = enemyCreator.attackFuncDict[enemy.data.attackPattern];
        enemy.movementFunc = enemyCreator.movementFuncDict[enemy.data.movementPattern];
    }
}