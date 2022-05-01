using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int enemiesToSpawn;

    private int currentEnemyCount;

    private int enemiesSpawnedSoFar;

    private int enemyMaxConcurrentSpawnNumber;

    private Room currentRoom;

    private RoomEnemySpawnParameters roomEnemySpawnParameters;

	private void OnEnable()
	{
        // Subscribe to room changed event
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
	}

	private void OnDisable()
	{
        // Unsubscribe from room changed event
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
	{
        enemiesSpawnedSoFar = 0;

        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        // If the room is a corridor or the entrance then return
        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            return;

        // If the room has already been defeated then return
        if (currentRoom.isClearedOfEnemies) return;

        // Get random number of enemies to spawn
        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        // Get room enemy spawn parameters
        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        // If no enemies to spawn return
        if (enemiesToSpawn == 0)
		{
            // Mark the room as cleared
            currentRoom.isClearedOfEnemies = true;

            return;
		}

        // Get concurrent number of enemies to spawn
        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        // Lock doors
        currentRoom.instanciatedRoom.LockDoors();

        // Spawn Enemies
        SpawnEnemies();
	}

    private void SpawnEnemies()
	{
        // Set game state engaging enemies
        if (GameManager.Instance.gameState == GameState.playingLevel)
		{
            GameManager.Instance.previousGameState = GameState.playingLevel;

            GameManager.Instance.gameState = GameState.engagingEnemies; 
		}

        StartCoroutine(SpawnEnemiesRoutine());
	}

    private IEnumerator SpawnEnemiesRoutine()
	{
        Grid grid = currentRoom.instanciatedRoom.grid;

        // Create an instance of the helper class used to select a random enemy
        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

        // Check we have somewhere to spawn the enemies
        if (currentRoom.spawnPositionArray.Length > 0)
		{
            // Loop through to create all enemies
            for (int i = 0; i < enemiesToSpawn; i++)
			{
                // Wait until current enemy count is less then max concurrent enemies
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
				{
                    yield return null;
				}

                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

                // Create enemy - get next enemy type to spawn
                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
			}
		}
	}

    private float GetEnemySpawnInterval()
	{
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
	}

    private int GetConcurrentEnemies()
	{
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
	}

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
	{
        // Keep track of the number of enemies spawned so far
        enemiesSpawnedSoFar++;

        // Add one to the current enemy count - this is reduced when an enemy is destroyed
        currentEnemyCount++;

        // Get current dungeon level
        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        // Instanciate enemy
        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

        // Initialize enemy
        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

        // Subscribe to enemy destroyed event
        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
	}

    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
	{
        // Unsubscribe from event
        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

        // Reduce current enemy count
        currentEnemyCount--;

        // Score points - call points scored event
        StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
		{
            currentRoom.isClearedOfEnemies = true;

            // Set game state
            if (GameManager.Instance.gameState == GameState.engagingEnemies)
			{
                GameManager.Instance.gameState = GameState.playingLevel;

                GameManager.Instance.previousGameState = GameState.engagingEnemies;
			}

            else if (GameManager.Instance.gameState == GameState.engagingBoss)
			{
                GameManager.Instance.gameState = GameState.bossStage;

                GameManager.Instance.previousGameState = GameState.engagingBoss;
			}

            // Unlock doors
            currentRoom.instanciatedRoom.UnlockDoors(Settings.doorUnlockDelay);

            // Trigger room enemies defeated event
            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
		}
	}
}
