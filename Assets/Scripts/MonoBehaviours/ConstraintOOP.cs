using UnityEngine;

public class ConstraintOOP : MonoBehaviour
{
    [field: SerializeField]
    public ParticleOOP ParticleA { get; set; }
    [field: SerializeField]
    public ParticleOOP ParticleB { get; set; }

    public float RestLength { get; set; }

    private void Start()
    {
        var diff = ParticleA.transform.position - ParticleB.transform.position;
        RestLength = diff.magnitude;
    }
}
