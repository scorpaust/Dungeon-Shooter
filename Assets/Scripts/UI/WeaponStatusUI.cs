using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class WeaponStatusUI : MonoBehaviour
{
	#region Header OBJECT REFERENCES
	[Space(10)]
	[Header("OBJECT REFERENCES")]
	#endregion
	#region Tooltip
	[Tooltip("Populate with Image component on the child weaponImage gameObject")]
	#endregion Tooltip
	[SerializeField] private Image weaponImage;
	#region Tooltip
	[Tooltip("Populate with Transform from the child AmmoHolder gameObject")]
	#endregion Tooltip
	[SerializeField] private Transform ammoHolderTransform;
	#region Tooltip
	[Tooltip("Populate with the TextMeshPro text component on the child ReloadText gameObject")]
	#endregion Tooltip
	[SerializeField] private TextMeshProUGUI reloadText;
	#region Tooltip
	[Tooltip("Populate with the TextMeshPro text component on the child AmmoRemainingText gameObject")]
	#endregion Tooltip
	[SerializeField] private TextMeshProUGUI ammoRemainingText;
	#region Tooltip
	[Tooltip("Populate with the TextMeshPro text component on the child WeaponNameText gameObject")]
	#endregion Tooltip
	[SerializeField] private TextMeshProUGUI weaponNameText;
	#region Tooltip
	[Tooltip("Populate with the RectTransform component of the child gameObject ReloadBar")]
	#endregion Tooltip
	[SerializeField] private Transform reloadBar;
	#region Tooltip
	[Tooltip("Populate with the Image component of the child gameObject BarImage")]
	#endregion Tooltip
	[SerializeField] private Image barImage;

	private Player player;
	private List<GameObject> ammoIconList = new List<GameObject>();
	private Coroutine reloadWeaponCoroutine;
	private Coroutine blinkingReloadTextCoroutine;

	private void Awake()
	{
		player = GameManager.Instance.GetPlayer();
	}

	private void OnEnable()
	{
		// Subscribe to set active weapon event
		player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

		// Subscribe to weapon fired event
		player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

		// Subscribe to reload weapon event
		player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

		// Subscribe to weapon reloaded event
		player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
	}

	private void OnDisable()
	{
		// Unsubscribe from set active weapon event
		player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

		// Unsubscribe from weapon fired event
		player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

		// Unsubscribe from reload weapon event
		player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

		// Unsubscribe from weapon reloaded event
		player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
	}

	private void Start()
	{
		SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
	}

	private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
	{
		SetActiveWeapon(setActiveWeaponEventArgs.weapon);
	}

	private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
	{
		WeaponFired(weaponFiredEventArgs.weapon);
	}

	private void WeaponFired(Weapon weapon)
	{
		UpdateAmmoText(weapon);

		UpdateAmmoLoadedIcons(weapon);

		UpdateReloadText(weapon);
	}

	private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
	{
		UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
	}

	private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
	{
		WeaponReloaded(weaponReloadedEventArgs.weapon);
	}

	private void WeaponReloaded(Weapon weapon)
	{
		// if weapon reloaded is the current weapon
		if (player.activeWeapon.GetCurrentWeapon() == weapon)
		{
			UpdateReloadText(weapon);

			UpdateAmmoText(weapon);

			UpdateAmmoLoadedIcons(weapon);

			ResetWeaponReloadBar();
		}
	}

	private void SetActiveWeapon(Weapon weapon)
	{
		UpdateActiveWeaponImage(weapon.weaponDetails);

		UpdateActiveWeaponName(weapon);

		UpdateAmmoText(weapon);

		UpdateAmmoLoadedIcons(weapon);

		// if set weapon is still reloading then update the reload bar
		if (weapon.isWeaponReloading)
		{
			UpdateWeaponReloadBar(weapon);
		}
		else
		{
			ResetWeaponReloadBar();
		}

		UpdateReloadText(weapon);
	}

	private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
	{
		weaponImage.sprite = weaponDetails.weaponSprite;
	}

	private void UpdateActiveWeaponName(Weapon weapon)
	{
		weaponNameText.text = "<" + weapon.weaponListPosition + "> " + weapon.weaponDetails.weaponName.ToUpper();
	}

	private void UpdateAmmoText(Weapon weapon)
	{
		if (weapon.weaponDetails.hasInfiniteAmmo)
		{
			ammoRemainingText.text = "INFINITE AMMO";
		}
		else
		{
			ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
		}
	}

	private void UpdateAmmoLoadedIcons(Weapon weapon)
	{
		ClearAmmoLoadedIcons();

		for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
		{
			// Instantiate ammo icon prefab
			GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

			ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

			ammoIconList.Add(ammoIcon);
		}
	}

	private void ClearAmmoLoadedIcons()
	{
		// Loop through icon gameObjects and destroy
		foreach (GameObject ammoIcon in ammoIconList)
		{
			Destroy(ammoIcon);
		}

		ammoIconList.Clear();
	}

	private void UpdateWeaponReloadBar(Weapon weapon)
	{
		if (weapon.weaponDetails.hasInfiniteClipCapacity)
			return;

		StopReloadWeaponCoroutine();

		UpdateReloadText(weapon);

		reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
	}

	private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
	{
		// Set the reload bar to red
		barImage.color = Color.red;

		// Animate the weapon reload bar
		while (currentWeapon.isWeaponReloading)
		{
			// update the reload bar
			float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

			// update bar fill
			reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

			yield return null;
		}
	}

	private void ResetWeaponReloadBar()
	{
		StopReloadWeaponCoroutine();

		// Set bar color as green
		barImage.color = Color.green;

		// Set bar scale to 1
		reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void StopReloadWeaponCoroutine()
	{
		// Stop any active weapon reload bar on the UI
		if (reloadWeaponCoroutine != null)
		{
			StopCoroutine(reloadWeaponCoroutine);
		}
	}

	private void UpdateReloadText(Weapon weapon)
	{
		if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
		{
			// Set the reload bar to red
			barImage.color = Color.red;

			StopBlinkingReloadTextCoroutine();

			blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
		}
		else
		{
			StopBlinkingReloadText();
		}
	}

	private IEnumerator StartBlinkingReloadTextRoutine()
	{
		while (true)
		{
			reloadText.text = "RELOAD";

			yield return new WaitForSeconds(0.3f);

			reloadText.text = "";

			yield return new WaitForSeconds(0.3f);
		}
	}

	private void StopBlinkingReloadText()
	{
		StopBlinkingReloadTextCoroutine();

		reloadText.text = "";
	}

	private void StopBlinkingReloadTextCoroutine()
	{
		if (blinkingReloadTextCoroutine != null)
		{
			StopCoroutine(blinkingReloadTextCoroutine);
		}
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);

		HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);

		HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);

		HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);

		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);

		HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);

		HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
	}

#endif

	#endregion Validation
}
