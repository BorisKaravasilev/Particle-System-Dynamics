using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ParticleAuthoring : MonoBehaviour
{
    public float3 InitialForce = float3.zero;
    public float Mass = 1f;
    public bool Static = false;
}

public class ParticleBaker : Baker<ParticleAuthoring>
{
    public override void Bake(ParticleAuthoring authoring)
    {
        AddComponent(new Particle
        {
            Position = authoring.transform.position,
            PreviousPosition = authoring.transform.position,
            ForceAccumulator = authoring.InitialForce,
            Mass = authoring.Mass,
            Radius = authoring.transform.localScale.x / 2f,
            Static = authoring.Static
        });
    }
}