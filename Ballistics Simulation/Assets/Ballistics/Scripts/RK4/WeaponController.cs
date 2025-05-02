using UnityEngine;

namespace BallisticsSimulation.RK4
{
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RK4BallisticsHandler _rk4Handler;
        [SerializeField] private GameObject _rkProjectilePrefab;
        [SerializeField] private Transform _shotOrigin;
        #endregion

        #region Unity loop
        private void Awake()
        {
            if (_rk4Handler == null)
                _rk4Handler = FindAnyObjectByType<RK4BallisticsHandler>();
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0) && _rk4Handler != null)
                Shot();
        }
        #endregion

        #region Shooting
        private void Shot()
        {
            _rk4Handler.Recalculate();

            var projectileGO = Instantiate(_rkProjectilePrefab);
            projectileGO.transform.position = transform.position;
            projectileGO.transform.rotation = transform.rotation;

            if (projectileGO.TryGetComponent(out RK4Projectile proj))
                proj.Init(_rk4Handler, _shotOrigin);
        }
        #endregion
    }
}
