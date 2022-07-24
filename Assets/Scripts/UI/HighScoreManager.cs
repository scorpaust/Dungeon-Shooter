using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores highScores = new HighScores();

    protected override void Awake()
	{
		base.Awake();

		LoadScores();
	}

	private void LoadScores()
	{
		BinaryFormatter bf = new BinaryFormatter();

		if (File.Exists(Application.persistentDataPath + "/DungeonShooterHighscores.dat"))
		{
			ClearScoreList();

			FileStream file = File.OpenRead(Application.persistentDataPath + "/DungeonShooterHighscores.dat");

			highScores = (HighScores)bf.Deserialize(file);

			file.Close();
		}
	}

	private void ClearScoreList()
	{
		highScores.scoreList.Clear();
	}

	public void AddScore(Score score, int rank)
	{
		highScores.scoreList.Insert(rank - 1, score);

		// Maintain the maximum number of scores to save
		if (highScores.scoreList.Count > Settings.numberOfHighscoresToSave)
		{
			highScores.scoreList.RemoveAt(Settings.numberOfHighscoresToSave);
		}

		SaveScores();
	}

	private void SaveScores()
	{
		BinaryFormatter bf = new BinaryFormatter();

		FileStream file = File.Create(Application.persistentDataPath + "/DungeonShooterHighscores.dat");

		bf.Serialize(file, highScores);

		file.Close();
	}

	public HighScores GetHighScores()
	{
		return highScores;
	}

	public int GetRank(long playerScore)
	{
		if (highScores.scoreList.Count == 0) return 1;

		int index = 0;

		for (int i = 0; i < highScores.scoreList.Count; i++)
		{
			index++;

			if (playerScore >= highScores.scoreList[i].playerScore)
			{
				return index;
			}
		}

		if (highScores.scoreList.Count < Settings.numberOfHighscoresToSave)
		{
			return (index + 1);
		}

		return 0;
	}
}
