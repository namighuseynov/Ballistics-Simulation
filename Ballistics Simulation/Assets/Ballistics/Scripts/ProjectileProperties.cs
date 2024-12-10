using UnityEngine;

namespace BallisticsSimulation
{
    [CreateAssetMenu(fileName = "ProjectileProperties", menuName = "ScriptableObjects/ProjectileProperties", order = 1)]
    public class ProjectileProperties : ScriptableObject
    {
        #region Fields
        [Header("Ballistics")]
        public float Weight = 1.00f;                    // kg
        public float Area = 0.50f;                      // m^2
        public float dragCoefficient = 0.50f;           // const
        public float StartingSpeed = 10.0f;             // m/s

        [Header("Engine")]
        public float GasOutflowSpeed;                   // m/s
        public float FuelMass;                          // kg
        public float FuelBurningTime;                   // s

        [Header("Time Update")]
        public float deltaTime = 0.5f;

        [Header("Projectile")]
        public ShotDirection ShotDirection = ShotDirection.FORWARD;
        public float liveTime = 15f;
        public ProjectileUpdateMode updateMode = ProjectileUpdateMode.VELOCITY_CHANGE;
        #endregion
    }
}