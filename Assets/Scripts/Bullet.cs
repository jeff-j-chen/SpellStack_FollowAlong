using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int damage;
    private void Start() {
        
    }

    private void Update() {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject g = other.gameObject;
        if (g.tag == "Enemy") {
            g.GetComponent<Enemy>().ChangeHealthBy(damage);
        }
        print(g.name);
        Destroy(gameObject);
    }
}
