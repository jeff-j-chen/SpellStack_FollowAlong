using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum AttackPattern {
    BasicAttackPattern,
    FastAttackPattern,
} 

public enum MovementPattern {
    ChasePlayer,
    FleePlayer,
    Stationary,
    Jumping,
    Support,
    MaintainDistance,
}

public delegate void MovementFunc(Enemy enemy);
public delegate IEnumerator AttackFunc(Enemy enemy);

[System.Serializable]
public class EnemyData {
    public int health;
    public float chaseSpeed;
    public float attackWaitTime;
    public Color color;

    public AttackPattern attackPattern;
    public MovementPattern movementPattern;
}