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
        float3 gravitationalAcceleration = new float3(0f, -9.81f, 0f);

        foreach (var (transform, particle) in SystemAPI.Query<TransformAspect, RefRW<Particle>>())
        {
            // Add unary forces
            particle.ValueRW.ForceAccumulator += particle.ValueRO.Mass * gravitationalAcceleration;

            float3 acceleration = particle.ValueRO.ForceAccumulator / particle.ValueRO.Mass;
            float3 tempPosition = particle.ValueRO.Position;
            float3 previousPosition = particle.ValueRO.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
            particle.ValueRW.Position = 2 * tempPosition - previousPosition + acceleration * deltaTime * deltaTime;

            // Collisions and contact handling
            float particleRadius = particle.ValueRO.Radius;
            var minCorner = new float3(-10, 0, -10) + particleRadius;
            var maxCorner = new float3(10, 20, 10) - particleRadius;

            particle.ValueRW.Position = math.clamp(particle.ValueRO.Position, minCorner, maxCorner);

            particle.ValueRW.PreviousPosition = tempPosition;
            particle.ValueRW.ForceAccumulator = float3.zero; // Clear the force accumulator

            transform.Position = particle.ValueRO.Position;
        }
    }
}
