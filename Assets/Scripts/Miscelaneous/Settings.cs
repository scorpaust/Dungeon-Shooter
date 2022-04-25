using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{
	#region UNITS
	public const float pixelsPerUnit = 16f;
	public const float tileSizePixels = 16f;
	#endregion

	#region DUNGEON BUILD SETTINGS

	public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;

	public const int maxDungeonBuildAttempts = 10;

	#endregion DUNGEON BUILD SETTINGS

	#region ROOM SETTINGS

	public const float fadeInTime = 0.5f;

	public const int maxChildCorridors = 3;

	#endregion ROOM SETTINGS

	#region ANIMATOR PARAMETERS

	public static int aimUp = Animator.StringToHash("aimUp");

	public static int aimDown = Animator.StringToHash("aimDown");

	public static int aimUpRight = Animator.StringToHash("aimUpRight");

	public static int aimUpLeft = Animator.StringToHash("aimUpLeft");

	public static int aimRight = Animator.StringToHash("aimRight");

	public static int aimLeft = Animator.StringToHash("aimLeft");

	public static int isIdle = Animator.StringToHash("isIdle");

	public static int isMoving = Animator.StringToHash("isMoving");

	public static int rollUp = Animator.StringToHash("rollUp");

	public static int rollRight = Animator.StringToHash("rollRight");

	public static int rollLeft = Animator.StringToHash("rollLeft");

	public static int rollDown = Animator.StringToHash("rollDown");

	public static int open = Animator.StringToHash("open");

	public static float baseSpeedForPlayerAnimations = 8f;

	#endregion ANIMATOR PARAMETERS

	#region GAMEOBJECT TAGS

	public const string playertag = "Player";

	public const string playerWeapon = "playerWeapon";

	#endregion

	#region FIRING CONTROL

	public const float useAimAngleDistance = 3.5f; // if the target distance is less than this, then the aim angle will be used (calculated from player),
												   // else the weapon aim angle will be used (calculated from the weapon shoot position)

	#endregion FIRING CONTROL

	#region ASTAR PATHFINDING PARAMETERS
	public const int defaultAStarMovementPenalty = 40;
	public const int preferedPathAStarMovementPenalty = 1;
	public const float playerMoveDistanceToRebuildPath = 3f;
	public const float enemyPathRebuildCooldown = 2f;

	#endregion

	#region UI PARAMETERS
	public const float uiAmmoIconSpacing = 4f;
	#endregion UI PARAMETERS
}
