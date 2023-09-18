using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ProjectileProperties     ProjectileProperties;
    public BallisticSettings        BallisticSettings;

    public bool                     ShowForceVectors = true;
    private Rigidbody               body;
    public Transform CenterOfMass;
    private float                   angle = 0;
    
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        BallisticSettings = GameObject.Find("Weapon").GetComponent<BallisticSettings>();
        Destroy(gameObject, ProjectileProperties.liveTime);
        Vector3 velocityDirection = transform.up;
        body.velocity = ProjectileProperties.StartingSpeed * velocityDirection;
    }

    private void FixedUpdate()
    {
        body.useGravity = false;
        DrawBaseVectors();

        if (ShowForceVectors) { DrawMoveVector(); }
        if (BallisticSettings.useGravity) { CalculateGravity(); }
        if (BallisticSettings.useDrag) { CalculateDrag(); }
    }

    private float GetSpeed()
    {
        return body.velocity.magnitude;
    }

    private float GetHeight()
    {
        return transform.position.y;
    }

    private void CalculateDrag()
    {
        Vector3 dragDirection = -body.velocity.normalized;
        Vector3 drag = dragDirection * ProjectileProperties.dragCoefficient * BallisticSettings.AtmosphereDensity 
                        * Mathf.Pow(GetSpeed(), 2) * ProjectileProperties.Area;
        body.AddForce(drag);
        if (ShowForceVectors) { Debug.DrawLine(transform.position, dragDirection + transform.position, Color.gray); }
    }

    private void CalculateWind()
    {
        return;
    }

    private void CalculateGravity()
    {
        Vector3 gravity = Physics.gravity*ProjectileProperties.Weight;
        body.AddForceAtPosition(gravity, CenterOfMass.position);
        if (ShowForceVectors) { Debug.DrawLine(CenterOfMass.position, CenterOfMass.position + gravity, Color.red); }
    }

    private void DrawBaseVectors()
    {
        Vector3 Axis = transform.position;
        Vector3 xBasis = transform.up;
        Vector3 yBasis = transform.up;
        yBasis.y = 0f;
        yBasis.Normalize();
        Debug.DrawLine(Axis, Axis + xBasis, Color.green);
        Debug.DrawLine(Axis, Axis + yBasis, Color.blue);
        angle = Vector3.Angle(xBasis, yBasis);
    }
    private void DrawMoveVector()
    {
        Vector3 moveDirection = transform.position + body.velocity;
        Debug.DrawLine(transform.position, moveDirection, Color.black);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CenterOfMass.position, 0.03f);
    }
}
