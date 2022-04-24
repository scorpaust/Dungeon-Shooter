using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

	private void Awake()
	{
		ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
	}

	public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
	{
		// Set hit effect color gradient
		SetHitEffectColorGradient(ammoHitEffect.colorGradient);

		// Set hit effect particle system starting values
		SetHitEffectParticleStartingValues(ammoHitEffect.duration, ammoHitEffect.startParticleSize, ammoHitEffect.startParticleSpeed,
			ammoHitEffect.startLifeTime, ammoHitEffect.effectGravity, ammoHitEffect.maxParticleNumber);

		// Set hit effect particle system particle burst particle number
		SetHitEffectParticleEmission(ammoHitEffect.emissionRate, ammoHitEffect.burstParticleNumber);

		// Set hit effect particle sprite
		SetHitEffectParticleSprite(ammoHitEffect.sprite);

		// Set hit effect lifetime min and max velocities
		SetHitEffectVelocityOverLifeTime(ammoHitEffect.velocityOverLifetimeMin, ammoHitEffect.velocityOverLifetimeMax);
	}

	private void SetHitEffectColorGradient(Gradient gradient)
	{
		// Set color gradient
		ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;

		colorOverLifetimeModule.color = gradient;
	}

	private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime,
		float effectGravity, int maxParticles)
	{
		ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

		mainModule.duration = duration;

		mainModule.startSize = startParticleSize;

		mainModule.startSpeed = startParticleSpeed;

		mainModule.startLifetime = startLifetime;

		mainModule.gravityModifier = effectGravity;

		mainModule.maxParticles = maxParticles;
	}

	private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
	{
		ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

		// Set particle burst number
		ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);

		emissionModule.SetBurst(0, burst);

		// Set particle emission rate
		emissionModule.rateOverTime = emissionRate;
	}

	private void SetHitEffectParticleSprite(Sprite sprite)
	{
		// Set particle burst number
		ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

		textureSheetAnimationModule.SetSprite(0, sprite);
	}

	private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
	{
		ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

		// Define min max X velocity
		ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();

		minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;

		minMaxCurveX.constantMin = minVelocity.x;

		minMaxCurveX.constantMax = maxVelocity.x;

		velocityOverLifetimeModule.x = minMaxCurveX;

		// Define min max Y velocity
		ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();

		minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;

		minMaxCurveY.constantMin = minVelocity.y;

		minMaxCurveY.constantMax = maxVelocity.y;

		velocityOverLifetimeModule.y = minMaxCurveY;

		// Define min max Z velocity
		ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();

		minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;

		minMaxCurveZ.constantMin = minVelocity.z;

		minMaxCurveZ.constantMax = maxVelocity.z;

		velocityOverLifetimeModule.z = minMaxCurveZ;
	}
}
