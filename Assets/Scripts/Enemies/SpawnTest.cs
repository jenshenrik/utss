using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public RoomTemplateSO roomTemplateSO;
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private GameObject instantiatedEnemy;

    private void Start()
    {
        testLevelSpawnList = roomTemplateSO.enemiesByLevelList;

        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (instantiatedEnemy != null)
            {
                Destroy(instantiatedEnemy);
            }

            var enemyDetails = randomEnemyHelperClass.GetItem();

            if (enemyDetails != null)
            {
                instantiatedEnemy = Instantiate(
                    enemyDetails.enemyPrefab,
                    HelperUtilities.GetSpawnPositionNearestToPlayer(HelperUtilities.GetMouseWorldPosition()),
                    Quaternion.identity);
            }
        }
    }
}
