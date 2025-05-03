using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation.Common
{
    public class Projectile : MonoBehaviour
    {
        private BallisticsHandler _solver;
        private Transform _origin;
        [SerializeField] private GameObject _explosionEffect;
        private Vector3 _startPosition = Vector3.zero;
        private float _lifeTime = 15;

        public void Init(BallisticsHandler solver, Transform shotOrigin)
        {
            _solver = solver;
            _origin = shotOrigin;
            StartCoroutine(Fly());
            _startPosition = _origin.position;
            _startPosition.y = 0;
            Destroy(gameObject, _lifeTime);
        }

        private IEnumerator Fly()
        {
            IReadOnlyList<State> path = _solver.Trajectory;
            if (path == null || path.Count < 2) yield break;

            double dt = _solver.StepSize;
            Vector3 straight = _solver.StraightVector;
            Vector3 right = _solver.RightVector;

            for (int i = 1; i < path.Count; i++)
            {
                Vector3 p0 = LocalToWorld(path[i - 1]);
                Vector3 p1 = LocalToWorld(path[i]);

                float t = 0f;
                while (t < dt)
                {
                    t += Time.deltaTime;
                    float k = Mathf.Clamp01((float)(t / dt));
                    transform.position = Vector3.Lerp(p0, p1, k);

                    Vector3 dir = (p1 - p0).normalized;
                    if (dir.sqrMagnitude > 0f)
                        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                    yield return null;
                }
            }

            Vector3 LocalToWorld(State s) =>
                (new Vector3(_origin.position.x, 0, _origin.position.z)) +
                straight * (float)s.X
              + Vector3.up * (float)s.Y
              + right * (float)s.Z;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Floor")
            {
                GameObject explosion = Instantiate(_explosionEffect);
                explosion.transform.position = transform.position;

                Vector3 finishPosition = transform.position;
                finishPosition.y = 0;
                Destroy(gameObject);
            }
        }
    }
}
