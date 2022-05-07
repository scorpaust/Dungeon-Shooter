using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiatedRoom))]
public class RoomLightingControl : MonoBehaviour
{
	private InstantiatedRoom instantiatedRoom;

	private void Awake()
	{
		instantiatedRoom = GetComponent<InstantiatedRoom>();
	}

	private void OnEnable()
	{
		StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
	}

	private void OnDisable()
	{
		StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
	}

	private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
	{
		if (roomChangedEventArgs.room == instantiatedRoom.room && !instantiatedRoom.room.isLit)
		{
			FadeInRoomLighting();

			// Ensure room environment decoration game objects are activated
			instantiatedRoom.ActivateEnvironmentGameObjects();

			// Fade in the environment decoration game objects lighting
			FadeInEnvironmentLighting();

			FadeInDoors();

			instantiatedRoom.room.isLit = true;
		}
	}

	private void FadeInRoomLighting()
	{
		StartCoroutine(FadeInRoomLightingRoutine(instantiatedRoom));
	}

	private IEnumerator FadeInRoomLightingRoutine(InstantiatedRoom instantiatedRoom)
	{
		Material material = new Material(GameResources.Instance.variableLitShader);

		instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;

		instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;

		instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;

		instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;

		instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

		for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
		{
			material.SetFloat("Alpha_Slider", i);

			yield return null;
		}

		instantiatedRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;

		instantiatedRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;

		instantiatedRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;

		instantiatedRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;

		instantiatedRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
	}

	private void FadeInEnvironmentLighting()
	{
		// Create new material to fade in
		Material material = new Material(GameResources.Instance.variableLitShader);

		// Get all environment components in room
		Environment[] environmentComponents = GetComponentsInChildren<Environment>();

		// Loop through
		foreach (Environment environmentComponent in environmentComponents)
		{
			if (environmentComponent.spriteRenderer != null)
				environmentComponent.spriteRenderer.material = material;
		}

		StartCoroutine(FadeInEnvironmentLightingRoutine(material, environmentComponents));
	}

	private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environmentComponents)
	{
		// Gradually fade in the lighting
		for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
		{
			material.SetFloat("Alpha_Slider", i);

			yield return null;
		}

		// Set environment components material back to lit material
		foreach (Environment environmentComponent in environmentComponents)
		{
			if (environmentComponent.spriteRenderer != null)
				environmentComponent.spriteRenderer.material = GameResources.Instance.litMaterial;
		}
	}

	private void FadeInDoors()
	{
		Door[] doorArray = GetComponentsInChildren<Door>();

		foreach (Door door in doorArray)
		{
			DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

			doorLightingControl.FadeInDoor(door);
		}
	}
}
