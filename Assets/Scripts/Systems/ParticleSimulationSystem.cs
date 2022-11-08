using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ParticleSimulationSystem : ISystem
{
    int relaxation_iterations;

    public void OnCreate(ref SystemState state)
    {
        relaxation_iterations = 1;
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
            if (particle.ValueRO.Static)
            {
                continue; // Early out if particle is static
            }

            // Add unary forces
            particle.ValueRW.ForceAccumulator += particle.ValueRO.Mass * gravitationalAcceleration; // Add gravitational force

            float3 acceleration = particle.ValueRO.ForceAccumulator / particle.ValueRO.Mass;
            float3 tempPosition = particle.ValueRO.Position;
            float3 previousPosition = particle.ValueRO.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
            //
            // x' - new position
            // x  - current position
            // x* - previous position

            particle.ValueRW.Position = 2 * tempPosition - previousPosition + acceleration * deltaTime * deltaTime;

            // Collisions and contact handling (keep particles within a box)
            float particleRadius = particle.ValueRO.Radius;
            var minCorner = new float3(-10, 0, -10) + particleRadius;
            var maxCorner = new float3(10, 20, 10) - particleRadius;

            particle.ValueRW.Position = math.clamp(particle.ValueRO.Position, minCorner, maxCorner);

            particle.ValueRW.PreviousPosition = tempPosition;
            particle.ValueRW.ForceAccumulator = float3.zero; // Clear the force accumulator

            transform.Position = particle.ValueRO.Position;  // Update the position
        }

        // Satisfy Constraints (through relaxation)
        foreach (var constraint in SystemAPI.Query<Constraint>())
        {
            var particleA = SystemAPI.GetComponent<Particle>(constraint.ParticleA);
            var particleB = SystemAPI.GetComponent<Particle>(constraint.ParticleB);

            var aspectParticleA = SystemAPI.GetAspectRW<TransformAspect>(constraint.ParticleA);
            var aspectParticleB = SystemAPI.GetAspectRW<TransformAspect>(constraint.ParticleB);


            // ======================================================== Fixed distance constraint
            

            for (int i = 0; i < relaxation_iterations; i++)
            {
                float3 deltaPosition = particleA.Position - particleB.Position;     // vector between the particles
                float deltaLength = math.length(deltaPosition);                     // distance between the particles
                float diff = (deltaLength - constraint.RestLength) / deltaLength;   // ratio of how much of the distance has to be corrected

                if (!particleA.Static)
                {
                    particleA.Position -= deltaPosition * 0.5f * diff;              // apply half of the correction to particleA
                    SystemAPI.SetComponent(constraint.ParticleA, particleA);
                    aspectParticleA.Position = particleA.Position;
                }

                if (!particleB.Static)
                {
                    particleB.Position += deltaPosition * 0.5f * diff;              // apply the other half of the correction to particleB
                    SystemAPI.SetComponent(constraint.ParticleB, particleB);
                    aspectParticleB.Position = particleB.Position;
                }
            }

            Debug.DrawLine(particleA.Position, particleB.Position);                 // visualize the constraint
        }
    }
}
