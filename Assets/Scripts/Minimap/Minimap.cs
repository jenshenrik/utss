using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [Tooltip("Populate with the child Minimap Player gameobject")]
    [SerializeField]
    private GameObject minimapPlayer;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        var cinemachineVirtualCVamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCVamera.Follow = playerTransform;

        var spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
        }
    }

    private void Update()
    {
        if (playerTransform != null && minimapPlayer != null)
        {
            minimapPlayer.transform.position = playerTransform.position;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }
#endif
}
