using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

	private void Awake()
	{
		// Load components
		enemy = GetComponent<Enemy>();
	}

	private void OnEnable()
	{
		// Subscribe to movement event
		enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

		// Subscribe to idle event
		enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
	}

	private void OnDisable()
	{
		// Unsubscribe from movement event
		enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

		// Unsubscribe from idle event
		enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
	}

	private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
	{
		if (enemy.transform.position.x < GameManager.Instance.GetPlayer().transform.position.x)
		{
			SetAimWeaponAnimationParameters(AimDirection.Right);
		}
		else
		{
			SetAimWeaponAnimationParameters(AimDirection.Left);
		}

		SetMovementAnimationParameters();
	}

	private void IdleEvent_OnIdle(IdleEvent idleEvent)
	{
		SetIdleAnimationParameters();
	}

	private void InitializeAimAnimationParameters()
	{
		enemy.animator.SetBool(Settings.aimUp, false);

		enemy.animator.SetBool(Settings.aimUpRight, false);

		enemy.animator.SetBool(Settings.aimUpLeft, false);

		enemy.animator.SetBool(Settings.aimRight, false);

		enemy.animator.SetBool(Settings.aimLeft, false);

		enemy.animator.SetBool(Settings.aimDown, false);
	}

	private void SetMovementAnimationParameters()
	{
		// Set moving
		enemy.animator.SetBool(Settings.isIdle, false);

		enemy.animator.SetBool(Settings.isMoving, true);
	}

	private void SetIdleAnimationParameters()
	{
		// Set idle
		enemy.animator.SetBool(Settings.isMoving, false);

		enemy.animator.SetBool(Settings.isIdle, true);
	}

	private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
	{
		InitializeAimAnimationParameters();

		// Set aimDirection
		switch (aimDirection)
		{
			case AimDirection.Up:
				enemy.animator.SetBool(Settings.aimUp, true);
				break;

			case AimDirection.UpRight:
				enemy.animator.SetBool(Settings.aimUpRight, true);
				break;

			case AimDirection.UpLeft:
				enemy.animator.SetBool(Settings.aimUpLeft, true);
				break;

			case AimDirection.Right:
				enemy.animator.SetBool(Settings.aimRight, true);
				break;

			case AimDirection.Left:
				enemy.animator.SetBool(Settings.aimLeft, true);
				break;

			case AimDirection.Down:
				enemy.animator.SetBool(Settings.aimDown, true);
				break;
		}
	}
}
