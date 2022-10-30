using Unity.Entities;
using Unity.Mathematics;

struct Constraint : IComponentData
{
    public Entity ParticleA;
    public Entity ParticleB;
    public float RestLength;
}
