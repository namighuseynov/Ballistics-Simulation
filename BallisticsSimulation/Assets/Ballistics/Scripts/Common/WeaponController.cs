using UnityEngine;

namespace BallisticsSimulation
{
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        public BallisticsHandler handler;
        public GameObject projectilePrefab;
        public Transform shotOrigin;
        public float projectileLifeTime = 10;
        #endregion

        #region Unity loop
        private void Awake()
        {
            if (handler == null)
                handler = FindAnyObjectByType<BallisticsHandler>();
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
            if (handler != null)
            {
                handler.GetTrajectory();

                var projectileGO = Instantiate(projectilePrefab);
                projectileGO.transform.position = transform.position;
                projectileGO.transform.rotation = transform.rotation;

                if (projectileGO.TryGetComponent(out Projectile proj))
                    proj.Init(handler, shotOrigin, projectileLifeTime);
            }
        }
        #endregion
    }
}