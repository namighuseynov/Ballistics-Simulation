using UnityEngine;

namespace BallisticsSimulation
{
    public class ProjectileGraph : MonoBehaviour
    {
        private Projectile projectile;
        private Transform centerOfMass;
        private Rigidbody body;

        private void Start()
        {
            projectile = GetComponent<Projectile>();
            if (projectile != null)
            {
                centerOfMass = projectile.CenterOfMass;
                body = GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            DrawGravityVector();
            DrawMovementVector();
            DrawDragVector();
        }

        private void DrawMovementVector()
        {
            Vector3 velocity = body.velocity;
            Vector3 direction = transform.position + velocity;
            Debug.DrawLine(transform.position, direction, Color.green);
        }

        private void DrawGravityVector()
        {
            Vector3 gravity = projectile.Gravity;
            Vector3 direction = centerOfMass.position + gravity;
            Debug.DrawLine(centerOfMass.position, direction, Color.red);
        }

        private void DrawDragVector()
        {
            Vector3 drag = projectile.Drag;
            Vector3 direction = transform.position + drag;
            Debug.DrawLine(transform.position, direction, Color.yellow);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(centerOfMass.position, 0.03f);
        }
    }
}