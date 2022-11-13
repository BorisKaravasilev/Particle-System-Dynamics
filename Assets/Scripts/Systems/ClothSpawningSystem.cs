using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
partial struct ClothSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ClothSpawningSettings>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        // Get spawner settings
        var settings = SystemAPI.GetSingleton<ClothSpawningSettings>();

        // Create an ECB
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Particle instantiation
        int particleCount = settings.Dimension * settings.Dimension;
        var particles = CollectionHelper.CreateNativeArray<Entity>(particleCount, Allocator.Temp);
        ecb.Instantiate(settings.ParticlePrefab, particles);

        //  Horizontal constraint instantiation
        int horizontalConstraintCount = (settings.Dimension - 1) * settings.Dimension;
        var horizontalConstraints = CollectionHelper.CreateNativeArray<Entity>(horizontalConstraintCount, Allocator.Temp);
        ecb.Instantiate(settings.ConstraintPrefab, horizontalConstraints);

        // Vertical constraint instantiation
        int verticalConstraintCount = (settings.Dimension - 1) * settings.Dimension;
        var verticalConstraints = CollectionHelper.CreateNativeArray<Entity>(verticalConstraintCount, Allocator.Temp);
        ecb.Instantiate(settings.ConstraintPrefab, verticalConstraints);


        // ================================================ Initialize particle data

        for (int i = 0; i < particleCount; i++)
        {
            float x = settings.Spacing * (i % settings.Dimension);
            float y = settings.Spacing * (i / settings.Dimension);
            float3 position = new float3(x, -y, 0);

            ecb.SetComponent(particles[i], new Translation
            {
                Value = position
            });

            ecb.SetComponent(particles[i], new Particle
            {
                Position = position,
                PreviousPosition = position,
                ForceAccumulator = x * settings.InitialImpulse,
                Mass = 1f,
                Radius = 1f,
                Static = y == 0 ? true : false
            });
        }


        // ================================================ Initialize constraint data

        int horizontalConstraintIndex = 0;
        int verticalConstraintIndex = 0;

        // Loop through the particles row by row
        for (int i = 0; i < particleCount; i++)
        {
            int x = i % settings.Dimension;
            int y = i / settings.Dimension;

            // Connect the particle to the right neighbour (if not the last column)
            if (x != settings.Dimension - 1)
            {
                ecb.SetComponent(horizontalConstraints[horizontalConstraintIndex], new Constraint
                {
                    ParticleA = particles[i],
                    ParticleB = particles[i + 1],
                    RestLength = settings.Spacing,
                });

                horizontalConstraintIndex++;
            }

            // Connect the particle to the bottom neighbour (if not the last row)
            if (y != settings.Dimension - 1)
            {
                ecb.SetComponent(verticalConstraints[verticalConstraintIndex], new Constraint
                {
                    ParticleA = particles[i],
                    ParticleB = particles[i + settings.Dimension],
                    RestLength = settings.Spacing,
                });

                verticalConstraintIndex++;
            }
        }

        state.Enabled = false; // Run only once
    }
}
