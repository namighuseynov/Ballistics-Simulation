using UnityEngine;

namespace BallisticsSimulation
{
    [CreateAssetMenu(fileName = "BallisticsProperties", menuName = "ScriptableObjects/BallisticsProperties", order = 1)]
    public class BallisticsProperties : ScriptableObject
    {
        [Header("Forces")]
        public bool useGravity;
        public bool useDrag;
        public bool useWind;
        public AxisDirection axisDirection;

        public double startSpeed;
        public double dragCoefficent;
        public double airDensity;
        public double area;
        public double mass;

    }
}