using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Basic,
    Fast
}

public enum MovementType
{
    Chase,
    Flee,
    Stationary,
    Jumping,
    Support,
    Maintain,
    Random
}

public delegate void MovmentFunc(Enemy enemy);
public delegate IEnumerator AttackFunc(Enemy enemy);

[System.Serializable]

public class EnemyData
{
    public int healtH;
    public float chaseSpeed;
    public float reloadTime;
    public Color enemyColor;
    public AttackFunc AttackFunction;
    public MovmentFunc MovementFunction;
}

