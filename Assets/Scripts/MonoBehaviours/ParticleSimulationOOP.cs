using Unity.Mathematics;
using UnityEngine;

public class ParticleSimulationOOP : MonoBehaviour
{
    Vector3 gravitationalAcceleration = new Vector3(0f, -9.81f, 0f);
    ParticleOOP[] particles;
    ConstraintOOP[] constraints;

    // Start is called before the first frame update
    void Start()
    {
        particles = FindObjectsOfType<ParticleOOP>();
        constraints = FindObjectsOfType<ConstraintOOP>();
    }

    private void FixedUpdate()
    {
        foreach (var particle in particles)
        {
            if (particle.Static) continue;

            // Add unary forces
            particle.ForceAccumulator += particle.Mass * gravitationalAcceleration;

            Vector3 acceleration = particle.ForceAccumulator / particle.Mass;
            Vector3 tempPosition = particle.Position;
            Vector3 previousPosition = particle.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
            particle.Position = 2 * tempPosition - previousPosition + acceleration * Time.deltaTime * Time.deltaTime;

            // Collisions and contact handling
            float particleRadius = particle.Radius;
            var minCorner = new Vector3(-10, 0, -10) + new Vector3(particleRadius, particleRadius, particleRadius);
            var maxCorner = new Vector3(10, 20, 10) - new Vector3(particleRadius, particleRadius, particleRadius);

            particle.Position = math.clamp(particle.Position, minCorner, maxCorner);

            particle.PreviousPosition = tempPosition;
            particle.ForceAccumulator = float3.zero; // Clear the force accumulator

            particle.transform.position = particle.Position;
        }

        // Satisfy Constraints
        foreach (var constraint in constraints)
        {
            var particleA = constraint.ParticleA;
            var particleB = constraint.ParticleB;

            Vector3 deltaPosition = particleA.Position - particleB.Position;
            float deltaLength = math.length(deltaPosition);
            float diff = (deltaLength - constraint.RestLength) / deltaLength;

            if (!particleA.Static)
            {
                particleA.Position -= deltaPosition * 0.5f * diff;
                particleA.transform.position = particleA.Position;
            }

            if (!particleB.Static)
            {
                particleB.Position += deltaPosition * 0.5f * diff;
                particleB.transform.position = particleB.Position;
            }

            Debug.DrawLine(particleA.Position, particleB.Position);
        }
    }
}
