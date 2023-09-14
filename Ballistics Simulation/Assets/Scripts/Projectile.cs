using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ProjectileProperties ProjectileProperties;
    public BallisticSettings BallisticSettings;

    private Rigidbody rb;
    private float angle = 0;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        BallisticSettings = GetComponent<BallisticSettings>();
        Destroy(gameObject, ProjectileProperties.liveTime);
        Vector3 velocityDirection = transform.up;
        rb.velocity = ProjectileProperties.StartingSpeed * velocityDirection;
    }

    private void FixedUpdate()
    {
        DrawBaseVectors();
        CalculateDrag();
    }

    private float GetSpeed()
    {
        return rb.velocity.magnitude;
    }

    private float GetHeight()
    {
        return transform.position.y;
    }

    private float CalculateDrag()
    {
        Vector3 dragDirection = -transform.up;
        Vector3 drag = dragDirection * ProjectileProperties.Drag * BallisticSettings.AtmosphereDensity 
                        * Mathf.Pow(GetSpeed(), 2) * ProjectileProperties.Area * Mathf.Sin(angle * Mathf.PI / 180);

        rb.AddForce(drag);
        Debug.Log(drag);
        return 0f;
    }

    private float CalculateWind()
    {
        return 0f;
    }

    private float CalculateGravity()
    {
        return (Physics.gravity.y * ProjectileProperties.Mass);
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
}
