using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO hitEffect)
    {
        SetHitEffectColorGradient(hitEffect.colorGradient);

        SetHitEffectParticleStartingValues(hitEffect.duration, hitEffect.startParticleSize, hitEffect.startParticleSpeed,
            hitEffect.startLifetime, hitEffect.effectGravity, hitEffect.maxParticleNumber);

        SetHitEffectParticleEmission(hitEffect.emissionRate, hitEffect.burstParticleNumber);

        SetHitEffectParticleSprite(hitEffect.sprite);

        SetHitEffectVelocityOverLifetime(hitEffect.velocityOverLifetimeMin, hitEffect.velocityOverLifetimeMax);
    }

    private void SetHitEffectColorGradient(Gradient colorGradient)
    {
        var colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticleNumber)
    {
        var mainModule = ammoHitEffectParticleSystem.main;

        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetHitEffectParticleEmission(int emissionRate, int burstParticleNumber)
    {
        var emissionModule = ammoHitEffectParticleSystem.emission;

        var burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        var textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetHitEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        var velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        var minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifetimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        var minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifetimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        var minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifetimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifetimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }

}
