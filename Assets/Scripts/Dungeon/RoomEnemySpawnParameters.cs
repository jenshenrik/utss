using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    [Tooltip("Defines the dungeon level for this room with regard to how many enemies in total should be spawned")]
    public DungeonLevelSO dungeonLevel;

    [Tooltip(@"The minimum number of enemies to spawn in this room for this dungeon level. 
        The actual number will be a random value between the minimum and maximum.")]
    public int minTotalEnemiesToSpawn;

    [Tooltip(@"The maximum number of enemies to spawn in this room for this dungeon level. 
        The actual number will be a random value between the minimum and maximum.")]
    public int maxTotalEnemiesToSpawn;

    [Tooltip(@"the minimum number of concurrent enemies to spawn in this room for this dungeon level.
        The actual number will be a random value between the minimum and maximun.")]
    public int minConcurrentEnemies;

    [Tooltip(@"the maximum number of concurrent enemies to spawn in this room for this dungeon level.
        The actual number will be a random value between the minimum and maximun.")]
    public int maxConcurrentEnemies;

    [Tooltip(@"The minimum spawn interval in seconds for enemies in this room for this dungeon level.
        The actual value will be a random value between the minimum and maximum.")]
    public int minSpawnInterval;

    [Tooltip(@"The maximum spawn interval in seconds for enemies in this room for this dungeon level.
        The actual value will be a random value between the minimum and maximum.")]
    public int maxSpawnInterval;
}
