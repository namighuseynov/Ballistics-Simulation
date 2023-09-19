using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ProjectileProperties     ProjectileProperties;
    public BallisticSettings        BallisticSettings;

    private Rigidbody               body;
    public Transform CenterOfMass;

    public Vector3 Gravity = Vector3.zero;
    public Vector3 Drag = Vector3.zero;
    public Vector3 Wind = Vector3.zero;

    //Projectile properies
    private float area;
    private float weight;
    private float dragCoefficent;
    private float startingSpeed;

    //Ballistic settings
    private bool useGravity;
    private bool useDrag;
    private bool useWind;
    private float atmoshereDensity;


    private void Start()
    {
        body = GetComponent<Rigidbody>();
        if (body != null && ProjectileProperties != null)
        {
            BallisticSettings = GameObject.Find("Weapon").GetComponent<BallisticSettings>();
            Destroy(gameObject, ProjectileProperties.liveTime);

            area = ProjectileProperties.Area;
            weight = ProjectileProperties.Weight;
            dragCoefficent = ProjectileProperties.dragCoefficient;
            startingSpeed = ProjectileProperties.StartingSpeed;
            atmoshereDensity = BallisticSettings.AtmosphereDensity;
            useGravity = BallisticSettings.useGravity;
            useDrag = BallisticSettings.useDrag;
            useWind = BallisticSettings.useWindForce;

            Vector3 velocityDirection = transform.up;
            body.velocity = startingSpeed * velocityDirection;
        }
    }

    private void FixedUpdate()
    {
        body.useGravity = false;

        if (useGravity) { CalculateGravity(); }
        if (useDrag) { CalculateDrag(); }
    }

    private void CalculateGravity()
    {
        Gravity = Physics.gravity * weight;
        body.AddForceAtPosition(Gravity, CenterOfMass.position);
    }

    private void CalculateDrag()
    {
        Vector3 dragDirection = -body.velocity.normalized;
        Drag = dragDirection * dragCoefficent * atmoshereDensity
                        * Mathf.Pow(GetSpeed(), 2) * area;
        body.AddForce(Drag);
        Debug.Log(Drag);
    }

    private float GetSpeed()
    {
        return body.velocity.magnitude;
    }
}
