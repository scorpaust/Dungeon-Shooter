using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	#region Tooltip
	[Tooltip("The player WeaponShootPosition gameObject in the hierarchy")]
	#endregion Tooltip
	[SerializeField] private Transform weaponShootPosition;

	[SerializeField] private MovementDetailsSO movementDetailsSO;

	private Player player;

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
	}

	private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
	{
		Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

		weaponDirection = (mouseWorldPosition - weaponShootPosition.position);

		Vector3 playerDirection = (mouseWorldPosition - transform.position);

		weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

		playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

		playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

		player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
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
		HelperUtilities.ValidateCheckNullValues(this, nameof(movementDetailsSO), movementDetailsSO);
	}

#endif

	#endregion Validation
}
