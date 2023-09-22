using UnityEngine;

public class TestCenterOfMass : MonoBehaviour
{
    public Transform CenterOfMass;
    private Rigidbody body;


    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        body.useGravity = false;
        ApplyGravityForce();
    }

    private void ApplyGravityForce()
    {
        Vector3 gravity = Physics.gravity*body.mass;
        body.AddForceAtPosition(gravity, CenterOfMass.position);
        Debug.DrawLine(CenterOfMass.position, CenterOfMass.position + gravity, Color.red);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CenterOfMass.position, 0.1f);

    }
}
