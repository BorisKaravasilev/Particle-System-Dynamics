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
        float restLength = 5f;

        if (authoring.ParticleA != null && authoring.ParticleB != null)
        {
            restLength = (authoring.ParticleA.transform.position - authoring.ParticleB.transform.position).magnitude;
        }

        AddComponent(new Constraint
        {
            ParticleA = authoring.ParticleA == null ? Entity.Null : GetEntity(authoring.ParticleA),
            ParticleB = authoring.ParticleB == null ? Entity.Null : GetEntity(authoring.ParticleB),
            RestLength = restLength
        });
    }
}