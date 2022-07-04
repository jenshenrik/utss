using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public Health health;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        // Load components
        activeWeapon = GetComponent<ActiveWeapon>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        animator = GetComponent<Animator>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        health = GetComponent<Health>();
        idleEvent = GetComponent<IdleEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
    }


    /// <summary>
    /// Initialize the player
    /// </summary>
    public void Initialize(PlayerDetailsSO playerDetails)
    {
        this.playerDetails = playerDetails;

        CreatePlayerStartingWeapons();

        // Set player starting health
        SetPlayerHealth();
    }

    private void CreatePlayerStartingWeapons()
    {
        weaponList.Clear();

        foreach (var weaponDetailsSO in playerDetails.startingWeaponList)
        {
            AddWeaponToPlayer(weaponDetailsSO);
        }
    }

    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetailsSO)
    {
        var weapon = new Weapon
        {
            weaponDetails = weaponDetailsSO,
            weaponReloadTimer = 0f,
            weaponClipRemainingAmmo = weaponDetailsSO.weaponClipAmmoCapacity,
            weaponRemainingAmmo = weaponDetailsSO.weaponAmmoCapacity,
            isWeaponReloading = false
        };

        weaponList.Add(weapon);
        weapon.weaponListPosition = weaponList.Count;

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }

    /// <summary>
    /// Set player health from playerDetails SO
    /// </summary>
    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

}
