using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
    //Enemy's rigidbody
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public EnemyData data;
    
    private delegate void MovementFunc();
    //Coroutine for Boss Phase transitions
    private Coroutine phaseCoroutine;
    //Coroutine reference for current attack pattern
    private Coroutine currentAttackPatternCoroutine;
    //Delegate for attack patterns
    public delegate IEnumerator StartAttackPattern();
    //GameObject referencing the enemy's bullet prefab
    [SerializeField] private GameObject basicEnemyBullet;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        health = enemyHealth[enemyType];
        //Start Enemy's Attack Pattern based on enemy type and call it currentAttackPatternCoroutine
        currentAttackPatternCoroutine = StartCoroutine(attackPatternDict[enemyType]()); 
    }

    

    private void Update() {
        
        //Set enemy's movement type
        movementTypeDict[movementType]();
    } 

    public void ChangeHealthBy(int amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
        if(enemyType == EnemyType.Boss1){
            StopCoroutine(phaseCoroutine);
            StartCoroutine(Boss1AttackPattern());
        }
        if(enemyType == EnemyType.Boss2){
            StopCoroutine(phaseCoroutine);
            StartCoroutine(Boss2AttackPattern());
        }
    }
}
