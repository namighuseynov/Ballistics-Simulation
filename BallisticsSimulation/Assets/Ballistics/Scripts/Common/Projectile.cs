using System.Collections;
using UnityEngine;

namespace BallisticsSimulation
{
    public class Projectile : MonoBehaviour
    {
        private BallisticsHandler _solver;
        private Transform _origin;
        [SerializeField] private GameObject _explosionEffect;

        public void Init(BallisticsHandler solver, Transform shotOrigin, float lifeTime)
        {
            _solver = solver;
            _origin = shotOrigin;
            StartCoroutine(Fly());
            Destroy(gameObject, lifeTime);
        }

        private IEnumerator Fly()
        {
            var path = _solver.GetTrajectory();
            if (path == null || path.Count < 2) yield break;

            float simTime = 0f;
            int seg = 0;

            Vector3 straight = _solver.StraightVector;
            Vector3 right = _solver.RightVector;

            while (seg < path.Count - 1)
            {
                simTime += Time.deltaTime;

                while (seg < path.Count - 1 && simTime >= (float)path[seg + 1].T)
                    seg++;

                if (seg >= path.Count - 1) break;

                float t0 = (float)path[seg].T;
                float t1 = (float)path[seg + 1].T;
                float k = Mathf.InverseLerp(t0, t1, simTime);

                Vector3 p0 = LocalToWorld(path[seg]);
                Vector3 p1 = LocalToWorld(path[seg + 1]);

                transform.position = Vector3.Lerp(p0, p1, k);
                Vector3 dir = (p1 - p0).normalized;
                if (dir.sqrMagnitude > 0f)
                    transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

                yield return null;
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