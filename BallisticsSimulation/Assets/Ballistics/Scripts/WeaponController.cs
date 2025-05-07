using UnityEngine;

namespace BallisticsSimulation
{
    /// <summary>
    /// Weapon controller
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        #region Fields
        [Header("Projectile")]
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GameObject _projectilePrefab;
        #endregion

        #region Methods
        virtual public void Shot()
        {
            GameObject projectile = Instantiate(_projectilePrefab);
            projectile.transform.position = _projectileSpawnPoint.position;
            projectile.transform.rotation = _projectileSpawnPoint.rotation;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shot();
            }
        }
        #endregion
    }
}