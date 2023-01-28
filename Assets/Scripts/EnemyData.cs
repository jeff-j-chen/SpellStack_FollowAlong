using UnityEngine;
using UnityEngine.Events;
using System.Collections;


[System.Serializable]
public class EnemyData {
    public int health;
    public float chaseSpeed;
    public float attackWaitTime;

    public UnityEvent MovementFunc;
    // public MovementFuncDelegate MovementFunc;

    public UnityEvent AttackPattern;
    // public AttackPatternDelegate AttackPattern;

    public Color color;
    public Sprite sprite;
}