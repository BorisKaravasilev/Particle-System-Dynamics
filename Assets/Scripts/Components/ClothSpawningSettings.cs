using Unity.Entities;

struct ClothSpawningSettings : IComponentData
{
    public Entity ParticlePrefab;
    public Entity ConstraintPrefab;
    public int Dimension;
    public float Spacing;
}
