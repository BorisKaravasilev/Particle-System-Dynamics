using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ParticleSimulationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (transform, particle) in SystemAPI.Query<TransformAspect, RefRW<Particle>>())
        {
            float3 acceleration = particle.ValueRO.ForceAccumulator / particle.ValueRO.Mass;
            float3 position = particle.ValueRO.Position;
            float3 previousPosition = particle.ValueRO.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
            particle.ValueRW.Position = 2 * position - previousPosition + acceleration * deltaTime * deltaTime;
            particle.ValueRW.PreviousPosition = position;
            particle.ValueRW.ForceAccumulator = float3.zero; // Clear the force accumulator

            transform.Position = particle.ValueRO.Position;
        }
    }
}
