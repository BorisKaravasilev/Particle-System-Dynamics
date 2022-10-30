using Unity.Entities;
using Unity.Mathematics;

struct Particle : IComponentData
{
    public float3 Position;
    public float3 PreviousPosition;
    public float3 ForceAccumulator; // Stores the sum of all forces applied to the partcile this simulation step
    public float Mass;
    public float Radius;
}
