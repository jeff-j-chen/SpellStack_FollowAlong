using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EnemyCreator : MonoBehaviour {
    [SerializeField] private GameObject lStrikeBorder;
    [SerializeField] private GameObject lStrikeCenter;
    [SerializeField] private GameObject lightningGO;

    [SerializeField] private List<Sprite> bulletSpriteList;
    private enum BulletSprites {
        Basic, 
        Fast
    }
    private Dictionary<BulletSprites, Sprite> bulletSpriteDict;
    public EnemyData basicEnemy;
    public EnemyData fastEnemy;
    public Dictionary<AttackPattern, AttackFunc> attackFuncDict;
    public Dictionary<MovementPattern, MovementFunc> movementFuncDict;

    private void Start() {
        bulletSpriteDict = new Dictionary<BulletSprites, Sprite>() {
            {BulletSprites.Basic, bulletSpriteList[0]},
            {BulletSprites.Fast, bulletSpriteList[1]},
        };
        attackFuncDict = new Dictionary<AttackPattern, AttackFunc>() {
            {AttackPattern.BasicAttackPattern, BasicAttackPattern},
            {AttackPattern.FastAttackPattern, FastAttackPattern},
        };
        movementFuncDict = new Dictionary<MovementPattern, MovementFunc>() {
            {MovementPattern.ChasePlayer, ChasePlayer},
            {MovementPattern.FleePlayer, FleePlayer},
            {MovementPattern.Stationary, Stationary},
            {MovementPattern.Jumping, Jumping},
            {MovementPattern.Support, Support},
            {MovementPattern.MaintainDistance, MaintainDistance},
        };
    }

    // // // // // // //
    // 
    // MOVEMENT FUNCTIONS
    // 
    // // // // // // //
    private void ChasePlayer(Enemy enemy) {
        enemy.rb.velocity = enemy.transform.right * enemy.chaseSpeed;
    }

    private void FleePlayer(Enemy enemy) { 
        enemy.rb.velocity = enemy.transform.right * -enemy.chaseSpeed;
    }

    private void Stationary(Enemy enemy) { 
        enemy.rb.velocity = Vector2.zero;
    }

    private void Jumping(Enemy enemy) {
        float timeNow = Time.time;
        if (timeNow % 3f < 0.5) {
            enemy.rb.velocity = enemy.transform.right * enemy.chaseSpeed * 3f;
        }
        else { 
            enemy.rb.velocity = enemy.transform.right * enemy.chaseSpeed / 3f;
        }
    }

    private void Support(Enemy enemy) { 
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        
        float minDistance = float.MaxValue;
        foreach (GameObject e in enemies) {
            float distance = DistFormula(transform.position, e.transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                nearestEnemy = e;
            }
        }
        if (nearestEnemy != null) {
            Vector2 lookDirection = nearestEnemy.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
            enemy.rb.velocity = transform.right * enemy.chaseSpeed;
        }
    }
    
    private void MaintainDistance(Enemy enemy) { 
        if (DistFormula(transform.position, enemy.player.transform.position) > 6f) {
            ChasePlayer(enemy);
        } 
        else if (DistFormula(transform.position, enemy.player.transform.position) < 5f) {
            FleePlayer(enemy);
        } 
        else {
            enemy.rb.velocity = Vector2.zero;
        }
    }
    
    // // // // // // //
    // 
    // ATTACK PATTERNS
    // 
    // // // // // // //

    private void BasicAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 5f, 
            damage: 20, 
            angle: theta, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1f,
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
    }

    private void FastAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 5f, 
            damage: 20, 
            angle: theta, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1.01f,
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Fast]
        );
    }
    

    // private IEnumerator Boss1AttackPattern(){
    //     if (health >= 250){
    //         phaseCoroutine = StartCoroutine(ShotgunAttackPattern());
    //         yield return new WaitForSeconds(1f);
    //     }
    //     else{
    //         StartCoroutine(MachineGunAttackPattern());
    //         StartCoroutine(LightingAttackPattern());
    //         yield return new WaitForSeconds(1f);
    //     }
    // }

    // private IEnumerator Boss2AttackPattern(){
    //     if (health >= 250){
    //         phaseCoroutine = StartCoroutine(ShotgunAttackPattern());
    //         yield return new WaitForSeconds(1f);
    //     }
    //     else{
    //         float spin = 0f;
    //         while(true)
    //         {
    //             yield return new WaitForSeconds(0.5f);
    //             FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin, 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+1.571f, 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+3.142f, 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+4.712f, 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             spin += 1f;
    //         }
            
    //     }
    // }


    // private IEnumerator LightingAttackPattern() {
    //     while (true) {
    //         Vector2 drop = (Vector2)player.transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    //         GameObject b = Instantiate(lStrikeBorder, drop, Quaternion.identity);
    //         GameObject c = Instantiate(lStrikeCenter, drop, Quaternion.identity);
    //         float initial = c.transform.localScale.x;
    //         for (int i = 0; i < (2f*10); i++) {
    //             c.transform.localScale -= new Vector3(initial/(2f*10), initial/(2f*10), 0f);
    //             yield return new WaitForSeconds(0.1f);
    //         }
    //         GameObject l = Instantiate(lightningGO, drop + new Vector2(0, 0.6f), Quaternion.identity);
    //         l.GetComponent<Bullet>().behavior = Bullet.Behavior.Linger;
    //         l.GetComponent<Bullet>().damage = 30;
    //         yield return new WaitForSeconds(0.01f);
    //         Destroy(l.GetComponent<CapsuleCollider2D>());
    //         yield return new WaitForSeconds(0.3f);
    //         Destroy(c);
    //         Destroy(l);
    //         Destroy(b);
    //         yield return new WaitForSeconds(2f);
    //     }
    // }

    // private IEnumerator SemiAutoAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(1f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         for (int i = 0; i < 3; i++) {
    //             float randOffset = Random.Range(-0.3f, 0.3f);
    //             FireProjectile(
    //                 speed: 6f, 
    //                 damage: 10, 
    //                 angle: theta + randOffset, 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             yield return new WaitForSeconds(0.1f);
    //         }
    //     }
    // }

    // private IEnumerator SplitterAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(1f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         GameObject fired = FireProjectile(
    //             speed: 4f, 
    //             damage: 20, 
    //             angle: theta, 
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 1f,
    //             scale: new Vector3(1f, 1f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.Basic]
    //         );
    //         StartCoroutine(SplitTheBullet(fired, theta));
    //     }
    // }

    // private IEnumerator SplitTheBullet(GameObject fired, float theta) {
    //     yield return new WaitForSeconds(0.5f);
    //     FireProjectile(
    //         speed: 6f, 
    //         damage: 20, 
    //         angle: theta + 0.3f,
    //         isWavy: false,
    //         pos: fired.transform.position,
    //         accelerationMultiplier: 1f, 
    //         scale: new Vector3(1f, 1f, 1f), 
    //         sprite: enemyBulletSpriteDict[BulletSprites.Basic]
    //     );
    //     FireProjectile(
    //         speed: 6f, 
    //         damage: 20, 
    //         angle: theta - 0.3f,
    //         isWavy: false,
    //         pos: fired.transform.position,
    //         accelerationMultiplier: 1f, 
    //         scale: new Vector3(1f, 1f, 1f), 
    //         sprite: enemyBulletSpriteDict[BulletSprites.Basic]
    //     );
    //     Destroy(fired);
    // }

    // private IEnumerator AcceleratorAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(1f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         FireProjectile(
    //             speed: 2f, 
    //             damage: 20, 
    //             angle: theta, 
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 1.04f,
    //             scale: new Vector3(1f, 1f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.Basic]
    //         );
    //     }
    // }

    // private IEnumerator TrackingAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(0.5f);
    //         Vector2 lookDirection = player.transform.position + CalculatePlayerPos(40) - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         FireProjectile(
    //             speed: 7f, 
    //             damage: 10, 
    //             angle: theta, 
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 1.005f,
    //             scale: new Vector3(1f, 1f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.Basic]
    //         );
    //     }
    // }

    // private IEnumerator ShotgunAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(2f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         for (int i = -2; i < 3; i++) {
    //             FireProjectile(
    //                 speed: 4f, 
    //                 damage: 6, 
    //                 angle: theta + (i * 0.2f), 
    //                 isWavy: false,
    //                 pos: transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: enemyBulletSpriteDict[BulletSprites.Shotgun]
    //             );
    //         }
    //     }
    // }

    // private IEnumerator MachineGunAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(0.3f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         float randOffset = Random.Range(-0.15f, 0.15f);
    //         FireProjectile(
    //             speed: 5f, 
    //             damage: 6, 
    //             angle: theta + randOffset, 
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 1f,
    //             scale: new Vector3(1.25f, 1.25f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
    //         );
    //     }
    // }

    // private IEnumerator FatShotAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(2f);
    //         Vector2 lookDirection = player.transform.position + CalculatePlayerPos(40) - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         FireProjectile(
    //             speed: 3f, 
    //             damage: 30, 
    //             angle: theta,
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 1f, 
    //             scale: new Vector3(2.25f, 2.25f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.FatShot]
    //         );
    //     }
    // }

    // private IEnumerator ProjectileTrailAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(1f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         FireProjectile(
    //             speed: 0f, 
    //             damage: 10, 
    //             angle: theta,
    //             isWavy: false,
    //             pos: transform.position,
    //             accelerationMultiplier: 0f, 
    //             scale: new Vector3(2f, 2f, 2f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.ProjectileTrail]
    //         );
    //     }
    // }

    // private IEnumerator WavyAttackPattern() {
    //     while (true) {
    //         yield return new WaitForSeconds(1f);
    //         Vector2 lookDirection = player.transform.position - transform.position;
    //         float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
    //         FireProjectile(
    //             speed: 5f, 
    //             damage: 10, 
    //             angle: theta,
    //             isWavy: true,
    //             pos: transform.position,
    //             accelerationMultiplier: 1f, 
    //             scale: new Vector3(1f, 1f, 1f), 
    //             sprite: enemyBulletSpriteDict[BulletSprites.Wavy]
    //         );
    //     }
    // }

    private float DistFormula(Vector3 pos1, Vector3 pos2) {
        return Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }
}