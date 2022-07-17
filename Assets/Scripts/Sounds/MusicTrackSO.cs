using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/Music Track")]
public class MusicTrackSO : ScriptableObject
{
	#region Header MUSIC TRACK DETAILS
	[Space(10)]
	[Header("MUSIC TRACK DETAILS")]
	#endregion Header MUSIC TRACK DETAILS

	#region Tooltip
	[Tooltip("The name for the music track")]
	#endregion Tooltip
	public string musicName;

	#region Tooltip
	[Tooltip("The audio clip for the music track")]
	#endregion Tooltip
	public AudioClip musicClip;

	#region Tooltip
	[Tooltip("The volume for the music track")]
	#endregion Tooltip
	[Range(0, 1)]
	public float musicVolume = 1f;

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(musicName), musicName);

		HelperUtilities.ValidateCheckNullValue(this, nameof(musicClip), musicClip);

		HelperUtilities.ValidateCheckPositiveValue(this, nameof(musicVolume), musicVolume, true);
	}

#endif

	#endregion Validation
}
