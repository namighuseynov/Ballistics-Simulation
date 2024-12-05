using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    /// <summary>
    /// Projectile class
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        #region Fields
        public ProjectileProperties ProjectileProperties;
        public BallisticSettings BallisticSettings;

        [Header("Impact forces")]
        private Rigidbody body;
        public Transform CenterOfMass;

        public Vector3 Gravity = Vector3.zero;
        public Vector3 Drag = Vector3.zero;
        public Vector3 Wind = Vector3.zero;

        //Projectile properies
        [Header("ProjectileProperties")]
        private float area;
        private float weight;
        private float dragCoefficent;
        private float startingSpeed;

        //Ballistic settings
        [Header("Ballistics settings")]
        private const float L = -0.0065f;
        private const float R = 8.31447f;
        private const float M = 0.029f;

        private bool useGravity;
        private bool useDrag;
        private bool useWind;
        private float atmosphereTemperature;
        private float atmosphereDensity;
        #endregion

        #region Events
        public event Action<Transform> OnProjectileTriggered;
        #endregion

        #region Methods
        private void Start()
        {
            body = GetComponent<Rigidbody>();
            if (body == null || ProjectileProperties == null)
            {
                Debug.LogError("Rigidbody or ProjectileProperties is missing.");
                return;
            }
            BallisticSettings = GameObject.FindGameObjectWithTag("Weapon")?.GetComponent<BallisticSettings>();
            if (BallisticSettings == null)
            {
                Debug.LogError("BallisticSettings component not found on Weapon object.");
                return;
            }
            Destroy(gameObject, ProjectileProperties.liveTime);
            area = ProjectileProperties.Area;
            weight = ProjectileProperties.Weight;
            dragCoefficent = ProjectileProperties.dragCoefficient;
            startingSpeed = ProjectileProperties.StartingSpeed;

            atmosphereDensity = BallisticSettings.AtmosphereDensity;
            atmosphereTemperature = BallisticSettings.AtmosphereTemperature;
            useGravity = BallisticSettings.useGravity;
            useDrag = BallisticSettings.useDrag;
            useWind = BallisticSettings.useWindForce;
            Vector3 velocityDirection = transform.forward;
            switch (ProjectileProperties.ShotDirection)
            {
                case ShotDirection.UP:
                    velocityDirection = transform.up;
                    break;
                case ShotDirection.RIGHT:
                    velocityDirection = transform.right;
                    break;
            }
            body.velocity = startingSpeed * velocityDirection;
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
            Drag = dragDirection * dragCoefficent * atmosphereDensity
                            * Mathf.Pow(GetSpeed(), 2) * area;
            body.AddForce(Drag);
        }
        private float GetTemperature()
        {
            return (atmosphereTemperature + transform.position.y * L);
        }
        private float GetPressure()
        {
            float power = (-Mathf.Abs(-Physics.gravity.y) * M) / (R * L);

            float var = 1 + (L * transform.position.y / atmosphereTemperature);
            float pressure = 101325f * Mathf.Pow(var, power);

            return pressure;
        }
        private float GetDensity()
        {
            float density = (GetPressure() * M) / (R * GetTemperature());
            atmosphereDensity = density;
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
                OnProjectileTriggered?.Invoke(transform);
            }
        }

        #endregion
    }
}