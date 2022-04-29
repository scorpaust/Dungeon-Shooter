using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;

	private EnemyMovementAI enemyMovementAI;

	[HideInInspector] public MovementToPositionEvent movementToPositionEvent;

	[HideInInspector] public IdleEvent idleEvent;

	private MaterializeEffect materializeEffect;

    private CircleCollider2D circleCollider2D;

    private PolygonCollider2D polygonCollider2D;

    [HideInInspector] public SpriteRenderer[] spriteRendererArray;

	[HideInInspector] public Animator animator;

	private void Awake()
	{
		// Load components

		enemyMovementAI = GetComponent<EnemyMovementAI>();

		movementToPositionEvent = GetComponent<MovementToPositionEvent>();

		idleEvent = GetComponent<IdleEvent>();

		materializeEffect = GetComponent<MaterializeEffect>();

		circleCollider2D = GetComponent<CircleCollider2D>();

		polygonCollider2D = GetComponent<PolygonCollider2D>();

		spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();

		animator = GetComponent<Animator>();
	}

	public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
	{
		this.enemyDetails = enemyDetails;

		SetEnemyMovementUpdateFrame(enemySpawnNumber);

		SetEnemyAnimationSpeed();

		// Materialize enemy
		StartCoroutine(MaterializeEnemy());
	}

	private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
	{
		// Set frame number that enemy should process its updates
		enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathFindingOver);
	}

	private void SetEnemyAnimationSpeed()
	{
		// Set animator speed to match movement speed
		animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
	}

	private IEnumerator MaterializeEnemy()
	{
		// Disable collider, movement AI and Weapon AI
		EnemyEnable(false);

		yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

		// Enable collider, movement AI and Weapon AI
		EnemyEnable(true);
	}

	private void EnemyEnable(bool isEnabled)
	{
		// Enable / Disable colliders
		circleCollider2D.enabled = isEnabled;
		polygonCollider2D.enabled = isEnabled;

		// Enable / Disable Movement AI
		enemyMovementAI.enabled = isEnabled;
	}
}
