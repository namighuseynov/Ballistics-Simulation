using UnityEngine;

namespace BallisticsSimulation
{
    [CreateAssetMenu(fileName = "BallisticsProperties", menuName = "ScriptableObjects/BallisticsProperties", order = 1)]
    public class BallisticsProperties : ScriptableObject
    {
        [Header("Forces")]
        public bool useGravity =                true;
        public bool useDrag =                   false;
        public bool useWind =                   false;
        public AxisDirection axisDirection =    AxisDirection.Forward;

        public double startSpeed =              100;
        public double dragCoefficent =          0.5;
        public double airDensity =              1.255;
        public double area =                    0.009;
        public double mass =                    2;

        [Header("Rocket motor")]
        public bool useThrust =                 false;
        public double thrustForce =             700;
        public double IgnitionTime =            15;
        public double BurnTime =                120;

    }
}