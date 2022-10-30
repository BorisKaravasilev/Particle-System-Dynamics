using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConstraintAuthoring : MonoBehaviour
{
    public GameObject ParticleA;
    public GameObject ParticleB;
}

public class ConstraintBaker : Baker<ConstraintAuthoring>
{
    public override void Bake(ConstraintAuthoring authoring)
    {
        var diff = authoring.ParticleA.transform.position - authoring.ParticleB.transform.position;

        AddComponent(new Constraint
        {
            ParticleA = GetEntity(authoring.ParticleA),
            ParticleB = GetEntity(authoring.ParticleB),
            RestLength = diff.magnitude
        });
    }
}