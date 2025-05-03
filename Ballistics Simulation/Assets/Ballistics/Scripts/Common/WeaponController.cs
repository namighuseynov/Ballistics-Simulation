using UnityEngine;

namespace BallisticsSimulation.RK4
{
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private BallisticsHandler _handler;
        [SerializeField] private GameObject _rkProjectilePrefab;
        [SerializeField] private Transform _shotOrigin;
        #endregion

        #region Unity loop
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

        #region Shooting
        private void Shot()
        {
            if (_handler != null)
            {
                _handler.Recalculate();

                var projectileGO = Instantiate(_rkProjectilePrefab);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = transform.rotation;

                if (projectileGO.TryGetComponent(out Common.Projectile proj))
                    proj.Init(_handler, _shotOrigin);
            }
        }
        #endregion
    }
}
