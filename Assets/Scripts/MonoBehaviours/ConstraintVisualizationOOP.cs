using UnityEngine;

public class ConstraintVisualizationOOP : MonoBehaviour
{
    ConstraintOOP[] constraints;
    
    private void Update()
    {
        if (constraints == null)
        {
            constraints = FindObjectsOfType<ConstraintOOP>();
        }
        
        // Satisfy Constraints
        foreach (var constraint in constraints)
        {
            ParticleOOP particleA = constraint.ParticleA;
            ParticleOOP particleB = constraint.ParticleB;
            
            Debug.DrawLine(particleA.Position, particleB.Position);                 // visualize the constraint
        }
    }
}
