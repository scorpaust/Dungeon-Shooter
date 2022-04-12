using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float fireRateCooldownTimer = 0f;

    private ActiveWeapon activeWeapon;

    private FireWeaponEvent fireWeaponEvent;

	private ReloadWeaponEvent reloadWeaponEvent;

    private WeaponFiredEvent weaponFiredEvent;

	private void Awake()
	{
		// Load components
		activeWeapon = GetComponent<ActiveWeapon>();

		fireWeaponEvent = GetComponent<FireWeaponEvent>();

		reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();

		weaponFiredEvent = GetComponent<WeaponFiredEvent>();
	}

	private void OnEnable()
	{
		// Subscribe to fire weapon event
		fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
	}

	private void OnDisable()
	{
		// Unsubscribe from the fire weapon event
		fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
	}

	private void Update()
	{
		// Decrease cooldown timer
		fireRateCooldownTimer -= Time.deltaTime;
	}

	private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
	{
		WeaponFire(fireWeaponEventArgs);
	}

	private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
	{
		// Weapon fire
		if (fireWeaponEventArgs.fire)
		{
			// Test if weapon is ready to fire
			if (IsWeaponReadyToFire())
			{
				FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

				ResetCooldownTimer();
			}
		}
	}

	private bool IsWeaponReadyToFire()
	{
		// If there is no ammo and weapon doesn't have infinite ammo then return false
		if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
			return false;

		// If the weapon is reloading return false
		if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
			return false;

		// If the weapon is cooling down return false
		if (fireRateCooldownTimer > 0f)
			return false;

		// If there is no ammo in the clip and the weapon doesn't have infinite clip capacity then return false
		if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
		{
			// Trigger a reload weapon event
			reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

			return false;
		}

		// Weapon is ready to fire - return true
		return true;
	}

	private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
	{
		AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

		if (currentAmmo != null)
		{
			// Get ammo prefab from array
			GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

			// Get random speed value
			float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

			// Get gameObject with IFireable component
			IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

			// Initialise Ammo
			ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

			// Reduce ammo clip count if not infinite clip capacity
			if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
			{
				activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;

				activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
			}

			// Call weapon fired event
			weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
		}
	}

	private void ResetCooldownTimer()
	{
		// Reset cooldown timer
		fireRateCooldownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
	}
}
