using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
	#region Header OBJECT REFERENCES
	[Space(10)]
	[Header("OBJECT REFERENCES")]
	#endregion Header OBJECT REFERENCES

	#region Tooltip
	[Tooltip("Populate this with the BoxCollider2D component on the DoorCollider gameObject")]
	#endregion
	[SerializeField] private BoxCollider2D doorCollider;

	[HideInInspector] public bool isBossRoomDoor = false;

	private BoxCollider2D doorTrigger;

	private bool isOpen = false;

	private bool previouslyOpened = false;

	private Animator animator;

	private void Awake()
	{
		doorCollider.enabled = false;

		animator = GetComponent<Animator>();

		doorTrigger = GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(Settings.playertag) || collision.CompareTag(Settings.playerWeapon))
		{
			OpenDoor();
		}
	}

	private void OnEnable()
	{
		animator.SetBool(Settings.open, isOpen);
	}

	public void OpenDoor()
	{
		if (!isOpen)
		{
			isOpen = true;

			previouslyOpened = true;

			doorCollider.enabled = false;

			animator.SetBool(Settings.open, true);
		}
	}

	public void LockDoor()
	{
		isOpen = false;

		doorCollider.enabled = true;

		doorTrigger.enabled = false;

		animator.SetBool(Settings.open, false);
	}

	public void UnlockDoor()
	{
		doorCollider.enabled = false;

		doorTrigger.enabled = true;

		if (previouslyOpened == true)
		{
			isOpen = false;

			OpenDoor();
		}
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
	}

#endif

	#endregion Validation
}
