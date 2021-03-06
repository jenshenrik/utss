using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    // [HideInInspector]
    public EnemyDetailsSO enemyDetails;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector]
    public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector]
    public IdleEvent idleEvent;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    public Animator animator;
    [HideInInspector]
    private SpriteRenderer[] spriteRendererArray;

    private void Awake()
    {
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
