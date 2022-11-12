using Unity.Entities;
using Unity.Mathematics;

struct ClothSpawningSettings : IComponentData
{
    public Entity ParticlePrefab;
    public Entity ConstraintPrefab;
    public int Dimension;
    public float Spacing;
    public float3 InitialImpulse;
}
