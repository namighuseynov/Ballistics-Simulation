using UnityEngine;

namespace BallisticsSimulation
{
    /// <summary>
    /// Atmoshpere properties
    /// </summary>
    [CreateAssetMenu(fileName = "AtmosphereProperties", menuName = "ScriptableObjects/AtmosphereProperties", order = 1)]
    public class AtmosphereProperties : ScriptableObject
    {
        #region Fields
        /// <summary>
        /// Normal Temperature
        /// </summary>
        public float Temperature = 288.15f;
        /// <summary>
        /// Normal Pressure
        /// </summary>
        public float Pressure = 101325f;
        /// <summary>
        /// Normal Density
        /// </summary>
        public float Density = 1.225f;
        public float L = 0.0065f;
        public float g0 = 9.80665f;
        public float R = 287.05f;
        #endregion Fields
    }
}