using Unity.Mathematics;
using UnityEngine;

public class ParticleSimulationOOP : MonoBehaviour
{
    [field: SerializeField]
    public int RelaxationIterations { get; set; } = 1;
    [field: SerializeField]
    public Vector3 MinCorner { get; set; } = new Vector3(-100, -100, -100);
    [field: SerializeField]
    public Vector3 MaxCorner { get; set; } = new Vector3(100, 100, 100);

    Vector3 gravitationalAcceleration = new Vector3(0f, -9.81f, 0f);
    ParticleOOP[] particles;
    ConstraintOOP[] constraints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (particles == null || constraints == null)
        {
            particles = FindObjectsOfType<ParticleOOP>();
            constraints = FindObjectsOfType<ConstraintOOP>();
        }

        foreach (var particle in particles)
        {
            if (particle.Static) continue; // Early out if particle is static

            // Add unary forces
            particle.ForceAccumulator += particle.Mass * gravitationalAcceleration;  // Add gravitational force

            Vector3 acceleration = particle.ForceAccumulator / particle.Mass;
            Vector3 tempPosition = particle.Position;
            Vector3 previousPosition = particle.PreviousPosition;

            // Verlet integration
            // x' = 2x - x* + a * dt^2
            //
            // x' - new position
            // x  - current position
            // x* - previous position
            
            particle.Position = 2 * tempPosition - previousPosition + acceleration * Time.deltaTime * Time.deltaTime;

            // Collisions and contact handling (keep particles within a box)
            float particleRadius = particle.Radius;
            var minCorner = MinCorner + new Vector3(particleRadius, particleRadius, particleRadius);
            var maxCorner = MaxCorner - new Vector3(particleRadius, particleRadius, particleRadius);

            particle.Position = math.clamp(particle.Position, minCorner, maxCorner);

            particle.PreviousPosition = tempPosition;
            particle.ForceAccumulator = float3.zero;          // Clear the force accumulator

            particle.transform.position = particle.Position;  // Update the position
        }

        // Satisfy Constraints
        foreach (var constraint in constraints)
        {
            ParticleOOP particleA = constraint.ParticleA;
            ParticleOOP particleB = constraint.ParticleB;

            // ======================================================== Fixed distance constraint

            for (int i = 0; i < RelaxationIterations; i++)
            {
                Vector3 deltaPosition = particleA.Position - particleB.Position;    // vector between the particles
                float deltaLength = deltaPosition.magnitude;                        // distance between the particles
                float diff = (deltaLength - constraint.RestLength) / deltaLength;   // ratio of how much of the distance has to be corrected

                if (!particleA.Static)
                {
                    particleA.Position -= deltaPosition * 0.5f * diff;              // apply half of the correction to particleA
                    particleA.transform.position = particleA.Position;
                }

                if (!particleB.Static)
                {
                    particleB.Position += deltaPosition * 0.5f * diff;              // apply the other half of the correction to particleB
                    particleB.transform.position = particleB.Position;
                }
            }

            Debug.DrawLine(particleA.Position, particleB.Position);                 // visualize the constraint
        }
    }
}
