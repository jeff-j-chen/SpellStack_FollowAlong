using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementMultiplier = 0.1f;
    private float moveLimiter = 1.414f;

    private void Start() {

    }

    private void Update() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() {
        if (horizontal != 0 && vertical != 0) {
            horizontal /= moveLimiter;
            vertical /= moveLimiter;
        }
        transform.position = new Vector2(
            transform.position.x + horizontal * movementMultiplier, 
            transform.position.y + vertical * movementMultiplier
        );
    }
}
