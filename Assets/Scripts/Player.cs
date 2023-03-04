using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour, IDamageable {
    [SerializeField] private int health = 100;
    [SerializeField] public float horizontal;
    [SerializeField] public float vertical;
    // player's current horizontal and vertical input
    [SerializeField] public float movementMultiplier = 0.1f;
    // multiplier to prevent player from flying off the screen
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;  
    public float moveLimiter = 1.414f;
    // sqrt2 to counteract diagonal movement (pythagorean)

    private Dictionary<string, int> dictName = new() {
        {"FirstKey", 1},
        {"SecondKey", 2},
    };

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        // input should be read in update, every frame
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        // make sure to capitalize and use GetAxisRaw
        if (Input.GetMouseButton(0)) {
            AttemptAttack();
        }
    }

    private void FixedUpdate() {
        if (horizontal != 0 && vertical != 0) {
            horizontal /= moveLimiter;
            vertical /= moveLimiter;
        }
        // limit player's movement if they are going diagonally
        transform.position = new Vector2(
            transform.position.x + horizontal * movementMultiplier, 
            transform.position.y + vertical * movementMultiplier
        );
        // update the player's position
        rb.velocity = new Vector2(0,0);
        // makes sure that their velocity is 0, otherwise the player will drift around after being pushed
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Contains("enemy")) {
            health -= 10;
            healthText.text = $"Player HP: {health}";
        }
    }

    private float GetAngleToCursor(Vector3 pos) { 
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - pos;
        return Mathf.Atan2(lookDirection.y, lookDirection.x);
    }

    private void AttemptAttack() {
        if (canAttack) {
            canAttack = false;
            StartCoroutine(RefreshAttack());
        }
        else {
            return;
        }
        GameObject fired = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        float theta = GetAngleToCursor(transform.position);
        fired.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, theta * Mathf.Rad2Deg));
        fired.GetComponent<Rigidbody2D>().velocity = fired.transform.right * bulletSpeed;
        print(fired.GetComponent<Rigidbody2D>().velocity);
        fired.GetComponent<Bullet>().damage = 25;
    }

    private IEnumerator RefreshAttack() {
        yield return new WaitForSeconds(1f);
        canAttack = true;   
    }

    public void ChangeHealthBy(int amount) {
        health -= amount;
        if (health < 0) {
            print("player died!");
        }
    }
}
