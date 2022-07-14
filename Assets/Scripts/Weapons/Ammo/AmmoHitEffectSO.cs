using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    [Space(10)]
    [Header("AMMO HIT EFFECT DETAILS")]
    [Tooltip(@"The color gradient for the hit effect. This gradient show the 
        color of particles during their lifetime - from left to right")]
    public Gradient colorGradient;

    [Tooltip("The length of time the particle system is emitting particles")]
    public float duration = 0.5f;

    [Tooltip("The start particle size of the particle effect")]
    public float startParticleSize = 0.25f;

    [Tooltip("The start particle speed for the particle effect")]
    public float startParticleSpeed = 3f;

    [Tooltip("The particle lifetime for the particle effect")]
    public float startLifetime = 0.5f;

    [Tooltip("The maximum number of particles to be emitted")]
    public int maxParticleNumber = 100;

    [Tooltip("The number of particles emitted per second. If zero it will just be the burst number")]
    public int emissionRate = 100;

    [Tooltip("How manu particles should be emitted in the particle effect burst")]
    public int burstParticleNumber = 20;

    [Tooltip("The gravity on the particles - a small negative number will make them float up")]
    public float effectGravity = -0.01f;

    [Tooltip("The sprite for the particle effect. If none is specified then the default sprite will be used")]
    public Sprite sprite;

    [Tooltip(@"The min velocity for the particle over its lifetime. 
        A random value between min amd max will be generated")]
    public Vector3 velocityOverLifetimeMin;

    [Tooltip(@"The max velocity for the particle over its lifetime. 
        A random value between min amd max will be generated")]
    public Vector3 velocityOverLifetimeMax;

    [Tooltip(@"ammoHitEffectPrefab contains the particle system for the shoot effect 
        - and is configured by the ammoHitEffect SO")]
    public GameObject ammoHitEffectPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHitEffectPrefab), ammoHitEffectPrefab);
    }
#endif
}
