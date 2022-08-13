using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour {
    [SerializeField] private int health = 100;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    // player's current horizontal and vertical input
    [SerializeField] private float movementMultiplier = 0.1f;
    // multiplier to prevent player from flying off the screen
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private TextMeshProUGUI healthText;
    private float moveLimiter = 1.414f;
    // sqrt2 to counteract diagonal movement (pythagorean)

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        // input should be read in update, every frame
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        // make sure to capitalize and use GetAxisRaw
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
            print("took collision damage!");
        }
    }
}
