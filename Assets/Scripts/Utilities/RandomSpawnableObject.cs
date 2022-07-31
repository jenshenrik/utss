using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    private struct ChanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int ratioValueTotal;
    private List<ChanceBoundaries> chanceBoundariesList = new List<ChanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        var upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        T spawnableObject = default(T);

        foreach (var spawnableObjectsByLevel in spawnableObjectsByLevelList)
        {
            if (spawnableObjectsByLevel.dungeonLevel == GameManager.Instance.GetCurrentDungeonLevel())
            {
                foreach (var spawnableObjectRatio in spawnableObjectsByLevel.spawnableObjectRatioList)
                {
                    var lowerBoundary = upperBoundary + 1;
                    upperBoundary = lowerBoundary + spawnableObjectRatio.ratio - 1;
                    ratioValueTotal += spawnableObjectRatio.ratio;

                    chanceBoundariesList.Add(new ChanceBoundaries
                    {
                        spawnableObject = spawnableObjectRatio.dungeonObject,
                        lowBoundaryValue = lowerBoundary,
                        highBoundaryValue = upperBoundary
                    });
                }
            }
        }

        if (chanceBoundariesList.Count == 0) return default(T);

        var lookupValue = Random.Range(0, ratioValueTotal);

        foreach (var spawnChance in chanceBoundariesList)
        {
            if (lookupValue >= spawnChance.lowBoundaryValue && lookupValue <= spawnChance.highBoundaryValue)
            {
                spawnableObject = spawnChance.spawnableObject;
                break;
            }
        }

        return spawnableObject;
    }
}