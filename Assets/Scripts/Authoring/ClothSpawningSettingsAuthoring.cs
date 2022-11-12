using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ClothSpawningSettingsAuthoring : MonoBehaviour
{
    public GameObject ParticlePrefab;
    public GameObject ConstraintPrefab;
    public int Dimension;
    public float Spacing;
    public float3 InitialImpulse;
}

public class ClothSpawningSettingsBaker : Baker<ClothSpawningSettingsAuthoring>
{
    public override void Bake(ClothSpawningSettingsAuthoring authoring)
    {
        AddComponent(new ClothSpawningSettings
        {
            ParticlePrefab = GetEntity(authoring.ParticlePrefab),
            ConstraintPrefab = GetEntity(authoring.ConstraintPrefab),
            Dimension = authoring.Dimension,
            Spacing = authoring.Spacing,
            InitialImpulse = authoring.InitialImpulse,
        });
    }
}