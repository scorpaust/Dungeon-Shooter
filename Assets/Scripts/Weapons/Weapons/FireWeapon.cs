using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
	private float firePrechargeTimer = 0f;

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
		// Handle weapon precharge timer
		WeaponPrecharge(fireWeaponEventArgs);

		// Weapon fire
		if (fireWeaponEventArgs.fire)
		{
			// Test if weapon is ready to fire
			if (IsWeaponReadyToFire())
			{
				FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

				ResetCooldownTimer();

				ResetPrechargeTimer();
			}
		}
	}

	private void WeaponPrecharge(FireWeaponEventArgs fireWeaponEventArgs)
	{
		// Weapon precharge
		if (fireWeaponEventArgs.firePreviousFrame)
		{
			// Decrease precharge timer if fire button held previous frame
			firePrechargeTimer -= Time.deltaTime;
		}
		else
		{
			// Reset the precharge timer
			ResetPrechargeTimer();
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

		// If the weapon isn't precharged or is cooling down return false
		if (fireRateCooldownTimer > 0f || firePrechargeTimer > 0f)
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
			// Fire ammo routine
			StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
		}
	}

	private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
	{
		int ammoCounter = 0;

		// Get random ammo per shot
		int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

		// Get random interval between ammo
		float ammoSpawnInterval;

		if (ammoPerShot > 1)
		{
			ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
		}
		else
		{
			ammoSpawnInterval = 0f;
		}

		// Loop for number of ammo per shot
		while (ammoCounter < ammoPerShot)
		{
			ammoCounter++;

			// Get ammo prefab from array
			GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

			// Get random speed value
			float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

			// Get gameObject with IFireable component
			IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

			// Initialise Ammo
			ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

			// Wait for ammo per shot timegap
			yield return new WaitForSeconds(ammoSpawnInterval);
		}

		// Reduce ammo clip count if not infinite clip capacity
		if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
		{
			activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;

			activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
		}

		// Call weapon fired event
		weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());

		// Weapon fired sound effect
		WeaponSoundEffect();
	}

	private void ResetCooldownTimer()
	{
		// Reset cooldown timer
		fireRateCooldownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
	}

	private void ResetPrechargeTimer()
	{
		// Reset precharge timer
		firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
	}

	private void WeaponSoundEffect()
	{
		if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
		{
			SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
		}
	}
}
