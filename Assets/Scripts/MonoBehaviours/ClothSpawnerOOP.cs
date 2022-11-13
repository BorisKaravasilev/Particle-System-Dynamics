using UnityEngine;

public class ClothSpawnerOOP : MonoBehaviour
{
    public ParticleOOP ParticlePrefab;
    public ConstraintOOP ConstraintPrefab;
    public int Dimension;
    public float Spacing;
    public Vector3 InitialImpulse;

    // Start is called before the first frame update
    void Start()
    {
        // Particle instantiation
        int particleCount = Dimension * Dimension;
        GameObject[] particles = new GameObject[particleCount];

        //  Horizontal constraint instantiation
        int horizontalConstraintCount = (Dimension - 1) * Dimension;
        GameObject[] horizontalConstraints = new GameObject[horizontalConstraintCount];


        // Vertical constraint instantiation
        int verticalConstraintCount = (Dimension - 1) * Dimension;
        GameObject[] verticalConstraints = new GameObject[verticalConstraintCount];


        // ================================================ Instantiate particles

        for (int i = 0; i < particleCount; i++)
        {
            float x = Spacing * (i % Dimension);
            float y = Spacing * (i / Dimension);
            Vector3 position = new Vector3(x, -y, 0);

            particles[i] = Instantiate(ParticlePrefab.gameObject, position, Quaternion.identity);
            var particle = particles[i].GetComponent<ParticleOOP>();
            particle.Position = position;
            particle.PreviousPosition = position;
            particle.ForceAccumulator = x * InitialImpulse;
            particle.Mass = 1f;
            particle.Radius = 1f;
            particle.Static = y == 0 ? true : false;
        }


        // ================================================ Initialize constraint data

        int horizontalConstraintIndex = 0;
        int verticalConstraintIndex = 0;

        // Loop through the particles row by row
        for (int i = 0; i < particleCount; i++)
        {
            int x = i % Dimension;
            int y = i / Dimension;

            // Connect the particle to the right neighbour (if not the last column)
            if (x != Dimension - 1)
            {
                horizontalConstraints[horizontalConstraintIndex] = Instantiate(ConstraintPrefab.gameObject);
                var horizontalConstraint = horizontalConstraints[horizontalConstraintIndex].GetComponent<ConstraintOOP>();
                horizontalConstraint.ParticleA = particles[i].GetComponent<ParticleOOP>();
                horizontalConstraint.ParticleB = particles[i + 1].GetComponent<ParticleOOP>();
                horizontalConstraint.RestLength = Spacing;

                horizontalConstraintIndex++;
            }

            // Connect the particle to the bottom neighbour (if not the last row)
            if (y != Dimension - 1)
            {
                verticalConstraints[verticalConstraintIndex] = Instantiate(ConstraintPrefab.gameObject);
                var verticalConstraint = verticalConstraints[verticalConstraintIndex].GetComponent<ConstraintOOP>();
                verticalConstraint.ParticleA = particles[i].GetComponent<ParticleOOP>();
                verticalConstraint.ParticleB = particles[i + Dimension].GetComponent<ParticleOOP>();
                verticalConstraint.RestLength = Spacing;

                verticalConstraintIndex++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
