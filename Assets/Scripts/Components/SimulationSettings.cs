using Unity.Entities;
using Unity.Mathematics;

struct SimulationSettings : IComponentData
{
    public int RelaxationIterations;
    public float3 MinCorner;
    public float3 MaxCorner;
}
