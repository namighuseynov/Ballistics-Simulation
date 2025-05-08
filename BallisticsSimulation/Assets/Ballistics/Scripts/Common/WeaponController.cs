using UnityEngine;

namespace BallisticsSimulation
{
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private BallisticsHandler _handler;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _shotOrigin;
        [SerializeField] private float _projectileLifeTime = 10;
        #endregion

        #region Unity loop
        private void Awake()
        {
            if (_handler == null)
                _handler = FindAnyObjectByType<BallisticsHandler>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Shot();
        }
        #endregion

        #region Shoot
        public void Shot()
        {
            if (_handler != null)
            {
                _handler.GetTrajectory();

                var projectileGO = Instantiate(_projectilePrefab);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = transform.rotation;

                if (projectileGO.TryGetComponent(out Projectile proj))
                    proj.Init(_handler, _shotOrigin, _projectileLifeTime);
            }
        }
        #endregion
    }
}