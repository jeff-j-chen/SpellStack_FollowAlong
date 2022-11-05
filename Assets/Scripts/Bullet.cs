using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    void ChangeHealthBy(int amount);
}

public class Bullet : MonoBehaviour {
    [SerializeField] private Rigidbody2D rb;
    public int damage;
    public float accelerationMultiplier = 1f;
    public bool isWavy;
    public enum Behavior { Break, Linger }
    public Behavior behavior = Behavior.Break;
    private void Start() {
        
    }

    private void Update() {
        rb.velocity *= accelerationMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject g = other.gameObject;
        if (g.GetComponent<IDamageable>() != null) {
            g.GetComponent<IDamageable>().ChangeHealthBy(damage);
        }
        if (g.name is "enemy(Clone)" or "player") {
            switch (behavior) {
                case Behavior.Linger: break;
                case Behavior.Break: Destroy(gameObject); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        Destroy(gameObject);
    }
}
