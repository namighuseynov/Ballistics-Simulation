using UnityEngine;

namespace BallisticsSimulation
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Ballistics")]
        public BallisticSettings ballisticSettings;

        [Header("Projectile")]
        public GameObject ProjectileObject;
        public Transform ProjectileSpawnPoint;

        private void Start()
        {
            ballisticSettings = GetComponent<BallisticSettings>();
        }

        virtual public void Shot()
        {
            GameObject Projectile = Instantiate(ProjectileObject);
            Projectile.transform.position = ProjectileSpawnPoint.position;
            Projectile.transform.rotation = ProjectileSpawnPoint.rotation;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shot();
            }
        }
    }
}