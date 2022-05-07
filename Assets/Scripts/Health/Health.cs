using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
	[SerializeField] private HealthBar healthBar;

    private int startingHealth;
    
    private int currentHealth;
    
    private HealthEvent healthEvent;

	private Player player;

	private Coroutine immunityCoroutine;

	private bool isImmuneAfterHit = false;

	private float immunityTime = 0f;

	private SpriteRenderer spriteRenderer = null;

	private const float spriteFlashInterval = 0.2f;

	private WaitForSeconds waitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;

	[HideInInspector] public Enemy enemy;

	private void Awake()
	{
        // Load components
        healthEvent = GetComponent<HealthEvent>();
	}

	private void Start()
	{
		// Trigger a health event for UI update
		CallHealthEvent(0);

		// Attempt to load enemy / player components
		player = GetComponent<Player>();
		enemy = GetComponent<Enemy>();

		if (player != null)
		{
			if (player.playerDetails.isImmuneAfterHit)
			{
				isImmuneAfterHit = true;

				immunityTime = player.playerDetails.hitImmunityTime;

				spriteRenderer = player.spriteRenderer;
			}
		}
		else if (enemy != null)
		{
			if (enemy.enemyDetails.isImmuneAfterHit)
			{
				isImmuneAfterHit = true;

				immunityTime = enemy.enemyDetails.hitImmunityTime;

				spriteRenderer = enemy.spriteRendererArray[0];
			}
		}

		// Enable the health bar if required
		if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
		{
			healthBar.EnableHealthBar();
		}
		else if (healthBar != null)
		{
			healthBar.DisableHealthBar();
		}
	}

	public void TakeDamage(int damageAmount)
	{
		bool isRolling = false;

		if (player != null)
			isRolling = player.playerControl.isPlayerRolling;

		if (isDamageable && !isRolling)
		{
			currentHealth -= damageAmount;

			CallHealthEvent(damageAmount);

			PostHitImmunity();

			// Set health bar as the percentage of health remaining
			if (healthBar != null)
			{
				healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
			}
		}
	}

	private void PostHitImmunity()
	{
		// Check if game object is active - if not return
		if (gameObject.activeSelf == false)
			return;

		// If there is post hit immunity then
		if (isImmuneAfterHit)
		{
			if  (immunityCoroutine != null)
			{
				StopCoroutine(immunityCoroutine);
			}

			// Flash red and give period of immunity
			immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
		}
	}

	private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
	{
		int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

		isDamageable = false;
		
		while (iterations > 0)
		{
			spriteRenderer.color = new Color(1f, 0f, 0f, 0.8f);

			yield return waitForSecondsSpriteFlashInterval;

			spriteRenderer.color = Color.white;

			yield return waitForSecondsSpriteFlashInterval;

			iterations--;

			yield return null;
		}

		isDamageable = true;
	}

	private void CallHealthEvent(int damageAmount)
	{
		// Trigger health event
		healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
	}

	public void SetStartingHealth(int startingHealth)
	{
        this.startingHealth = startingHealth;

        currentHealth = startingHealth;
	}

    public int GetStartingHealth()
	{
        return startingHealth;
	}
}
