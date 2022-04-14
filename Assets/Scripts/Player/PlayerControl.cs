using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
	[SerializeField] private MovementDetailsSO movementDetailsSO;

	private Player player;

	private bool leftMouseDownPreviousFrame = false;

	private int currentWeaponIndex = 1;

	private float moveSpeed;

	private Coroutine playerRollCoroutine;

	private WaitForFixedUpdate waitForFixedUpdate;

	private bool isPlayerRolling = false;

	private float playerRollCooldownTimer = 0f;

	private void Awake()
	{
		player = GetComponent<Player>();

		moveSpeed = movementDetailsSO.GetMoveSpeed();
	}

	private void Start()
	{
		waitForFixedUpdate = new WaitForFixedUpdate();

		// Set starting weapon
		SetStartingWeapon();

		SetPlayerAnimationSpeed();
	}

	private void SetStartingWeapon()
	{
		int index = 1;

		foreach (Weapon weapon in player.weaponList)
		{
			if (weapon.weaponDetails == player.playerDetails.startingWeapon)
			{
				SetWeaponByIndex(index);
				break;
			}
			index++;
		}
	}

	private void SetPlayerAnimationSpeed()
	{
		player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
	}

	private void Update()
	{
		if (isPlayerRolling) return;

		MovementInput();

		WeaponInput();

		PlayerRollCooldownTimer();
	}

	private void MovementInput()
	{
		float horizontalMovement = Input.GetAxisRaw("Horizontal");

		float verticalMovement = Input.GetAxisRaw("Vertical");

		bool rightMouseButton = Input.GetMouseButtonDown(1);

		Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

		if (horizontalMovement != 0f && verticalMovement != 0f)
		{
			direction *= 0.7f;
		}

		if (direction != Vector2.zero)
		{
			if (!rightMouseButton)
			{
				player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
			}
			else if (playerRollCooldownTimer <= 0f)
			{
				PlayerRoll((Vector3)direction);
			}
		}
		else
		{
			player.idleEvent.CallIdleEvent();
		}
	}

	private void PlayerRoll(Vector3 direction)
	{
		playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
	}

	private IEnumerator PlayerRollRoutine(Vector3 direction)
	{
		float minDistance = 0.2f;

		isPlayerRolling = true;

		Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetailsSO.rollDistance;

		while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
		{
			player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetailsSO.rollSpeed, direction, isPlayerRolling);

			yield return waitForFixedUpdate;
		}

		isPlayerRolling = false;

		playerRollCooldownTimer = movementDetailsSO.rollCoolDownTime;

		player.transform.position = targetPosition;
	}

	private void PlayerRollCooldownTimer()
	{
		if (playerRollCooldownTimer >= 0f)
		{
			playerRollCooldownTimer -= Time.deltaTime;
		}
	}

	private void WeaponInput()
	{
		Vector3 weaponDirection;

		float weaponAngleDegrees, playerAngleDegrees;

		AimDirection playerAimDirection;

		AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

		// Fire weapon input
		FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

		// Reload weapon input
		ReloadWeaponInput();
	}

	private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
	{
		Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

		weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

		Vector3 playerDirection = (mouseWorldPosition - transform.position);

		weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

		playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

		playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

		player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
	}

	private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
	{
		// Fire when left mouse button is clicked
		if (Input.GetMouseButton(0))
		{
			// Trigger fire wepon event
			player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);

			leftMouseDownPreviousFrame = true;
		}
		else
		{
			leftMouseDownPreviousFrame = false;
		}
	}

	private void SetWeaponByIndex(int weaponIndex)
	{
		if (weaponIndex - 1 < player.weaponList.Count)
		{
			currentWeaponIndex = weaponIndex;

			player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
		}
	}

	private void ReloadWeaponInput()
	{
		Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

		// if current weapon is reloading return
		if (currentWeapon.isWeaponReloading) return;

		// remaining ammo is less than clip capacity and not infinite ammo then return
		if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipAmmoCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo) return;

		// if ammo in clip equals clip capacity then return
		if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetails.weaponClipAmmoCapacity) return;

		if (Input.GetKeyDown(KeyCode.R))
		{
			// Call the reload weapon event
			player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		StopPlayerRollRoutine();
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		StopPlayerRollRoutine();
	}

	private void StopPlayerRollRoutine()
	{
		if (playerRollCoroutine != null)
		{
			StopCoroutine(playerRollCoroutine);

			isPlayerRolling = false;
		}
	}

	#region Validation 

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetailsSO), movementDetailsSO);
	}

#endif

	#endregion Validation
}
