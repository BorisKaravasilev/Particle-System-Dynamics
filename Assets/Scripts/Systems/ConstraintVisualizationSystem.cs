using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ConstraintVisualizationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSettings>(); // Run only in scenes where the simulation system runs
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var constraint in SystemAPI.Query<Constraint>())
            {
                var particleA = SystemAPI.GetComponent<Particle>(constraint.ParticleA);
                var particleB = SystemAPI.GetComponent<Particle>(constraint.ParticleB);

                Debug.DrawLine(particleA.Position, particleB.Position);                 // visualize the constraint
            }
        }
    }
}
