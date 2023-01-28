using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable {
    //Enemy needs to know where the player is
    [SerializeField] private Player player;
    //Enemy's rigidbody
    [SerializeField] private Rigidbody2D rb;
    //Current Enemy's Type, default is Basic
    private EnemyType enemyType = EnemyType.Basic;
    //Lightning Strike Border
    [SerializeField] private GameObject lStrikeBorder;
    //Lightning Strike Center
    [SerializeField] private GameObject lStrikeCenter;
    //Lightning Strike
    [SerializeField] private GameObject lightningGO;
    //Enemy's chase speed towards player
    [SerializeField] private float chaseSpeed = 2.5f; 
    //Enemy's health, default is 100
    [SerializeField] private int health = 100; 
    //Boss's phase, default is 1
    private int phase = 1;
    //Enemy's type
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
        Boss1,
        Boss2,
    }
    //Enemy's movement type
    [SerializeField] private enum MovementType {
        Chase,
        Flee,
        Maintain,
        Stationary,
        Jumping,
        Support,
        Random
    } 
   //Current Enemy's Movement Type
    [SerializeField] private MovementType movementType = MovementType.Chase;
    //Different Enemy Colors (Remember to add more colors if you add more enemies in the inspector)
    [SerializeField] private Color[] enemyColors; 
    //Dictionary of Enemy Types and their respective chase speeds
    [SerializeField] private Dictionary<EnemyType, float> enemySpeeds = new() {
        {EnemyType.Basic, 2f},
        {EnemyType.Fast, 3f},
        {EnemyType.Tracking, 2f},
        {EnemyType.Shotgun, 1f},
        {EnemyType.MachineGun, 1f},
        {EnemyType.FatShot, 1f},
        {EnemyType.ProjectileTrail, 1f},
        {EnemyType.Accelerator, 1f},
        {EnemyType.Splitter, 1f},
        {EnemyType.Wavy, 1f},
        {EnemyType.SemiAuto, 1f},
        {EnemyType.Lighting, 1f},
        {EnemyType.Boss1, 1f},
        {EnemyType.Boss2, 1f}
    };
    //Enum of Enemy's Bullet Sprites
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
    //List of Enemy's Bullet Sprites
    [SerializeField] private List<Sprite> enemyBulletSprites = new();
    //Dictionary of Bullet Sprites and their respective sprites
    private Dictionary<BulletSprites, Sprite> enemyBulletSpriteDict;
    //Dictionary of Enemy Types and their respective colors
    private Dictionary<EnemyType, Color> enemyColorDict;
    //Dictionary of Enemy Types and their respective attack patterns
    private Dictionary<EnemyType, StartAttackPattern> attackPatternDict;
    //Dictionary of Enemy Types and their respective movement type functions
    private Dictionary<MovementType, MovementFunc> movementTypeDict;
    //Dictionary linking enemy types to their respective health
    private Dictionary<EnemyType, int> enemyHealth;
    //Delegate for movement type functions
    private delegate void MovementFunc();
    //Coroutine for Boss Phase transitions
    private Coroutine phaseCoroutine;
    //Coroutine reference for current attack pattern
    private Coroutine currentAttackPatternCoroutine;
    //Delegate for attack patterns
    private delegate IEnumerator StartAttackPattern();
    //GameObject referencing the enemy's bullet prefab
    [SerializeField] private GameObject basicEnemyBullet;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        //Set Enemy's chase speed based on enemy type
        chaseSpeed = enemySpeeds[enemyType];
        //Set Enemy's health based on enemy type
        enemyHealth = new Dictionary<EnemyType, int>() {
            {EnemyType.Basic, 100},
            {EnemyType.Fast, 100},
            {EnemyType.Tracking, 100},
            {EnemyType.Shotgun, 100},
            {EnemyType.MachineGun, 100},
            {EnemyType.FatShot, 100},
            {EnemyType.ProjectileTrail, 100},
            {EnemyType.Accelerator, 100},
            {EnemyType.Splitter, 100},
            {EnemyType.Wavy, 100},
            {EnemyType.SemiAuto, 100},
            {EnemyType.Lighting, 100},
            {EnemyType.Boss1, 500},
            {EnemyType.Boss2, 500}
        };
        //Set Enemy's Color based on enemy type
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
            {EnemyType.Boss1, enemyColors[12]},
            {EnemyType.Boss2, enemyColors[13]}
        };
        //Set Enemy's Color based on enemy type from the Sprite Renderer
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        //Set Enemy's Attack Pattern based on enemy type
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
            {EnemyType.Boss1, Boss1AttackPattern},
            {EnemyType.Boss2, Boss2AttackPattern}
        };
        //Set Enemy's Movement Type based on enemy type
        movementTypeDict = new Dictionary<MovementType, MovementFunc>() {
            {MovementType.Chase, ChasePlayer},
            {MovementType.Flee, FleePlayer},
            {MovementType.Maintain, MaintainDistance},
            {MovementType.Stationary, Stationary},
            {MovementType.Jumping, Jumping},
            {MovementType.Support, Support},
            {MovementType.Random, RandomMovement},
        };
        //Set Enemy's Bullet Sprites based on enemy type
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
        //Set Enemy's Health based on enemy type from the Dictionary
        health = enemyHealth[enemyType];
        //Set Enemy's Color based on enemy type from the Sprite Renderer
        GetComponent<SpriteRenderer>().color = enemyColorDict[enemyType];
        //Start Enemy's Attack Pattern based on enemy type and call it currentAttackPatternCoroutine
        currentAttackPatternCoroutine = StartCoroutine(attackPatternDict[enemyType]()); 
    }

    //FireProjectile() is a function that creates a bullet and sets its properties (overloaded)
    private GameObject FireProjectile(Vector3 pos, float angle, Vector3 scale, float speed, int damage, Sprite sprite, float accelerationMultiplier, bool isWavy) {
        //Creates Bullet
        GameObject go = Instantiate(basicEnemyBullet, pos, Quaternion.identity);
        //Call Bullet's Script
        Bullet bullet = go.GetComponent<Bullet>();
        //Freeze Bullet's Rotation and Set bullet's facing direction (in Degrees) from the parameter
        go.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle * Mathf.Rad2Deg));
        //Set Bullet's Velocity (transform.right is the facing direction of the bullet) from the parameter
        go.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * speed;
        //Set Bullet's Sprite from the parameter
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        //Set Bullet's Scale (size) from the parameter
        go.transform.localScale = scale;
        //Set Bullet's Damage from the parameter
        bullet.damage = damage;
        //Set Bullet's Acceleration Multiplier from the parameter
        bullet.accelerationMultiplier = accelerationMultiplier;
        //Set Bullet's Wavy Property from the parameter, default is false
        bullet.isWavy = isWavy;
        //Return the Bullet
        return go;
    }

    //CalculatePlayerPos() is a function that calculates the player's position based on the number of steps (Where to shoot arount the player, radius is steps)
    private Vector3 CalculatePlayerPos(int steps) {
        //Call player's horizontal and vertical movement
        float playerH = player.horizontal;
        float playerV = player.vertical;
        //If player is moving diagonally, divide the movement by the moveLimiter (to make it move slower)
        if (playerH != 0 && playerV != 0) {
            playerH /= player.moveLimiter;
            playerV /= player.moveLimiter;
        }
        //Calculate the player's position based on the movement and the number of steps
        Vector3 toAdd = new Vector3(
            playerH * player.movementMultiplier * steps, 
            playerV * player.movementMultiplier * steps,
            0f
        );
        //return the Vector3 to add to the predicted player's position
        return toAdd;
    }

    //Boss Type 1 Attack Pattern (Phase 1: Shotgun, Phase 2: Machine Gun + Lighting)
    private IEnumerator Boss1AttackPattern(){
        //If health is greater than 250, do Phase 1
        if (health >= 250){
            //Set phaseCoroutine to the ShotgunAttackPattern() Coroutine so that it can be stopped later
            phaseCoroutine = StartCoroutine(ShotgunAttackPattern());
            //Delay for 1 second (Projectile fire rate)
            yield return new WaitForSeconds(1f);
        }
        //else do Phase 2
        else{
            StartCoroutine(MachineGunAttackPattern());
            StartCoroutine(LightingAttackPattern());
            //Delay for 1 second (Projectile fire rate)
            yield return new WaitForSeconds(1f);
        }
    }

    //Boss Type 2 Attack Pattern (Phase 1: Shotgun, Phase 2: Shooting in Spriral and if hit, fire more projectiles)
    private IEnumerator Boss2AttackPattern(){
        //If health is greater than 250, do Phase 1
        if (health >= 250){
            //Set phaseCoroutine to the ShotgunAttackPattern() Coroutine so that it can be stopped later
            phaseCoroutine = StartCoroutine(ShotgunAttackPattern());
            //Delay for 1 second
            yield return new WaitForSeconds(1f);
        }
        //else do Phase 2
        else{
            //For the shooting of projectiles to be in sprials, the all the angle of the projectiles must be shifted by a certain amount (spin variable is the counter variable)
            float spin = 0f;
            //While the enemy is alive, loop continues
            while(true)
            {
                //Delay for 0.5 seconds (Projectile's fire rate)
                yield return new WaitForSeconds(0.5f);
                //Create a game object (Bullet) and set its properties (angle is 0f at start, then firing angle changes by 1f every time it finishes loop (0.5f delay))
                FireProjectile(
                    speed: 5f, 
                    damage: 6, 
                    angle: spin, 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
                );
                //Create a game object (Bullet) and set its properties (angle is pi/2 [90 degrees] at start, then firing angle changes by 1f every time it finishes loop (0.5f delay))
                FireProjectile(
                    speed: 5f, 
                    damage: 6, 
                    angle: spin+1.571f, 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
                );
                //Create a game object (Bullet) and set its properties (angle is pi [180 degrees] at start, then firing angle changes by 1f every time it finishes loop (0.5f delay))
                FireProjectile(
                    speed: 5f, 
                    damage: 6, 
                    angle: spin+3.142f, 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
                );
                //Create a game object (Bullet) and set its properties (angle is 3pi/2 [270 degrees] at start, then firing angle changes by 1f every time it finishes loop (0.5f delay))
                FireProjectile(
                    speed: 5f, 
                    damage: 6, 
                    angle: spin+4.712f, 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
                );
                //Add 1f to spin
                spin += 1f;
            }
            
        }
    }

    //Basic Attack Pattern for Enemy (Shoot bullets at player)
    private IEnumerator BasicAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties
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
    
    //Fast Attack Pattern for Enemy (Shoot bullets at player in a faster bullet speed)
    private IEnumerator FastAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1.5 seconds (Projectile's fire rate)
            yield return new WaitForSeconds(1.5f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties (Bullet speed is 5f)
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

    //Lightning Attack Pattern for Enemy (Cast a lightning strike at player, striking a random location around the player after a delay)
    private IEnumerator LightingAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Calculate a random location around the player to strike
            Vector2 drop = (Vector2)player.transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
            //Assign a game object (Border of lightning strike) at the calculated location and set no rotations calling it b
            GameObject b = Instantiate(lStrikeBorder, drop, Quaternion.identity);
            //Assign a game object (Center of lightning strike) at the calculated location and set no rotations calling it c
            GameObject c = Instantiate(lStrikeCenter, drop, Quaternion.identity);
            //Save the initial scale of the center (size) of the lightning strike (will be used to shrink it)
            float initial = c.transform.localScale.x;
            //Create a for loop that will shrink the center of the lightning strike by 1/20th of its initial size every 0.1 seconds
            for (int i = 0; i < (2f*10); i++) {
                //Shrink the center of the lightning strike by 1/20th of its initial size
                c.transform.localScale -= new Vector3(initial/(2f*10), initial/(2f*10), 0f);
                //Delay for 0.1 seconds
                yield return new WaitForSeconds(0.1f);
            }
            //Assign a game object (Lightning) at the calculated location + 0.6f vertical and set no rotations calling it l
            GameObject l = Instantiate(lightningGO, drop + new Vector2(0, 0.6f), Quaternion.identity);
            //From the Bullet script, set the behavior of the lightning to Linger (will stay on screen for 2 seconds)
            l.GetComponent<Bullet>().behavior = Bullet.Behavior.Linger;
            //From the Bullet script, set the damage of the lightning to 30
            l.GetComponent<Bullet>().damage = 30;
            //Delay for 0.01 seconds
            yield return new WaitForSeconds(0.01f);
            //Destroy the lightning strike border, center, and lightning
            Destroy(l.GetComponent<CapsuleCollider2D>());
            yield return new WaitForSeconds(0.3f);
            Destroy(c);
            Destroy(l);
            Destroy(b);
            yield return new WaitForSeconds(2f);
        }
    }

    //Semi-Auto Attack Pattern for Enemy (Shoot bullets at player in a 3-bullet burst)
    private IEnumerator SemiAutoAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a for loop that will create 3 bullets at the calculated angle with a random offset of -0.3 to 0.3
            for (int i = 0; i < 3; i++) {
                //Create a random offset using the Random.Range function
                float randOffset = Random.Range(-0.3f, 0.3f);
                //Create a game object (Bullet) and set its properties (with the random offset in the angle)
                FireProjectile(
                    speed: 6f, 
                    damage: 10, 
                    angle: theta + randOffset, 
                    isWavy: false,
                    pos: transform.position,
                    accelerationMultiplier: 1f,
                    scale: new Vector3(1.25f, 1.25f, 1f), 
                    sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
                );
                //Delay for 0.1 seconds for every bullet created
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    //Splitter Attack Pattern for Enemy (Shoot a bullet at player, split into 2 bullets after a delay)
    private IEnumerator SplitterAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties
            GameObject fired = FireProjectile(
                speed: 4f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
            //Start a coroutine that will split the bullet after a delay (takes in the game object of the bullet and the angle of the bullet as a parameter)
            StartCoroutine(SplitTheBullet(fired, theta));
        }
    }

    //Coroutine that splits the bullet into 2 bullets after a delay, for the Splitter Attack Pattern
    private IEnumerator SplitTheBullet(GameObject fired, float theta) {
        //Delay for 0.5 seconds (After the bullet is fired)
        yield return new WaitForSeconds(0.5f);
        //Create 2 bullets at the calculated angle with a different angle offset (one with 0.3 and one with -0.3)
        FireProjectile(
            speed: 6f, 
            damage: 20, 
            angle: theta + 0.3f,
            isWavy: false,
            pos: fired.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(1f, 1f, 1f), 
            sprite: enemyBulletSpriteDict[BulletSprites.Basic]
        );
        FireProjectile(
            speed: 6f, 
            damage: 20, 
            angle: theta - 0.3f,
            isWavy: false,
            pos: fired.transform.position,
            accelerationMultiplier: 1f, 
            scale: new Vector3(1f, 1f, 1f), 
            sprite: enemyBulletSpriteDict[BulletSprites.Basic]
        );
        //Destroy the original bullet
        Destroy(fired);
    }

    //Accelerator Attack Pattern for Enemy (Shoot a bullet at player, accelerate the bullet after being fired)
    private IEnumerator AcceleratorAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties (acceleration multiplier is set to 1.04, which will increase the speed of the bullet by 4% every frame)
            FireProjectile(
                speed: 2f, 
                damage: 20, 
                angle: theta, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1.04f,
                scale: new Vector3(1f, 1f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.Basic]
            );
        }
    }

    //Tracking Attack Pattern for Enemy (Shoot a bullet at player by predicting the player's position)
    private IEnumerator TrackingAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(0.5f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile (with the predicted position using the CalculatePlayerPos function)
            Vector2 lookDirection = player.transform.position + CalculatePlayerPos(40) - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties
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

    //Shotgun Attack Pattern for Enemy (Shoot 5 bullets at player with different angles)
    private IEnumerator ShotgunAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 2 seconds (Projectile's fire rate)
            yield return new WaitForSeconds(2f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Creates a for loop that will create 5 bullets with different angles (one with the calculated angle and 4 with different angles offset by 0.2)
            for (int i = -2; i < 3; i++) {
                //Create a game object (Bullet) and set its properties (scale is set to 1.25 to make the bullet bigger, the angle is offset by 0.2 for each bullet)
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

    //Machine Gun Attack Pattern for Enemy (Rapidly Shoot bullets at player with a random angle offset)
    private IEnumerator MachineGunAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 0.3 seconds (Projectile's fire rate)
            yield return new WaitForSeconds(0.3f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Generate random offset for the angle of the bullet (between -0.15 and 0.15)
            float randOffset = Random.Range(-0.15f, 0.15f);
            //Create a game object (Bullet) and set its properties (scale is set to 1.25 to make the bullet bigger, the angle is offset by the random offset)
            FireProjectile(
                speed: 5f, 
                damage: 6, 
                angle: theta + randOffset, 
                isWavy: false,
                pos: transform.position,
                accelerationMultiplier: 1f,
                scale: new Vector3(1.25f, 1.25f, 1f), 
                sprite: enemyBulletSpriteDict[BulletSprites.MachineGun]
            );
        }
    }

    //FatShot Attack Pattern for Enemy (Shoot a bullet at player with a bigger size with prediction of the player's position)
    private IEnumerator FatShotAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 2 seconds (Projectile's fire rate)
            yield return new WaitForSeconds(2f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile (with the predicted position using the CalculatePlayerPos function)
            Vector2 lookDirection = player.transform.position + CalculatePlayerPos(40) - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties (scale is set to 2.25 to make the bullet bigger)
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

    //Projectile Trail Attack Pattern for Enemy (Creates a bullet on it path that follows the player) [Landmine]
    private IEnumerator ProjectileTrailAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties (scale is set to 2 to make the bullet bigger and the speed is set to 0 to make the bullet not move but just stay on the enemy's path)
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

    //Wavy Attack Pattern for Enemy (Shoot a bullet at player with a wavy path)
    private IEnumerator WavyAttackPattern() {
        //While enemy is alive, loop continues
        while (true) {
            //Delay for 1 second (Projectile's fire rate)
            yield return new WaitForSeconds(1f);
            //Gets a Vector2 of the difference between the player's position and the enemy's position to calculate the angle of the projectile
            Vector2 lookDirection = player.transform.position - transform.position;
            //Calculate the angle of the projectile
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            //Create a game object (Bullet) and set its properties (scale is set to 1 to make the bullet normal size and the isWavy is set to true to make the bullet wavy)
            //The wavy bullet behavior is handled in the Bullet script
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
        //Allows the enemy to look at the player
        Vector2 lookDirection = player.transform.position - transform.position;
        //Calculate the angle of enemy facing the player
        float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
        //Rotate the enemy to face the player (using the angle)
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        //Set enemy's movement type
        movementTypeDict[movementType]();
    }
    
    //Chase Player Movement Type makes 
    private void ChasePlayer() {
        rb.velocity = transform.right * chaseSpeed;
    }

    private void FleePlayer() { 
        rb.velocity = transform.right * -chaseSpeed;
    }

    private void Stationary() { 
        rb.velocity = Vector2.zero;
    }

    private void Jumping() {
        float timeNow = Time.time;
        if (timeNow % 3f < 0.5) {
            rb.velocity = transform.right * chaseSpeed * 3f;
        }
        else { 
            rb.velocity = transform.right * chaseSpeed / 3f;
        }
    }

    private void Support() { 
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        
        float minDistance = float.MaxValue;
        foreach (GameObject enemy in enemies) {
            float distance = DistFormula(transform.position, enemy.transform.position);
            if (distance < minDistance) {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null) {
            Vector2 lookDirection = nearestEnemy.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
            rb.velocity = transform.right * chaseSpeed;
        }
    }
    
    private void RandomMovement() { 
        float timeNow = Time.time;
        if (timeNow % 3f < 0.01) { 
            Vector2 lookDirection = player.transform.position - transform.position;
            float theta = Mathf.Atan2(lookDirection.y, lookDirection.x);
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        }
    }

    private void MaintainDistance() { 
        if (DistFormula(transform.position, player.transform.position) > 6f) {
            ChasePlayer();
        } else if (DistFormula(transform.position, player.transform.position) < 5f) {
            FleePlayer();
        } else {
            rb.velocity = Vector2.zero;
        }
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
    
    private float DistFormula(Vector3 pos1, Vector3 pos2) {
        return Mathf.Sqrt((pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y));
    }
}
