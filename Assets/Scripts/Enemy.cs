using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] public EnemyData data;
    [SerializeField] public float chaseSpeed; 
    [SerializeField] public int health; 
    [SerializeField] public Rigidbody2D rb;
    public AttackFunc attackFunc;
    public MovementFunc movementFunc;
    private WaitForSeconds attackDelay;

    private void Start() {
        chaseSpeed = data.chaseSpeed;
        health = data.health;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = data.color;
        attackDelay = new WaitForSeconds(data.attackWaitTime);
        StartCoroutine(Attack());
    }

    private void Update() {
        movementFunc(this);
    }

    private IEnumerator Attack() {
        while (true) {
            yield return attackDelay;
            StartCoroutine(attackFunc(this));
        }
    }
    
    public void ChangeHealthBy(int amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
