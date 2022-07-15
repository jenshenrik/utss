using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    [Tooltip("The name of the enemy")]
    public string enemyName;

    [Tooltip("The prefab for the enemy")]
    public GameObject enemyPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
    }
#endif
}
