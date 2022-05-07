using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D = new Rigidbody2D();

    private MovementByVelocityEvent movementByVelocityEvent;

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
	}

	private void OnEnable()
	{
		movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
	}

	private void OnDisable()
	{
		movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
	}

	private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
	{
		MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
	}

	private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
	{
		rigidbody2D.velocity = moveDirection * moveSpeed;
	}
}
