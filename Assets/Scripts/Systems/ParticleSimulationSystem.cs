using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ParticleSimulationSystem : ISystem
{
    float initialZ;

    public void OnCreate(ref SystemState state)
    {
        // Hinge constraint
        // =================
        // 1) Find the plane in which are all of the hinge particles
        //    - Z plane for the prototype
        // 2) Save the plane
        //    - Save the initial Z value of the particles
        // 3) Project all the hinge particles onto the plane before enforcing the distance constraints
        //    - Subtract the difference between the actual Z value and the initial one

        // 1) + 2) Find + Save the plane
        foreach (var (hinge, transform) in SystemAPI.Query<Hinge, TransformAspect>())
        {
            initialZ = transform.Position.z; // Save the Z value of the first hinge particle
            Debug.Log(initialZ);
            break; // Early out
        }
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
            particle.ValueRW.ForceAccumulator += particle.ValueRO.Mass * gravitationalAcceleration; // gravity

            float3 acceleration = particle.ValueRO.ForceAccumulator / particle.ValueRO.Mass;
            float3 tempPosition = particle.ValueRO.Position;
            float3 previousPosition = particle.ValueRO.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
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

        foreach (var (hinge, particle) in SystemAPI.Query<Hinge, RefRW<Particle>>())
        {
            // ======================================================== Hinge Constraint

            var correctedPosition = particle.ValueRO.Position;
            correctedPosition.z = initialZ;
            particle.ValueRW.Position = correctedPosition;
        }

        // Satisfy Constraints (through 1 iteration of relaxation)
        foreach (var constraint in SystemAPI.Query<Constraint>())
        {
            var particleA = SystemAPI.GetComponent<Particle>(constraint.ParticleA);
            var particleB = SystemAPI.GetComponent<Particle>(constraint.ParticleB);

            var aspectParticleA = SystemAPI.GetAspectRW<TransformAspect>(constraint.ParticleA);
            var aspectParticleB = SystemAPI.GetAspectRW<TransformAspect>(constraint.ParticleB);


            // ======================================================== Fixed distance constraint

            float3 deltaPosition = particleA.Position - particleB.Position;     // vector between the particles
            float deltaLength = math.length(deltaPosition);                     // distance between the particles
            float diff = (deltaLength - constraint.RestLength) / deltaLength;   // ratio of how much of the distance has to be corrected

            if (!particleA.Static)
            {
                particleA.Position -= deltaPosition * 0.5f * diff;          // apply half of the correction to particleA
                SystemAPI.SetComponent(constraint.ParticleA, particleA);
                aspectParticleA.Position = particleA.Position;
            }

            if (!particleB.Static)
            {
                particleB.Position += deltaPosition * 0.5f * diff;          // apply the other half of the correction to particleB
                SystemAPI.SetComponent(constraint.ParticleB, particleB);
                aspectParticleB.Position = particleB.Position;
            }

            Debug.DrawLine(particleA.Position, particleB.Position);         // visualize the constraint
        }
    }
}
