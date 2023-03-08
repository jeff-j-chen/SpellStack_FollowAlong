using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] public EnemyData data;
    [SerializeField] public float chaseSpeed; 
    [SerializeField] public int health; 
    [SerializeField] public Player player;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private GameObject basicEnemyBullet;
    public AttackFunc attackFunc;
    public MovementFunc movementFunc;
    private WaitForSeconds attackDelay;

    private void Start() {
        chaseSpeed = data.chaseSpeed;
        health = data.health;
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        attackDelay = new WaitForSeconds(data.attackWaitTime);
        GetComponent<SpriteRenderer>().color = data.color;
        StartCoroutine(Attack());
    }

    private void Update() {
        Vector2 lookDirection = player.transform.position - transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
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
        // if(enemyType == EnemyType.Boss1){
        //     StopCoroutine(phaseCoroutine);
        //     StartCoroutine(Boss1AttackPattern());
        // }
        // if(enemyType == EnemyType.Boss2){
        //     StopCoroutine(phaseCoroutine);
        //     StartCoroutine(Boss2AttackPattern());
        // }
    }

    public GameObject FireProjectile(Vector3 pos, float angle, Vector3 scale, float speed, int damage, Sprite sprite, float accelerationMultiplier, bool isWavy) {
        GameObject go = Instantiate(basicEnemyBullet, pos, Quaternion.identity);
        Bullet bullet = go.GetComponent<Bullet>();
        go.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle * Mathf.Rad2Deg));
        go.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * speed;
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        go.transform.localScale = scale;
        bullet.damage = damage;
        bullet.accelerationMultiplier = accelerationMultiplier;
        bullet.isWavy = isWavy;
        return go;
    }
}
