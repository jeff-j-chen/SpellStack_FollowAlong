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
public delegate void AttackFunc(Enemy enemy);

[System.Serializable]
public class EnemyData {
    public int health;
    public float chaseSpeed;
    public float attackWaitTime;

    public AttackPattern attackPattern;
    public AttackFunc attackFunc;

    public MovementPattern movementPattern;
    public MovementFunc movementFunc;
    
    public Color color;
}