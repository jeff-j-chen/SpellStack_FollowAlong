using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    void ChangeHealthBy(int amount);
}

public class Bullet : MonoBehaviour {
    public int damage;
    private void Start() {
        
    }

    private void Update() {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject g = other.gameObject;
        if (g.GetComponent<IDamageable>() != null) {
            g.GetComponent<IDamageable>().ChangeHealthBy(damage);
        }
        Destroy(gameObject);
    }
}
