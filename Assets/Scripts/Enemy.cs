using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] public EnemyData enemyData;
    [SerializeField] public float chaseSpeed; 
    [SerializeField] private int health; 
    [SerializeField] public Player player;
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private GameObject basicEnemyBullet;
    private WaitForSeconds attackDelay;


    private void Start() {
        chaseSpeed = enemyData.chaseSpeed;
        health = enemyData.health;
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        attackDelay = new WaitForSeconds(enemyData.attackWaitTime);
        StartCoroutine(Attack());
    }

    private void Update() {
        Vector2 lookDirection = player.transform.position - transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        enemyData.MovementFunc.Invoke(this);
    }

    private IEnumerator Attack() {
        yield return attackDelay;
        enemyData.AttackPattern.Invoke(this);
    }









    // CHANGE TO SCRIPTABLE OBJECTS, ADD PARAMETERS TO UNITY EVENTS







    
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
