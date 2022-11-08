using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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

        // Constraint (stick) instantiation
        int constraintCount = particleCount - 1;//(settings.Dimension - 1) * (settings.Dimension - 1);
        var constraints = CollectionHelper.CreateNativeArray<Entity>(constraintCount, Allocator.Temp);
        ecb.Instantiate(settings.ConstraintPrefab, constraints);

        // Initialize particle data
        for (int i = 0; i < particleCount; i++)
        {
            float x = settings.Spacing * i;
            float3 position = new float3(x, 0, 0);

            ecb.SetComponent(particles[i], new Translation
            {
                Value = new float3(x, 0, 0)
            });

            ecb.SetComponent(particles[i], new Particle
            {
                Position = position,
                PreviousPosition = position,
                ForceAccumulator = float3.zero,
                Mass = 1f,
                Radius = 1f,
                Static = i == 0 ? true : false
            });
        }

        for (int i = 0; i < constraintCount; i++)
        {
            ecb.SetComponent(constraints[i], new Constraint
            {
                ParticleA = particles[i],
                ParticleB = particles[i + 1],
                RestLength = settings.Spacing,
            });
        }

            state.Enabled = false; // Run only once
    }
}
