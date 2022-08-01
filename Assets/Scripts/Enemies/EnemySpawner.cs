using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;
    private int currentEnemyCount;
    private int enemiesSpawnedSoFar;
    private int enemyMaxConcurrentSpawnNumber;
    private Room currentRoom;
    private RoomEnemySpawnParameters roomEnemySpawnParameters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        enemiesSpawnedSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // No enemies in corridors or entrances
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // Skip if room was already cleared
        if (currentRoom.isClearedOfEnemies)
            return;

        var currentDungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(currentDungeonLevel);

        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(currentDungeonLevel);

        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearedOfEnemies = true;

            return;
        }

        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        currentRoom.instantiatedRoom.LockDoors();

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        var grid = currentRoom.instantiatedRoom.grid;

        var randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        if (currentRoom.spawnPositionArray.Length > 0)
        {
            // Loop to create all enemies
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // If max concurrent is reached, wait until count gets below it
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                // Spawn an enemy
                var cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    private float GetEnemySpawnInterval()
    {
        return Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval);
    }

    private int GetConcurrentEnemies()
    {
        return Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies);
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        enemiesSpawnedSoFar++;
        currentEnemyCount++;

        var dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        var enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);
    }
}
