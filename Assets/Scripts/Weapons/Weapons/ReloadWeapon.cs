using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]

[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;

    private WeaponReloadedEvent weaponReloadedEvent;

    private SetActiveWeaponEvent setActiveWeaponEvent;

    private Coroutine reloadWeaponCoroutine;

	private void Awake()
	{
		// Load components
		reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();

		weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();

		setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
	}

	private void OnEnable()
	{
		// subscribe to reload weapon event
		reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

		// subscribe to set active weapon event
		setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
	}

	private void OnDisable()
	{
		// unsubscribe from reload weapon event
		reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

		// unsubscribe from set active weapon event
		setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
	}

	private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
	{
		StartReloadWeapon(reloadWeaponEventArgs);
	}

	private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
	{
		if (reloadWeaponCoroutine != null)
		{
			StopCoroutine(reloadWeaponCoroutine);
		}

		reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.topUpAmmoPercent));
	}

	private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent)
	{
		// Play reload sound if there is one
		if (weapon.weaponDetails.weaponReloadingSoundEffect != null)
		{
			SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.weaponReloadingSoundEffect);
		}

		// Set weapon as reloading
		weapon.isWeaponReloading = true;

		// Update reload progress timer
		while(weapon.weaponReloadTimer < weapon.weaponDetails.weaponReloadTime)
		{
			weapon.weaponReloadTimer += Time.deltaTime;

			yield return null;
		}

		// if total ammo has to be increased update
		if (topUpAmmoPercent != 0)
		{
			int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetails.weaponAmmoCapacity * topUpAmmoPercent) / 100f);

			int totalAmmo = weapon.weaponRemainingAmmo + ammoIncrease;

			if (totalAmmo > weapon.weaponDetails.weaponAmmoCapacity)
			{
				weapon.weaponRemainingAmmo = weapon.weaponDetails.weaponAmmoCapacity;
			}
			else
			{
				weapon.weaponRemainingAmmo = totalAmmo;
			}
		}

		// if weapon has infinite ammo then just refil the clip
		if (weapon.weaponDetails.hasInfiniteAmmo)
		{
			weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
		}

		else if (weapon.weaponRemainingAmmo >= weapon.weaponDetails.weaponClipAmmoCapacity)
		{
			weapon.weaponClipRemainingAmmo = weapon.weaponDetails.weaponClipAmmoCapacity;
		}

		else
		{
			weapon.weaponClipRemainingAmmo = weapon.weaponRemainingAmmo;
		}

		// Reset weapon reload timer
		weapon.weaponReloadTimer = 0f;

		// Set weapon as not reloading
		weapon.isWeaponReloading = false;

		// Call weapon reloaded event
		weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
	}

	private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
	{
		if (setActiveWeaponEventArgs.weapon.isWeaponReloading)
		{
			if (reloadWeaponCoroutine != null)
			{
				StopCoroutine(reloadWeaponCoroutine);
			}

			reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(setActiveWeaponEventArgs.weapon, 0));
		}
	}
}
