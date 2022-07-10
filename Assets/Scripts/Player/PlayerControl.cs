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

	[HideInInspector] public bool isPlayerRolling = false;

	private float playerRollCooldownTimer = 0f;

	private bool isPlayerMovementDisabled = false;

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
		// If player movement is disabled then return
		if (isPlayerMovementDisabled) return;

		if (isPlayerRolling) return;

		MovementInput();

		WeaponInput();

		// Process player use item input
		UseItemInput();

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

		// Switch weapon input
		SwitchWeaponInput();

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

	private void SwitchWeaponInput()
	{
		// Switch weapon if mouse scroll wheel selected
		if (Input.mouseScrollDelta.y < 0f)
		{
			PreviousWeapon();
		}

		if (Input.mouseScrollDelta.y > 0f)
		{
			NextWeapon();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetWeaponByIndex(1);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetWeaponByIndex(2);
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetWeaponByIndex(3);
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetWeaponByIndex(4);
		}

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SetWeaponByIndex(5);
		}

		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			SetWeaponByIndex(6);
		}

		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			SetWeaponByIndex(7);
		}

		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			SetWeaponByIndex(8);
		}

		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			SetWeaponByIndex(9);
		}

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			SetWeaponByIndex(10);
		}

		if (Input.GetKeyDown(KeyCode.Minus))
		{
			SetCurrentWeaponToFirstInTheList();
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

	private void NextWeapon()
	{
		currentWeaponIndex++;

		if (currentWeaponIndex > player.weaponList.Count)
		{
			currentWeaponIndex = 1;
		}

		SetWeaponByIndex(currentWeaponIndex);
	}

	private void PreviousWeapon()
	{
		currentWeaponIndex--;

		if (currentWeaponIndex < 1)
		{
			currentWeaponIndex = player.weaponList.Count;
		}

		SetWeaponByIndex(currentWeaponIndex);
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

	private void UseItemInput()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			float useItemRadius = 2f;

			// Get any useable item near the player
			Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

			// Loop through detected items to see if any are useable
			foreach (Collider2D collider2D in collider2DArray)
			{
				IUseable iUsable = collider2D.GetComponent<IUseable>();

				if (iUsable != null)
				{
					iUsable.UseItem();
				}
			}
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

	public void EnablePlayer()
	{
		isPlayerMovementDisabled = false;
	}

	public void DisablePlayer()
	{
		isPlayerMovementDisabled = true;

		player.idleEvent.CallIdleEvent();
	}

	private void SetCurrentWeaponToFirstInTheList()
	{
		// Create new temporary list
		List<Weapon> tempWeaponList = new List<Weapon>();

		// Add the current weapon to first in the temp list
		Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];

		currentWeapon.weaponListPosition = 1;

		tempWeaponList.Add(currentWeapon);

		// Loop through existing weapon list and add - skipping current weapon
		int index = 2;

		foreach (Weapon weapon in player.weaponList)
		{
			if (weapon == currentWeapon) continue;

			tempWeaponList.Add(weapon);

			weapon.weaponListPosition = index;

			index++;
		}

		// Assign new list
		player.weaponList = tempWeaponList;

		currentWeaponIndex = 1;

		// Set current weapon
		SetWeaponByIndex(currentWeaponIndex);
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
