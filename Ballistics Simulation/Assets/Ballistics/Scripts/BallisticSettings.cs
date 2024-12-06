using UnityEngine;

namespace BallisticsSimulation
{
    /// <summary>
    /// Ballistic settings
    /// </summary>
    public class BallisticSettings : MonoBehaviour
    {
        #region Fields
        public bool UseGravity = true;
        public bool UseDrag = false;
        public bool UseWindForce = false;
        public bool UseThrust = false;

        public WindZone Wind;
        #endregion
    }
}