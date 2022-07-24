using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("Populate with the enter the dungeon play button gameobject")]
    #endregion Tooltip
    [SerializeField] private GameObject playButton;

    #region Tooltip
    [Tooltip("Populate with the enter the dungeon quit button gameobject")]
    #endregion Tooltip
    [SerializeField] private GameObject quitButton;

    #region Tooltip
    [Tooltip("Populate with the high scores button gameobject")]
    #endregion
    [SerializeField] private GameObject highScoresButton;

    #region Tooltip
    [Tooltip("Populate with the instructions button gameobject")]
    #endregion
    [SerializeField] private GameObject instructionsButton;

    #region Tooltip
    [Tooltip("Populate with the return to main menu button gameobject")]
    #endregion
    [SerializeField] private GameObject returnToMainMenuButton;

    private bool isInstructionsSceneLoaded = false;

    private bool isHighScoresSceneLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    public void PlayGame()
	{
        SceneManager.LoadScene("MainGameScene");
	}

    public void LoadHighScores()
    {
        playButton.SetActive(false);

        quitButton.SetActive(false);

        highScoresButton.SetActive(false);

        instructionsButton.SetActive(false);

        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        // Load High Score scene additively
        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionsSceneLoaded)
		{
            SceneManager.UnloadSceneAsync("InstructionsScene");

            isInstructionsSceneLoaded = false;
		}
        playButton.SetActive(true);

        quitButton.SetActive(true);

        highScoresButton.SetActive(true);

        instructionsButton.SetActive(true);

        // Load character selector scene additively
        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    public void LoadInstructions()
	{
        playButton.SetActive(false);

        quitButton.SetActive(false);

        highScoresButton.SetActive(false);

        instructionsButton.SetActive(false);

        isInstructionsSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        // Load instructions scene addictively
        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
	}

    public void QuitGame()
	{
        Application.Quit();
	}

    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoresButton), highScoresButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
    }
#endif
    #endregion
}
