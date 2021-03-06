using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
	#region Tooltip
	[Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
	#endregion Tooltip
	[SerializeField] private MovementDetailsSO movementDetails;

	private Enemy enemy;

	private Stack<Vector3> movementSteps = new Stack<Vector3>();

	private Vector3 playerReferencePosition;

	private Coroutine moveEnemyRoutine;

	private float currentEnemyPathRebuildCooldown;

	private WaitForFixedUpdate waitForFixedUpdate;

	[HideInInspector] public float moveSpeed;

	private bool chasePlayer = false;

	[HideInInspector] public int updateFrameNumber = 1; // default value; this is set by the enemy spawner

	private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

	private void Awake()
	{
		enemy = GetComponent<Enemy>();

		moveSpeed = movementDetails.GetMoveSpeed();
	}

	private void Start()
	{
		// Create waitForFixedUpdate for use in coroutine
		waitForFixedUpdate = new WaitForFixedUpdate();

		// Reset player reference position
		playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
	}

	private void Update()
	{
		MoveEnemy();
	}

	private void MoveEnemy()
	{
		// Movement cooldown timer
		currentEnemyPathRebuildCooldown -= Time.deltaTime;

		// Check distance to player to see if enemy should start chasing
		if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
		{
			chasePlayer = true;
		}

		// If not close enough to chase player then return
		if (!chasePlayer)
			return;

		// Only process an AStar Path rebuild on certain frames to spread the load between enemies
		if (Time.frameCount % Settings.targetFrameRateToSpreadPathFindingOver != updateFrameNumber) return;

		// If the movement cooldown timer reached or player has moved more than required distance
		// then rebuild the enemy path and move the enemy
		if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition())) > Settings.playerMoveDistanceToRebuildPath)
		{
			// Reset path rebuild cooldown timer
			currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

			// Reset player reference position
			playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

			// Move the enemy using AStar pathfinding - trigger rebuild of path to player
			CreatePath();

			// If a path has been found, move the enemy
			if (movementSteps != null)
			{
				if (moveEnemyRoutine != null)
				{
					// Trigger idle event
					enemy.idleEvent.CallIdleEvent();

					StopCoroutine(moveEnemyRoutine);
				}
			}

			// Move enemy along the path using a coroutine
			moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
		}
	}

	private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
	{
		while (movementSteps.Count > 0)
		{
			Vector3 nextPosition = movementSteps.Pop();

			// While not very close continue to move - when close move onto the next step
			while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
			{
				// Trigger movement event
				enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);

				yield return waitForFixedUpdate;
			}

			yield return waitForFixedUpdate;
		}
	}

	private void CreatePath()
	{
		Room currentRoom = GameManager.Instance.GetCurrentRoom();

		Grid grid = currentRoom.instanciatedRoom.grid;

		// Get player's position on the grid
		Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

		// Get enemy position on the grid
		Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

		// Build a path for the enemy to move on
		movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

		// Take off first step on path - this is the grid square the enemy is already on
		if (movementSteps != null)
		{
			movementSteps.Pop();
		}
		else
		{
			// Trigger the idle event - no path
			enemy.idleEvent.CallIdleEvent();
		}
	}

	public void SetUpdateFrameNumber(int updateFrameNumber)
	{
		this.updateFrameNumber = updateFrameNumber;
	}

	private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
	{
		Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

		Vector3Int playerCellPosition = currentRoom.instanciatedRoom.grid.WorldToCell(playerPosition);

		Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

		int obstacle = Mathf.Min(currentRoom.instanciatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y], 
			currentRoom.instanciatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

		// If the player isn't on a cell square marked as an obstacle then return that position
		if (obstacle != 0)
		{
			return playerCellPosition;
		}
		else
		{

			// Empty surrounding position list
			surroundingPositionList.Clear();

			// Populate surrounding position list - this will hold the 8 possible vector locations surrounding a (0,0) grid square
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (j == 0 && i == 0) continue;

					surroundingPositionList.Add(new Vector2Int(i, j));
				}
			}

			// Loop through all positions
			for (int k = 0; k < 8; k++)
			{
				// Generate a random index for the list
				int index = Random.Range(0, surroundingPositionList.Count);

				// See if there is an obstacle in the selected surrounding position
				try
				{
					obstacle = Mathf.Min(currentRoom.instanciatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y],
						currentRoom.instanciatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x + surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);

					// If there is no obstacle return the cell position to navigate to
					if (obstacle != 0)
					{
						return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
					}
				}
				// Catch errors where the surrounding position is outside the grid
				catch
				{

				}

				// Remove the surrounding position with the obstacle so we can try again
				surroundingPositionList.RemoveAt(index);
			}

			// if no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
			return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
		}
	}
}
