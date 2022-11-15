using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SimulationSettingsAuthoring : MonoBehaviour
{
    public int RelaxationIterations = 1;
    public float3 MinCorner = new float3(-10, 0, -10);
    public float3 MaxCorner = new float3(10, 20, 10);
    public bool EnableWalls = true;
}

public class SimulationSettingsBaker : Baker<SimulationSettingsAuthoring>
{
    public override void Bake(SimulationSettingsAuthoring authoring)
    {
        AddComponent(new SimulationSettings
        {
            RelaxationIterations = authoring.RelaxationIterations,
            MinCorner = authoring.MinCorner,
            MaxCorner = authoring.MaxCorner,
            EnableWalls = authoring.EnableWalls
        });
    }
}