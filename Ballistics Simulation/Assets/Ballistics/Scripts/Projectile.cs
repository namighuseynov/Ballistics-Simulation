using System;
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
        private float startingSpeed;

        //Ballistic settings
        [Header("Ballistics settings")]
        [SerializeField] private BallisticsCalculator calculator;
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
            body.useGravity = false;
            BallisticSettings = GameObject.FindGameObjectWithTag("Weapon")?.GetComponent<BallisticSettings>();
            if (BallisticSettings == null)
            {
                Debug.LogError("BallisticSettings component not found on Weapon object.");
                return;
            }
            if (calculator == null)
            {
                calculator = GameObject.FindGameObjectWithTag("Weapon")?.GetComponent<BallisticsCalculator>();
            }
            Destroy(gameObject, ProjectileProperties.liveTime);

            startingSpeed = ProjectileProperties.StartingSpeed;

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
            if (BallisticSettings.UseGravity)
            {
                float height = transform.position.y;
                Vector3 gravity = calculator.CalculateGravity(height);
                body.AddForce(gravity, ForceMode.Acceleration);
            }
            if (BallisticSettings.UseDrag)
            {
                float height = transform.position.y;
                Vector3 velocity = body.velocity;
                Vector3 drag = calculator.CalculateDrag(height, velocity);
                drag = transform.up * -drag.x + transform.right * -drag.y + transform.forward * -drag.z;
                body.AddForce(drag, ForceMode.Acceleration);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Floor")
            {
                OnProjectileTriggered?.Invoke(transform);
                Destroy(gameObject);
            }
        }

        #endregion
    }
}