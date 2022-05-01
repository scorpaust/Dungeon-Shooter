using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
	#region Header DEAL DAMAGE
	[Space(10)]
	[Header("DEAL DAMAGE")]
	#endregion Header DEAL DAMAGE
	#region Tooltip
	[Tooltip("The contact damage to deal (overriden by the receiver)")]
	#endregion Tooltip
	[SerializeField] private int contactDamageAmount;
	#region Tooltip
	[Tooltip("Specify what layers should be on to receive contact damage")]
	#endregion Tooltip
	[SerializeField] private LayerMask layerMask;

	private bool isColliding = false;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// If already colliding with something return
		if (isColliding) return;

		ContactDamage(collision);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		// If already colliding with something return
		if (isColliding) return;

		ContactDamage(collision);
	}

	private void ContactDamage(Collider2D collision)
	{
		// If the collision object isn't in the specified layer then return (used bitwise comparison)
		int collisionObjectLayerMask = (1 << collision.gameObject.layer);

		if ((layerMask.value & collisionObjectLayerMask) == 0)
			return;

		// Check to see if the colliding object should take contact damage
		ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

		if (receiveContactDamage != null)
		{
			isColliding = true;

			// Reset the contact collision after set time
			Invoke("ResetContactCollision", Settings.contactDamageCollisionResetDelay);

			receiveContactDamage.TakeContactDamage(contactDamageAmount);
		}
	}

	private void ResetContactCollision()
	{
		isColliding = false;
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
	}

#endif

	#endregion Validation
}
