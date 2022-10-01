using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private int health = 100;
    [SerializeField] public enum EnemyType {
        Basic,
        Fast
    }
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Color[] enemyColors; 
    [SerializeField] private Dictionary<EnemyType, float> enemySpeeds = new() {
        {EnemyType.Basic, 2f},
        {EnemyType.Fast, 3f},
    };
    private enum BulletSprites {
        Basic,
        Fast
    }
    [SerializeField] private List<Sprite> enemyBulletSprites = new();
    private Dictionary<BulletSprites, Sprite> enemyBulletSpriteDict;
    private Dictionary<EnemyType, Color> enemyColorDict;
    private delegate IEnumerator StartAttackPattern();
    private Dictionary<EnemyType, StartAttackPattern> attackPatternDict;
    [SerializeField] private GameObject basicEnemyBullet;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        chaseSpeed = enemySpeeds[enemyType];
        enemyColorDict = new Dictionary<EnemyType, Color>() {
            {EnemyType.Basic, enemyColors[0]},
            {EnemyType.Fast, enemyColors[1]},
        };
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        attackPatternDict = new Dictionary<EnemyType, StartAttackPattern>() {
            {EnemyType.Basic, BasicAttackPattern},
            {EnemyType.Fast, FastAttackPattern},
        };
        enemyBulletSpriteDict = new Dictionary<BulletSprites, Sprite>() {
            {BulletSprites.Basic, enemyBulletSprites[0]},
            {BulletSprites.Fast, enemyBulletSprites[1]},
        };
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        StartCoroutine(attackPatternDict[enemyType]());        
    }

    private void FireProjectile(float theta, Vector3 scale, float speed, int damage, Sprite sprite) {
        GameObject bullet = Instantiate(basicEnemyBullet, transform.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        bullet.transform.localScale = scale;
        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        bullet.GetComponent<Bullet>().damage = damage;
        bullet.GetComponent<SpriteRenderer>().sprite = sprite;
    }   

    private IEnumerator BasicAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(theta, new Vector3(1f, 1f, 1f), 5f, 20, enemyBulletSpriteDict[BulletSprites.Basic]);
        }
    }
    
    private IEnumerator FastAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1.5f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(theta, new Vector3(1f, 1f, 1f), 7f, 10, enemyBulletSpriteDict[BulletSprites.Fast]);
        }
    }

    private void Update() {
        Vector2 lookDirection = player.transform.position - transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        // make the enemy face in the direction of the player
        rb.velocity = transform.right * chaseSpeed;
        // enemy chases after player
    }

    public void ChangeHealthBy(int amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
