using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float chaseSpeed = 0.3f;
    [SerializeField] private Color basicEnemyColor;
    [SerializeField] private int health = 100;
    // whatever color necessary to differentiate player and enemy

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        GetComponent<SpriteRenderer>().color = basicEnemyColor;
        // student should be able to do ^ on their own
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
