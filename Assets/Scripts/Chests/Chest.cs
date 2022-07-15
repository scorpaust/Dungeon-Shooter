using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{
	#region Tooltip
	[Tooltip("Set this to the colour to be used for the materialization effect")]
	#endregion Tooltip
	[ColorUsage(false, true)]
	[SerializeField] private Color materializeColor;

	#region Tooltip
	[Tooltip("Set this to the time it will take to materialize the chest")]
	#endregion Tooltip
	[SerializeField] private float materializeTime = 3f;

	#region Tooltip
	[Tooltip("Populate with the item spawn point transform")]
	#endregion Tooltip
	[SerializeField] private Transform itemSpawnPoint;

	private int healthPercent;

	private int ammoPercent;

	private WeaponDetailsSO weaponDetails;

	private Animator animator;

	private SpriteRenderer spriteRenderer;

	private MaterializeEffect materializeEffect;

	private bool isEnabled = false;

	private ChestState chestState = ChestState.closed;

	private GameObject chestItemGameObject;

	private ChestItem chestItem;

	private TextMeshPro messageTextTMP;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		spriteRenderer = GetComponent<SpriteRenderer>();

		materializeEffect = GetComponent<MaterializeEffect>();

		messageTextTMP = GetComponentInChildren<TextMeshPro>();
	}

	public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent)
	{
		this.healthPercent = healthPercent;

		this.weaponDetails = weaponDetails;

		this.ammoPercent = ammoPercent;

		if (shouldMaterialize)
		{
			StartCoroutine(MaterializeChest());
		}
		else
		{
			EnableChest();
		}
	}

	private IEnumerator MaterializeChest()
	{
		SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

		yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

		EnableChest();
	}

	private void EnableChest()
	{
		isEnabled = true;
	}

	public void UseItem()
	{
		if (!isEnabled) return;

		switch (chestState)
		{
			case ChestState.closed:
				OpenChest();
				break;
			case ChestState.healthItem:
				CollectHealthItem();
				break;
			case ChestState.ammoItem:
				CollectAmmoItem();
				break;
			case ChestState.weaponItem:
				CollectWeaponItem();
				break;
			case ChestState.empty:
				return;
			default:
				return;
		}
	}

	private void OpenChest()
	{
		animator.SetBool(Settings.use, true);

		// chest open sound effect
		SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);

		// Check if player already has the weapon - if so set weapon to null
		if (weaponDetails != null)
		{
			if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
			{
				weaponDetails = null;
			}
		}

		UpdateChestState();
	}

	private void UpdateChestState()
	{
		if (healthPercent != 0)
		{
			chestState = ChestState.healthItem;

			InstantiateHealthItem();
		}

		else if (ammoPercent != 0)
		{
			chestState = ChestState.ammoItem;

			InstantiateAmmoItem();
		}

		else if (weaponDetails != null)
		{
			chestState = ChestState.weaponItem;

			InstantiateWeaponItem();
		}

		else
		{
			chestState = ChestState.empty;
		}
	}

	private  void InstantiateItem()
	{
		chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

		chestItem = chestItemGameObject.GetComponent<ChestItem>();
	}

	private void InstantiateHealthItem()
	{
		InstantiateItem();

		chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
	}

	private void CollectHealthItem()
	{
		// Check if item exists and has been materialized
		if (chestItem == null || !chestItem.isItemMaterialized) return;

		// Add health to player
		GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

		// Play pickup sound effect
		SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickup);

		healthPercent = 0;

		Destroy(chestItemGameObject);

		UpdateChestState();
	}

	private void InstantiateAmmoItem()
	{
		InstantiateItem();

		chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
	}

	private void CollectAmmoItem()
	{
		// Check if item exists and has been materialized
		if (chestItem == null || !chestItem.isItemMaterialized) return;

		Player player = GameManager.Instance.GetPlayer();

		// Update ammo for current weapon
		player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

		// Play pickup sound effect
		SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);

		ammoPercent = 0;

		Destroy(chestItemGameObject);

		UpdateChestState();
	}

	private void InstantiateWeaponItem()
	{
		InstantiateItem();

		chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor);
	}

	private void CollectWeaponItem()
	{
		// Check if item exists and has been materialized
		if (chestItem == null || !chestItem.isItemMaterialized) return;

		// If the player doesn't already has the weapon, then add to the player
		if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails))
		{
			// Add weapon to player
			GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

			// Play pickup sound effect
			SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
		}
		else
		{
			// Display message saying you already have the weapon
			StartCoroutine(DisplayMessage("WEAPON\nALREADY\nEQUIPPED", 5f));	
		}

		weaponDetails = null;

		Destroy(chestItemGameObject);

		UpdateChestState();
	}

	private IEnumerator DisplayMessage(string messageText, float messageDisplayTime)
	{
		messageTextTMP.text = messageText;

		yield return new WaitForSeconds(messageDisplayTime);

		messageTextTMP.text = "";
	}
}
