using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    public class Projectile : MonoBehaviour
    {
        private List<State> _path;
        [SerializeField] private GameObject _explosionEffect;
        public event EventHandler<ProjectileHitEventArgs> OnHit;

        public void Init(BallisticsHandler solver, float lifeTime)
        {
            _path = new List<State>(solver.GetTrajectory());

            if (_path.Count > 0)
            {
                StartCoroutine(Fly());
            }

            Destroy(gameObject, lifeTime);
        }

        private IEnumerator Fly()
        {
            var path = _path;
            if (path == null || path.Count < 2) yield break;

            float simTime = 0f;
            int seg = 0;

            while (seg < path.Count - 1)
            {
                simTime += Time.deltaTime;

                while (seg < path.Count - 1 && simTime >= (float)path[seg + 1].T)
                    seg++;

                if (seg >= path.Count - 1) break;

                float t0 = (float)path[seg].T;
                float t1 = (float)path[seg + 1].T;
                float k = Mathf.InverseLerp(t0, t1, simTime);

                Vector3 p0 = path[seg].Position;
                Vector3 p1 = path[seg + 1].Position;

                transform.position = Vector3.Lerp(p0, p1, k);
                Vector3 dir = (p1 - p0).normalized;
                if (dir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(dir);

                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Vector3 hitPoint = transform.position;
            Vector3 normal = Vector3.up;

            if (other != null)
            {
                hitPoint = other.ClosestPoint(transform.position);
                normal = (transform.position - hitPoint).sqrMagnitude > 0f
                    ? (transform.position - hitPoint).normalized
                    : Vector3.up;
            }

            OnHit?.Invoke(this, new ProjectileHitEventArgs(other?.gameObject, other, hitPoint, normal, transform.position));

            if (other.CompareTag("Floor"))
            {
                var explosion = Instantiate(_explosionEffect);
                explosion.transform.position = transform.position;

                Destroy(gameObject);
            }
        }
    }
}