using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ParticleAuthoring : MonoBehaviour
{
    public float3 InitialForce = float3.zero;
    public bool Static = false;
    public bool IsHinge = false;
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
            Mass = 1f,
            Radius = authoring.transform.localScale.x / 2f,
            Static = authoring.Static
        });

        if (authoring.IsHinge)
        {
            AddComponent<Hinge>();
        }
    }
}