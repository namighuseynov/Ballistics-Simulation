using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ProjectileProperties ProjectileProperties;
    public BallisticSettings BallisticSettings;

    public bool ShowForceVectors = true;
    private Rigidbody rb;
    private float angle = 0;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        BallisticSettings = GameObject.Find("Weapon").GetComponent<BallisticSettings>();
        Destroy(gameObject, ProjectileProperties.liveTime);
        Vector3 velocityDirection = transform.up;
        rb.velocity = ProjectileProperties.StartingSpeed * velocityDirection;
    }

    private void FixedUpdate()
    {
        DrawBaseVectors();

        if (ShowForceVectors) { DrawMoveVector(); }

        if (BallisticSettings.useGravity) { CalculateGravity(); }
        if (BallisticSettings.useDrag) { CalculateDrag(); }
    }

    private float GetSpeed()
    {
        return rb.velocity.magnitude;
    }

    private float GetHeight()
    {
        return transform.position.y;
    }

    private void CalculateDrag()
    {
        Vector3 dragDirection = -rb.velocity.normalized;
        Vector3 drag = dragDirection * ProjectileProperties.Drag * BallisticSettings.AtmosphereDensity 
                        * Mathf.Pow(GetSpeed(), 2) * ProjectileProperties.Area;
        rb.AddForce(drag);
        if (ShowForceVectors) { Debug.DrawLine(transform.position, dragDirection + transform.position, Color.gray); }
    }

    private void CalculateWind()
    {
        return;
    }

    private void CalculateGravity()
    {
        Vector3 gravity = Physics.gravity*ProjectileProperties.Mass;
        rb.AddForce(gravity);
        if (ShowForceVectors) { Debug.DrawLine(transform.position, transform.position + gravity, Color.red); }
    }

    private float CalculateAngle()
    {
        return angle;
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
        Vector3 moveDirection = transform.position + rb.velocity;
        Debug.DrawLine(transform.position, moveDirection, Color.black);
    }
}
