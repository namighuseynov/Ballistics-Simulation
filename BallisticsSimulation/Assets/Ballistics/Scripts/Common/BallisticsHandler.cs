using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BallisticsSimulation
{
    public class BallisticsHandler : MonoBehaviour
    {
        #region Fields
        [Header("Ballistics")]
        [SerializeField] private BallisticsProperties _ballisticsProps;
        [SerializeField] private Transform _origin;

        [Header("Integrator")]
        [SerializeField] private IntegrationMethod _integrationMethod;
        [SerializeField] private bool _runtimeCalculate = true;
        [SerializeField] private double stepSize = 0.1f;
        [SerializeField] private int maxSteps = 10000;
        [SerializeField] private bool _enableLog = false;
        private IIntegrator _integrator;
        

        [Header("RK45 (Fehlberg)")]
        public double eps = 1e-4;  // local error
        public double hMin = 1e-4;  // min step
        public double hMax = 1.0;   // max step

        [Header("Trajectory")]
        private List<State> _trajectory = new();
        private Vector3 _directionVector = Vector3.zero;
        private Vector3 _straightVector = Vector3.zero;
        private Vector3 _rightVector = Vector3.zero;

        [Header("Wind")]
        [SerializeField] private WindZone _windZone;
        #endregion

        #region Unity loop
        private void Start()
        {
            if (_windZone == null)
            {
                _windZone = FindAnyObjectByType<WindZone>();
            }
            _integrator = Create(_integrationMethod);
        }
        private void FixedUpdate()
        {
            if (_runtimeCalculate)
                Recalculate();
        }
        #endregion

        #region Public API
        public IReadOnlyList<State> Trajectory => _trajectory;

        public double StepSize => stepSize;

        public Vector3 StraightVector => _straightVector;
        public Vector3 RightVector => _rightVector;
        public Vector3 DirectionVector => _directionVector;
        public Transform Origin => _origin;

        public void Recalculate() => CalculateTrajectory();
        #endregion

        #region Helpers
        private double GetAngle()
        {
            _directionVector = Direction switch
            {
                AxisDirection.Right => _origin.right,
                AxisDirection.Up => _origin.up,
                _ => _origin.forward,
            };

            _straightVector = new Vector3(_directionVector.x, 0, _directionVector.z).normalized;
            _rightVector = -Vector3.Cross(_straightVector, _directionVector);

            int sign = MathF.Sign(_directionVector.y);

            double angle = Vector3.Angle(_directionVector, _straightVector);
            return angle * Mathf.Deg2Rad * sign;
        }

        private Vector3 GetWind()
        {
            Vector3 main = _windZone.transform.forward;

            Vector3 turbDir = Quaternion.AngleAxis(
                                  UnityEngine.Random.Range(0f, 360f),
                                  Vector3.up) * main;

            float windSpeed = _windZone.windMain;
            float turbulenceAbs = _windZone.windTurbulence
                                * UnityEngine.Random.Range(-1f, 1f);

            return main * windSpeed + turbDir * turbulenceAbs;
        }

        private static IIntegrator Create(IntegrationMethod method)
        {
            return method switch
            {
                IntegrationMethod.Euler => new EulerIntegrator(),
                IntegrationMethod.RK4 => new RK4Integrator(),
                IntegrationMethod.RKF45 => new RKF45Integrator(),
                _ => throw new NotImplementedException()
            };
        }
        #endregion

        #region Properties
        public AxisDirection Direction => _ballisticsProps.axisDirection;
        #endregion

        #region Core math
        private void CalculateTrajectory()
        {
            double angleRad = GetAngle();
            State state = new State(
                0, _origin.position.y, 0,
                _ballisticsProps.startSpeed * Math.Cos(angleRad),
                _ballisticsProps.startSpeed * Math.Sin(angleRad),
                0, 0
            );

            _integrator = Create(_integrationMethod);
            _trajectory = _integrator.Calculate(
                state,
                stepSize,
                maxSteps,
                this,
                eps,
                hMin,
                hMax
            );

            if (_enableLog && !_runtimeCalculate)
            {
                List<string> logData = new List<string>();
                logData.Add("x,y,vx,vy,t");

                for (int i = 0; i < _trajectory.Count; i++)
                {
                    var currentCorner = _trajectory[i];
                    string line = string.Format("{0:F3},{1:F3},{2:F3},{3:F3},{4:F3}",
                        currentCorner.X,
                        currentCorner.Y,
                        currentCorner.Vx,
                        currentCorner.Vy,
                        currentCorner.T
                        );
                    logData.Add( line );
                }
                if (logData.Count > 0)
                {
                    string fileName = string.Empty;
                    if (_integrationMethod == IntegrationMethod.Euler) fileName = "sim_Euler.csv";
                    else if (_integrationMethod == IntegrationMethod.RK4) fileName = "sim_RK4.csv";
                    else if (_integrationMethod == IntegrationMethod.RKF45) fileName = "sim_RKF45.csv";

                    string filePath = Path.Combine(Application.dataPath, fileName);
                    File.WriteAllLines(filePath, logData.ToArray());
                    Debug.Log("CSV file was created at: " + filePath);
                }
            }
        }

        public State Derivatives(State s)
        {
            if (_ballisticsProps.mass == 0) _ballisticsProps.mass = 0.001;

            Vector3 vWorld = _straightVector * (float)s.Vx
                           + Vector3.up * (float)s.Vy
                           + _rightVector * (float)s.Vz;

            Vector3 vRel = vWorld - (_ballisticsProps.useWind ? GetWind() : Vector3.zero);

            double vMag = vRel.magnitude;
            double dragFactor = _ballisticsProps.airDensity
                              * _ballisticsProps.dragCoefficent
                              * _ballisticsProps.area * 0.5;

            Vector3 dragAcc = _ballisticsProps.useDrag
                            ? -(float)(dragFactor / _ballisticsProps.mass) * (float)vMag * vRel
                            : Vector3.zero;

            Vector3 gravity = _ballisticsProps.useGravity ? new Vector3(0, -9.8066f, 0) : Vector3.zero;
            Vector3 accWorld = gravity + dragAcc;

            if (_ballisticsProps.useThrust &&
    s.X > _ballisticsProps.ignitionDistance &&
    s.X < _ballisticsProps.ignitionDistance + _ballisticsProps.burnDistance)
            {
                Vector3 dir = vWorld.sqrMagnitude > 1e-6f ? vWorld.normalized : _straightVector;
                Vector3 thrustAcc = (float)(_ballisticsProps.thrustForce / _ballisticsProps.mass) * dir;
                accWorld += thrustAcc;
            }

            double ax = Vector3.Dot(accWorld, _straightVector);
            double ay = Vector3.Dot(accWorld, Vector3.up);
            double az = Vector3.Dot(accWorld, _rightVector);

            return new State(s.Vx, s.Vy, s.Vz, ax, ay, az, 1.0);
        }
        #endregion
    }
}