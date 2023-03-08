using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EnemyCreator : MonoBehaviour {
    [SerializeField] private GameObject enemyBase;
    [SerializeField] private GameObject lStrikeBorder;
    [SerializeField] private GameObject lStrikeCenter;
    [SerializeField] private GameObject lightningGO;

    [SerializeField] private List<Sprite> bulletSpriteList;
    private enum BulletSprites {
        Basic, 
        Fast,
        MachineGun,
        Shotgun,
        FatShot,
        ProjectileTrail,
        Wavy
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
            {BulletSprites.MachineGun, bulletSpriteList[2]},
            {BulletSprites.Shotgun, bulletSpriteList[3]},
            {BulletSprites.FatShot, bulletSpriteList[4]},
            {BulletSprites.ProjectileTrail, bulletSpriteList[5]},
            {BulletSprites.Wavy, bulletSpriteList[6]},
            
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

    public void SpawnEnemy(EnemyData enemyData) {
        GameObject go = Instantiate(enemyBase, transform.position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.data = enemyData;
        enemy.attackFunc = attackFuncDict[enemy.data.attackPattern];
        enemy.movementFunc = movementFuncDict[enemy.data.movementPattern];
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

    private IEnumerator BasicAttackPattern(Enemy enemy) {
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
        yield break;
    }

    private IEnumerator FastAttackPattern(Enemy enemy) {
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
        yield break;
    }
    
    private IEnumerator LightingAttackPattern(Enemy enemy) {
        Vector2 drop = (Vector2)enemy.player.transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        GameObject b = Instantiate(lStrikeBorder, drop, Quaternion.identity);
        GameObject c = Instantiate(lStrikeCenter, drop, Quaternion.identity);
        float initial = c.transform.localScale.x;
        for (int i = 0; i < (2f*10); i++) {
            c.transform.localScale -= new Vector3(initial/(2f*10), initial/(2f*10), 0f);
            yield return new WaitForSeconds(0.1f);
        }
        GameObject l = Instantiate(lightningGO, drop + new Vector2(0, 0.6f), Quaternion.identity);
        l.GetComponent<Bullet>().behavior = Bullet.Behavior.Linger;
        l.GetComponent<Bullet>().damage = 30;
        yield return new WaitForSeconds(0.01f);
        Destroy(l.GetComponent<CapsuleCollider2D>());
        yield return new WaitForSeconds(0.3f);
        Destroy(c);
        Destroy(l);
        Destroy(b);
    }

    private IEnumerator SemiAutoAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        for (int i = 0; i < 3; i++) {
            float randOffset = Random.Range(-0.3f, 0.3f);
            enemy.FireProjectile(
                speed: 6f, 
                damage: 10, 
                angle: theta + randOffset, 
                isWavy: false,
                pos: enemy.transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1.25f, 1.25f, 1f), 
                sprite: bulletSpriteDict[BulletSprites.MachineGun]
            );
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator SplitterAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        GameObject fired = enemy.FireProjectile(
            speed: 4f, 
            damage: 20, 
            angle: theta, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1f,
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SplitTheBullet(fired, theta, enemy));
    }

    private IEnumerator SplitTheBullet(GameObject fired, float theta, Enemy enemy) {
        yield return new WaitForSeconds(0.5f);
        enemy.FireProjectile(
            speed: 6f, 
            damage: 20, 
            angle: theta + 0.3f,
            isWavy: false,
            pos: fired.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
        enemy.FireProjectile(
            speed: 6f, 
            damage: 20, 
            angle: theta - 0.3f,
            isWavy: false,
            pos: fired.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
        Destroy(fired);
    }

    private IEnumerator AcceleratorAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 2f, 
            damage: 20, 
            angle: theta, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1.04f,
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
        yield break;
    }

    private IEnumerator TrackingAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position + CalculatePlayerPos(enemy.player, 40) - transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 7f, 
            damage: 10, 
            angle: theta, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1.005f,
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Basic]
        );
        yield break;
    }

    private IEnumerator ShotgunAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        for (int i = -2; i < 3; i++) {
            enemy.FireProjectile(
                speed: 4f, 
                damage: 6, 
                angle: theta + (i * 0.2f), 
                isWavy: false,
                pos: enemy.transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1.25f, 1.25f, 1f), 
                sprite: bulletSpriteDict[BulletSprites.Shotgun]
            );
        }
        yield break;
    }

    private IEnumerator MachineGunAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        float randOffset = Random.Range(-0.15f, 0.15f);
        enemy.FireProjectile(
            speed: 5f, 
            damage: 6, 
            angle: theta + randOffset, 
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1f,
            scale: new Vector3(1.25f, 1.25f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.MachineGun]
        );
        yield break;
    }

    private IEnumerator FatShotAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position + CalculatePlayerPos(enemy.player, 40) - transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 3f, 
            damage: 30, 
            angle: theta,
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(2.25f, 2.25f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.FatShot]
        );
        yield break;
    }

    private IEnumerator ProjectileTrailAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 0f, 
            damage: 10, 
            angle: theta,
            isWavy: false,
            pos: enemy.transform.position,
            accelerationMultiplier: 0f, 
            scale: new Vector3(2f, 2f, 2f), 
            sprite: bulletSpriteDict[BulletSprites.ProjectileTrail]
        );
        yield break;
    }

    private IEnumerator WavyAttackPattern(Enemy enemy) {
        Vector2 lookDirection = enemy.player.transform.position - enemy.transform.position;
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        enemy.FireProjectile(
            speed: 5f, 
            damage: 10, 
            angle: theta,
            isWavy: true,
            pos: enemy.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(1f, 1f, 1f), 
            sprite: bulletSpriteDict[BulletSprites.Wavy]
        );
        yield break;
    }

    private IEnumerator Boss1AttackPattern(Enemy enemy){
        if (enemy.health >= 250){
            enemy.attackFunc = ShotgunAttackPattern;
        }
        else{
            enemy.attackFunc = MachineGunAttackPattern;
        }
        yield break;
    }

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
    //             enemy.FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin, 
    //                 isWavy: false,
    //                 pos: enemy.transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: bulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             enemy.FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+1.571f, 
    //                 isWavy: false,
    //                 pos: enemy.transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: bulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             enemy.FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+3.142f, 
    //                 isWavy: false,
    //                 pos: enemy.transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: bulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             enemy.FireProjectile(
    //                 speed: 5f, 
    //                 damage: 6, 
    //                 angle: spin+4.712f, 
    //                 isWavy: false,
    //                 pos: enemy.transform.position,
    //                 accelerationMultiplier: 1f,
    //                 scale: new Vector3(1.25f, 1.25f, 1f), 
    //                 sprite: bulletSpriteDict[BulletSprites.MachineGun]
    //             );
    //             spin += 1f;
    //         }
            
    //     }
    // }
    private float DistFormula(Vector3 pos1, Vector3 pos2) {
        return Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }

    private Vector3 CalculatePlayerPos(Player player, int steps) {
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
}