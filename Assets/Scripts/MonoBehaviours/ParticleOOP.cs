using UnityEngine;

public class ParticleOOP : MonoBehaviour
{
    // Exposed in inspector
    [field: SerializeField]
    public Vector3 InitialForce { get; set; } = Vector3.zero;
    [field: SerializeField]
    public bool Static { get; set; } = false;

    // Properties
    public Vector3 Position { get; set; }
    public Vector3 PreviousPosition { get; set; }
    public Vector3 ForceAccumulator { get; set; }
    public float Mass { get; set; }
    public float Radius { get; set; }

    void Start()
    {
        Position = transform.position;
        PreviousPosition = transform.position;
        ForceAccumulator = InitialForce;
        Mass = 1;
        Radius = transform.localScale.x / 2f;
    }
}
