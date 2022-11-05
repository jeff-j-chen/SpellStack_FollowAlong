using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
    [SerializeField] private Player player;
    [SerializeField] private GameObject lStrikeBorder;
    [SerializeField] private GameObject lStrikeCenter;
    [SerializeField] private GameObject lightningGO;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private int health = 100;
    [SerializeField] public enum EnemyType {
        Basic,
        Fast,
        Tracking,
        Shotgun,
        MachineGun,
        FatShot, 
        ProjectileTrail,
        Accelerator,
        Splitter,
        Wavy,
        SemiAuto,
        Lighting,
    }
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Color[] enemyColors; 
    [SerializeField] private Dictionary<EnemyType, float> enemySpeeds = new() {
        {EnemyType.Basic, 2f},
        {EnemyType.Fast, 3f},
        {EnemyType.Tracking, 2f},
        {EnemyType.Shotgun, 1f},
    };
    private enum BulletSprites {
        Basic,
        Fast,
        Tracking,
        Shotgun,
        MachineGun,
        FatShot, 
        ProjectileTrail,
        Accelerator,
        Splitter,
        Wavy,
        SemiAuto,
        Lighting
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
            {EnemyType.Tracking, enemyColors[2]},
            {EnemyType.Shotgun, enemyColors[3]},
            {EnemyType.MachineGun, enemyColors[4]},
            {EnemyType.FatShot, enemyColors[5]},
            {EnemyType.ProjectileTrail, enemyColors[6]},
            {EnemyType.Accelerator, enemyColors[7]},
            {EnemyType.Splitter, enemyColors[8]},
            {EnemyType.Wavy, enemyColors[9]},
            {EnemyType.SemiAuto, enemyColors[10]},
            {EnemyType.Lighting, enemyColors[11]},
        };
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        attackPatternDict = new Dictionary<EnemyType, StartAttackPattern>() {
            {EnemyType.Basic, BasicAttackPattern},
            {EnemyType.Fast, FastAttackPattern},
            {EnemyType.Tracking, TrackingAttackPattern},
            {EnemyType.Shotgun, ShotgunAttackPattern},
            {EnemyType.MachineGun, MachineGunAttackPattern},
            {EnemyType.FatShot, FatShotAttackPattern},
            {EnemyType.ProjectileTrail, ProjectileTrailAttackPattern},
            {EnemyType.Accelerator, AcceleratorAttackPattern},
            {EnemyType.Splitter, SplitterAttackPattern},
            {EnemyType.Wavy, WavyAttackPattern},
            {EnemyType.SemiAuto, SemiAutoAttackPattern},
            {EnemyType.Lighting, LightingAttackPattern},
        };
        enemyBulletSpriteDict = new Dictionary<BulletSprites, Sprite>() {
            {BulletSprites.Basic, enemyBulletSprites[0]},
            {BulletSprites.Fast, enemyBulletSprites[1]},
            {BulletSprites.Tracking, enemyBulletSprites[2]},
            {BulletSprites.Shotgun, enemyBulletSprites[3]},
            {BulletSprites.MachineGun, enemyBulletSprites[4]},
            {BulletSprites.FatShot, enemyBulletSprites[5]},
            {BulletSprites.ProjectileTrail, enemyBulletSprites[6]},
            {BulletSprites.Accelerator, enemyBulletSprites[7]},
            {BulletSprites.Splitter, enemyBulletSprites[8]},
            {BulletSprites.Wavy, enemyBulletSprites[9]},
            {BulletSprites.SemiAuto, enemyBulletSprites[10]},
            {BulletSprites.Lighting, enemyBulletSprites[11]},
        };
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        StartCoroutine(attackPatternDict[enemyType]());        
    }

    private GameObject FireProjectile(Vector3 pos, float angle, Vector3 scale, float speed, int damage, Sprite sprite, float accelerationMultiplier, bool isWavy) {
        GameObject go = Instantiate(basicEnemyBullet, pos, Quaternion.identity);
        Bullet bullet = go.GetComponent<Bullet>();
        go.transform.rotation = Quaternio   n.Euler(new Vector3(0f, 0f, angle * Mathf.Rad2Deg));
        go.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * speed;
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        go.transform.localScale = scale;
        bullet.damage = damage;
        bullet.accelerationMultiplier = accelerationMultiplier;
        bullet.isWavy = isWavy;
        return go;
    }

    private Vector3 CalculatePlayerPos(int steps) {
        float playerH = player.horizontal;
        float playerV = player.vertical;
        if (playerH != 0 && playerV != 0) {
            playerH /= player.moveLimiter;
            playerV /= player.moveLimiter;
        }
        Vector3 toAdd = new Vector3(
            playerH * player.movementMultiplier * steps, 
            playerV * player.movementMultiplier * steps,
            0f
        );
        return toAdd;
    }

    private IEnumerator BasicAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }
    
    private IEnumerator FastAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1.5f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1.01f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }

    private IEnumerator LightingAttackPattern() {
        while (true) {
            Vector2 drop = (Vector2)player.transform.position + new Vector2(Random.Range(-4, 4), Random.Range(-4, 4));
            GameObject b = Instantiate(lStrikeBorder, drop, Quaternion.identity);
            GameObject c = Instantiate(lStrikeCenter, drop, Quaternion.identity);
            float initial = c.transform.localScale.x;
            for (int i = 0; i < (2f*10); i++) {
                c.transform.localScale -= new Vector3(initial/(2f*10), initial/(2f*10), 0f);
                yield return new WaitForSeconds(0.1f);
            }
            GameObject l = Instantiate(lightningGO, drop + new Vector2(0, 1f), Quaternion.identity);
            l.transform.localScale = new Vector3(4f, 4f, 1f);
            l.GetComponent<Bullet>().damage = 30;
            l.GetComponent<Bullet>().behavior = Bullet.Behavior.Linger;
            yield return new WaitForSeconds(0.3f);
            Destroy(c);
            Destroy(l);
            Destroy(b);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator SemiAutoAttackPattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            StartCoroutine(SemiAutoShoot(theta));
        }
    }

    private IEnumerator SemiAutoShoot(float angleBullet)
    {
        int countBullet = 0;
        while (countBullet < 3)
        {
            FireProjectile(
                speed: 5f,
                damage: 20,
                angle: angleBullet,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1f, 1f, 1f),
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
            countBullet++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SplitterAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            GameObject fired = FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
            StartCoroutine(SplitTheBullet(fired, theta));
        }
    }

    private IEnumerator SplitTheBullet(GameObject fired, float theta) {
        int countBullet = 0;
        while(countBullet < 2)
        {
            yield return new WaitForSeconds(1f);
            FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta + 0.1f,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f, 
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
            FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta - 0.1f,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f, 
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }

    private IEnumerator AcceleratorAttackPattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 5f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1.5f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }

    private IEnumerator TrackingAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(0.5f);
            Vector2 lookDirection = player.transform.position + CalculatePlayerPos(40) - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 7f, 
                damage: 10, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1.005f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }

    private IEnumerator ShotgunAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(2f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            for (int i = -2; i < 3; i++) {
                FireProjectile(
                    speed: 4f, 
                    damage: 6, 
                    angle: theta + (i * 0.2f), 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.Shotgun]
                );
            }
        }
    }

    private IEnumerator MachineGunAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(0.1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            float randOffset = Random.Range(-0.2f, 0.2f);
            FireProjectile(
                speed: 5f, 
                damage: 5, 
                angle: theta + randOffset, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1.25f, 1.25f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
            );
        }
    }

    private IEnumerator FatShotAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(2f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 3f, 
                damage: 30, 
                angle: theta,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f, 
                scale: new Vector3(2.25f, 2.25f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.FatShot]
            );
        }
    }

    private IEnumerator ProjectileTrailAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 0f, 
                damage: 10, 
                angle: theta,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 0f, 
                scale: new Vector3(2f, 2f, 2f), 
                sprite: enemyBulletSpriteDict[BulletSprites.ProjectileTrail]
            );
        }
    }

    private IEnumerator AcceleratorShotPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 1f, 
                damage: 5, 
                angle: theta,
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1.05f, 
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Accelerator]
            );
        }
    }

    private IEnumerator WavyAttackPattern() {
        while (true) {
            yield return new WaitForSeconds(1f);
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            FireProjectile(
                speed: 5f, 
                damage: 10, 
                angle: theta,
                isWavy: true,
                pos: transform.position,
                accelerationMultiplier: 1f, 
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Wavy]
            );
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
