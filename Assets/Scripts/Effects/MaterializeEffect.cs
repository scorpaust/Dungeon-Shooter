using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
	{
		Material materializeMaterial = new Material(materializeShader);

		materializeMaterial.SetColor("_EmissionColor", materializeColor);

		// Set materialize material in sprite renderers
		foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
		{
			spriteRenderer.material = materializeMaterial;
		}

		float disolveAmount = 0f;

		// Materialize enemy
		while (disolveAmount < 1f)
		{
			disolveAmount += Time.deltaTime / materializeTime;

			materializeMaterial.SetFloat("_DissolveAmount", disolveAmount);

			yield return null;
		}

		// Set standard material to sprite renderers
		foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
		{
			spriteRenderer.material = normalMaterial;
		}
	}
}
