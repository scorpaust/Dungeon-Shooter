using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D = new Rigidbody2D();

    private IdleEvent idleEvent;

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		idleEvent = GetComponent<IdleEvent>();
	}

	private void OnEnable()
	{
		idleEvent.OnIdle += idleEvent_onIdle;
	}

	private void OnDisable()
	{
		idleEvent.OnIdle -= idleEvent_onIdle;
	}

	private void idleEvent_onIdle(IdleEvent idleEvent)
	{
		MoveRigidBody();
	}

	private void MoveRigidBody()
	{
		rigidbody2D.velocity = Vector2.zero;
	}
}
