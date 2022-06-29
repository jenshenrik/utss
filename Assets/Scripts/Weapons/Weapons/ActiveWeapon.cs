using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [Tooltip("Populate with the SpriteRenderer on the child Weapon game object")]
    [SerializeField]
    private SpriteRenderer weaponSpriteRenderer;

    [Tooltip("Populate with the PolygonCollider2D on the child Weapon game object")]
    [SerializeField]
    private PolygonCollider2D weaponPolygonCollider2D;

    [Tooltip("Populate with the transform on the WeaponShootPosition game object")]
    [SerializeField]
    private Transform weaponShootPositionTransform;

    [Tooltip("Populate with the transform on the WeaponEffectPosition game object")]
    [SerializeField]
    private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setWeaponEvent, SetActiveWeaponEventArgs eventArgs)
    {
        SetWeapon(eventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        // if the weapon has a polygon collider and a asprite then set it to the weapon sprite physics shape
        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            var spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();
        }

        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
}
