using UnityEngine;

public class DisplayHighScoresUI : MonoBehaviour
{
	#region Header OBJECT REFERENCES
	[Space(10)]
	[Header("OBJECT REFERENCES")]
	#endregion Header OBJECT REFERENCES
	#region Tooltip
	[Tooltip("Populate with the child content game object transform component")]
	#endregion Tooltip
	[SerializeField] private Transform contentAnchorTransform;

	private void Start()
	{
		DisplayScores();
	}

	private void DisplayScores()
	{
		HighScores highScores = HighScoreManager.Instance.GetHighScores();

		GameObject scoreGameObject;

		// Loop through scores
		int rank = 0;

		foreach (Score score in highScores.scoreList)
		{
			rank++;

			scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);

			ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

			// Populate
			scorePrefab.rankTMP.text = rank.ToString();

			scorePrefab.nameTMP.text = score.playerName;

			scorePrefab.levelTMP.text = score.levelDescription;

			scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
		}

		scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
	}
}
