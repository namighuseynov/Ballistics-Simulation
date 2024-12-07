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
            body.useGravity = false;
            if (BallisticSettings.UseGravity)
            {
                float height = transform.position.y;

                Vector3 gravity = new Vector3(0, calculator.CalculateGravity(height), 0);
                body.AddForceAtPosition(gravity, CenterOfMass.position);
            } 
            if (BallisticSettings.UseDrag)
            {
                Vector3 drag = new Vector3(calculator.CalculateDrag(transform.position.y, body.velocity.x),
                    calculator.CalculateDrag(transform.position.y, body.velocity.y),
                    calculator.CalculateDrag(transform.position.y, body.velocity.z));
                body.AddForceAtPosition(drag, CenterOfMass.position);
                Debug.Log(drag);
            }
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