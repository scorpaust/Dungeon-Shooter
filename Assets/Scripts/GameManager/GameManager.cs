using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
	#region Header DUNGEON LEVELS


	#endregion Header DUNGEON LEVELS

	#region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

	#endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("Populate with the start dungeon level for testing, first level = 0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;

    private Room previousRoom;

    private PlayerDetailsSO playerDetails;

    private Player player;

    [HideInInspector] public GameState gameState;

    [HideInInspector] public GameState previousGameState;

    private long gameScore;

    private int scoreMultiplier;

    private InstantiatedRoom bossRoom;

	protected override void Awake()
	{
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetailsSO;

        InstantiatePlayer();
	}

    private void InstantiatePlayer()
	{
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
	}

	private void OnEnable()
	{
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        // Subscribe to the points scored event
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        // Subscribe to room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        // Subscribe to score multiplier event
        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        // Subscribe to player destroyed event
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
	}

	private void OnDisable()
	{
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        // Unsubscribe from the points scored event
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        // Unsubscribe from room enemies defeated event
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        // Unsubscribe from score multiplier event
        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;

        // Unsubscribe from player destroyed event
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
	{
        SetCurrentRoom(roomChangedEventArgs.room);
	}

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArs roomEnemiesDefeatedArs)
	{
        RoomEnemiesDefeated();
	}

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
	{
        // Increase score
        gameScore += pointsScoredArgs.points * scoreMultiplier;

        // Call score changed event
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
	}

    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
	{
        if (multiplierArgs.multiplier)
		{
            scoreMultiplier++;
		}
        else
		{
            scoreMultiplier--;
		}

        // Clamp between 1 and 30
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        // Call score changed event
        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
	}

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
	{
        previousGameState = gameState;

        gameState = GameState.gameLost;
	}

    // Start is called before the first frame update
    private void Start()
    {
        previousGameState = GameState.gameStarted;

        gameState = GameState.gameStarted;

        // Set score to zero
        gameScore = 0;

        // Set multiplier to one
        scoreMultiplier = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        // if (Input.GetKeyDown(KeyCode.P))
		// {
        //    gameState = GameState.gameStarted;
		// }
    }

    private void HandleGameState()
	{
        switch (gameState)
		{
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                gameState = GameState.playingLevel;
                RoomEnemiesDefeated();
                break;

            case GameState.levelCompleted:
                // Display level completed text
                StartCoroutine(LevelCompleted());
                break;

            case GameState.gameWon:
                if (previousGameState != GameState.gameWon)
				{
                    StartCoroutine(GameWon());
				}
                break;

            case GameState.gameLost:
                if (previousGameState != GameState.gameLost)
				{
                    StopAllCoroutines();

                    StartCoroutine(GameLost());
				}
                break;

            case GameState.restartGame:
                RestartGame();
                break;
        }
	}

    public void SetCurrentRoom(Room room)
	{
        previousRoom = currentRoom;

        currentRoom = room;
	}

    private void RoomEnemiesDefeated()
	{
        // Initialize dungeon as being cleared - but then test each room
        bool isDungeonClearOfRegularEnemies = true;

        bossRoom = null;

        // Loop through all dungeon rooms to see if cleared of enemies
        foreach(KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
		{
            // Skip boss room for time being
            if (keyValuePair.Value.roomNodeType.isBossRoom)
			{
                bossRoom = keyValuePair.Value.instanciatedRoom;

                continue;
			}

            // Check if the other rooms have been cleared of enemies
            if (!keyValuePair.Value.isClearedOfEnemies)
			{
                isDungeonClearOfRegularEnemies = false;

                break;
			}
		}

        // Set game state
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
		{
            // Are there more dungeon levels then
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
			{
                gameState = GameState.levelCompleted;
			}
            else
			{
                gameState = GameState.gameWon;
			}
		}

        // Else if dungeon level cleared apart from boss room
        else if (isDungeonClearOfRegularEnemies)
		{
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
		}
	}

    private void PlayDungeonLevel(int dungeonLevelListIndex)
	{
        bool dungeonBuildSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuildSuccessfully)
		{
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
		}

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    private IEnumerator BossStage()
	{
        // Activate boss room
        bossRoom.gameObject.SetActive(true);

        // Unlock boss room
        bossRoom.UnlockDoors(0f);

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        Debug.Log("Boss stage - find and destroy the boss!");
	}

    private IEnumerator LevelCompleted()
	{
        // Play next level
        gameState = GameState.playingLevel;

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);

        Debug.Log("Level completed - press return to progress to the next level");

        // When player presses the return key proceed to the next level
        while(!Input.GetKeyDown(KeyCode.Return))
		{
            yield return null;
		}

        yield return null;

        // Increase index to next level
        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
	{
        previousGameState = GameState.gameWon;

        Debug.Log("Game Won! All levels completed and bosses defeated. Game will restart in 10 seconds");

        // Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // Set game state to restart game
        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
	{
        previousGameState = GameState.gameLost;

        Debug.Log("Game Lost! Bad Luck. Game will restart in 10 seconds");

        // Wait 10 seconds
        yield return new WaitForSeconds(10f);

        // Set game state to restart game
        gameState = GameState.restartGame;
    }

    private void RestartGame()
	{
        SceneManager.LoadScene("MainGameScene");
	}

    public Player GetPlayer()
	{
        return player;
	}

    public Room GetCurrentRoom()
	{
        return currentRoom;
	}

    public Sprite GetPlayerMinimapIcon()
	{
        return playerDetails.playerMinimapIcon;
	}

    public DungeonLevelSO GetCurrentDungeonLevel()
	{
        return dungeonLevelList[currentDungeonLevelListIndex];
	}

	#region Validation

#if UNITY_EDITOR

	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
	}

#endif

	#endregion Validation
}
