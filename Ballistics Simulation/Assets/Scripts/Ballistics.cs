using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ballistics : MonoBehaviour
{
    [Header("External Forces")]
    [SerializeField] private bool _useGravity = true;
    [SerializeField] private bool _useDrag = false;
    [SerializeField] private bool _useWind = false;
    [SerializeField] private bool _useThrust = false;


    [Header("Projectile")]
    [SerializeField] private float _projectileMass = 1;
    [SerializeField] private float _projectileArea = 1f;
    [SerializeField] private float _lifeTime = 10;

    [Header("Balistics")]
    [SerializeField] private float _dragCoefficent = 0.5f;
    [SerializeField] private float _airDensity;
    [SerializeField] private float _airTemperature;
    [SerializeField] private float _startingSpeed = 1;

    private Rigidbody _projectile;

    private float _dragForce = 0;
    //private float _windForce = 0;
    //private float _thrustForce = 0;

    private void Start()
    {
        _projectile = GetComponent<Rigidbody>();
        _projectile.mass = _projectileMass;
        _projectile.useGravity = _useGravity;
        if (_projectile != null)
        {
            Vector3 ImpulseForce = transform.forward * _startingSpeed;
            _projectile.AddForce(ImpulseForce, ForceMode.Impulse);
        }
        Destroy(_projectile.gameObject, _lifeTime); 
    }

    private void Update()
    {
        if (_useDrag)
        {
            CalculateDrag();
        }
        if (_useWind)
        {
            CalculateWind();
        }
        if (_useThrust)
        {
            CalculateThrust();
        }
        ApplyForces();
    }

    private void CalculateDrag()
    {
        _dragForce = _dragCoefficent * _projectileArea * _airDensity * Mathf.Pow(_projectile.velocity.magnitude, 2) / 2;
        Debug.Log(_dragForce);
    }

    private void CalculateWind() { }

    private void CalculateThrust() { }

    private void ApplyForces()
    {
        _projectile.AddForce(-transform.forward * _dragForce);
    }
}
