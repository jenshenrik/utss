using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    [Tooltip("Populate with image component on the child WeaponImage game object")]
    [SerializeField]
    private Image weaponImage;

    [Tooltip("Populate with the Transform from the child AmmoHolder game object")]
    [SerializeField] private Transform ammoHolderTransform;

    [Tooltip("Populate with the TMP Text component on the child ReloadText game object")]
    [SerializeField] private TextMeshProUGUI reloadText;

    [Tooltip("Populate with the TMP Text component on the child AmmoRemainingText game object")]
    [SerializeField] private TextMeshProUGUI ammoRemainingText;

    [Tooltip("Populate with the TMP Text component on the child WeaponNameText game object")]
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [Tooltip("Populate with the Transform on the child ReloadBar game object")]
    [SerializeField] private Transform reloadBar;

    [Tooltip("Populate with the Image component on the child BarImage game object")]
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void Start()
    {
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        // If set weapon is still reloading then update reload bar
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = $"({weapon.weaponListPosition}) {weapon.weaponDetails.weaponName.ToUpper()}";
    }

    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = $"{weapon.weaponRemainingAmmo.ToString()} / {weapon.weaponDetails.weaponAmmoCapacity.ToString()}";
        }
    }

    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            var ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);
            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }
    }

    private void ClearAmmoLoadedIcons()
    {
        foreach (var ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity) return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private void UpdateReloadText(Weapon weapon)
    {
        if (!weapon.weaponDetails.hasInfiniteClipCapacity && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();
            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
            StopCoroutine(blinkingReloadTextCoroutine);
    }

    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(.3f);

            reloadText.text = "";
            yield return new WaitForSeconds(.3f);
        }
    }

    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
            StopCoroutine(reloadWeaponCoroutine);
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon)
    {
        barImage.color = Color.red;

        while (weapon.isWeaponReloading)
        {
            var barFill = weapon.weaponReloadTimer / weapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion
}
