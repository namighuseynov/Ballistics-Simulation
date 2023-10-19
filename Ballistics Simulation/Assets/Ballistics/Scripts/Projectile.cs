using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public ProjectileProperties     ProjectileProperties;
    public BallisticSettings        BallisticSettings;

    private Rigidbody               body;
    public Transform                CenterOfMass;

    public Vector3                  Gravity =        Vector3.zero;
    public Vector3                  Drag =           Vector3.zero;
    public Vector3                  Wind =           Vector3.zero;

    //Projectile properies
    private float                   area;
    private float                   weight;
    private float                   dragCoefficent;
    private float                   startingSpeed;

    //Ballistic settings
    private const float             L =             -0.0065f;
    private const float             R =              8.31447f;
    private const float             M =              0.029f;

    private bool                    useGravity;
    private bool                    useDrag;
    private bool                    useWind;
    private float                   atmoshereTemperature;
    private float                   atmoshereDensity;


    private void Start()
    {
        body = GetComponent<Rigidbody>();
        if (body != null && ProjectileProperties != null)
        {
            BallisticSettings = GameObject.FindGameObjectWithTag("Weapon").GetComponent<BallisticSettings>();
            Destroy(gameObject, ProjectileProperties.liveTime);

            area = ProjectileProperties.Area;
            weight = ProjectileProperties.Weight;
            dragCoefficent = ProjectileProperties.dragCoefficient;
            startingSpeed = ProjectileProperties.StartingSpeed;

            atmoshereDensity = BallisticSettings.AtmosphereDensity;
            atmoshereTemperature = BallisticSettings.AtmosphereTemperature;
            useGravity = BallisticSettings.useGravity;
            useDrag = BallisticSettings.useDrag;
            useWind = BallisticSettings.useWindForce;
            Vector3 velocityDirection = Vector3.zero;
            switch (ProjectileProperties.ShotDirection)
            {
                case ShotDirection.FORWARD:
                    velocityDirection = transform.forward;
                    break;
                case ShotDirection.UP:
                    velocityDirection = transform.up;
                    break;
                case ShotDirection.RIGHT:
                    velocityDirection = transform.right;
                    break;
                default:
                    velocityDirection = transform.forward;
                    break;
            }
            body.velocity = startingSpeed * velocityDirection;
        }
    }

    private void FixedUpdate()
    {
        body.useGravity = false;
        if (useGravity) { CalculateGravity(); }
        if (useDrag) { CalculateDrag(); }

        GetDensity();
        
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

    }

    private float GetTemperature()
    {
        return (atmoshereTemperature + transform.position.y * L);
    }

    private float GetPressure()
    {
        //float s_pressure = R * atmoshereTemperature / 22.4f;
        float power = (-Mathf.Abs(-Physics.gravity.y) * M) / (R * L);

        float var = 1 + (L * transform.position.y / atmoshereTemperature);
        float pressure = 101325f * Mathf.Pow(var, power);
        //float pressure2 = 101325f * Mathf.Exp((-M * Mathf.Abs(Physics.gravity.y)*transform.position.y)/(R*GetTemperature()));
        return pressure;
    }

    private float GetDensity()
    {
        float density = (GetPressure() * M) / (R * GetTemperature());
        atmoshereDensity = density;
        return density;
    }

    private float GetSpeed()
    {
        return body.velocity.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            Destroy(this.gameObject);
        }
        
    }
}
