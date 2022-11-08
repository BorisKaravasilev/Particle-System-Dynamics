using Unity.Entities;
using UnityEngine;

public class SimulationAuthoring : MonoBehaviour
{
    public int RelaxationIterations = 1;
}

public class SimulationBaker : Baker<SimulationAuthoring>
{
    public override void Bake(SimulationAuthoring authoring)
    {
        AddComponent(new Simulation
        {
            RelaxationIterations = authoring.RelaxationIterations
        });
    }
}