using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [Tooltip("MovementDetailsSO containing movement details such as speed")]
    [SerializeField]
    private MovementDetailsSO movementDetails;
    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuild;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector]
    public float moveSpeed;
    private bool chasePlayer = false;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();

        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        // Update path rebuild cooldown
        currentEnemyPathRebuild -= Time.deltaTime;

        // Check to see if player is within chase distance
        if (!chasePlayer && GetDistanceToPlayer() < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        // Do nothing if player out of range
        if (!chasePlayer) return;

        // Check cooldown or player moved distance to see if path should be rebuilt
        if (currentEnemyPathRebuild <= 0f || GetDistanceToPlayer() > Settings.playerMoveDistanceToRebuildPath)
        {
            // Reset cooldown and player position
            currentEnemyPathRebuild = Settings.enemyPathRebuildCooldown;
            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            // Move enemy using A*
            // Trigger rebuild of path
            CreatePath();

            // If a path has bene found, move the enemy
            if (movementSteps != null)
            {
                // If enemy is already moving
                if (moveEnemyRoutine != null)
                {
                    // Go idle and stop moving
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                // Move enemy along the path using a coroutine
                moveEnemyRoutine = StartCoroutine(MoveEnemyCoroutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyCoroutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            var nextPosition = movementSteps.Pop();

            // Move until very close to next step
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed,
                    (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        // Go idle when destination is reached
        enemy.idleEvent.CallIdleEvent();
    }

    private void CreatePath()
    {
        var currentRoom = GameManager.Instance.GetCurrentRoom();
        var grid = currentRoom.instantiatedRoom.grid;

        var enemyGridPosition = grid.WorldToCell(transform.position);
        var playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        // If a path was found, remove the first step (current enemy position)
        if (movementSteps != null)
        {
            movementSteps.Pop();
        }
        else
        {
            // If no path was found, go idle
            enemy.idleEvent.CallIdleEvent();
        }
    }

    // Because players can be in collision tile (half collision tiles)
    // find nearest non-collision tile in order for pathfinding to work
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        var playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
        var playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        var adjustedPlayerCellPosition = new Vector2Int(
            playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y
        );

        var obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y];
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        else
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // skip current cell
                    if (j == 0 && i == 0) continue;

                    try
                    {
                        obstacle = currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + i, adjustedPlayerCellPosition.y + j];
                        if (obstacle != 0) return new Vector3Int(playerCellPosition.x + +i, playerCellPosition.y + j, 0);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            // If no non-obstacle position found, just use player position
            return playerCellPosition;
        }
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition());
    }
}
