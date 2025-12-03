using UnityEngine;

public class show_position : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.75f, 0.0f, 0.0f, 0.75f);

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(Vector3.zero, 2f);
    }
}
