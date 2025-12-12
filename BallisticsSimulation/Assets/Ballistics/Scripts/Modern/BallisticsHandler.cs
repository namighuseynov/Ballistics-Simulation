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
        [SerializeField] private AtmosphereProperties _atmosphereProps;
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
        private readonly List<State> _trajectory = new();
        private Vector3 _directionVector = Vector3.zero;
        private Vector3 _straightVector = Vector3.zero;
        private Vector3 _rightVector = Vector3.zero;
        [SerializeField] private bool _optimizeTrajectoryCalculation = false;

        int _stateHash;

        [Header("Gravity")]
        const float EarthRadius = 6371000f; // m

        [Header("Wind")]
        [SerializeField] private WindZone _windZone;
        [SerializeField] private float gustSigma = 1f;  
        [SerializeField] private float gustFreq = 0.5f; 
        [SerializeField] private int gustSeed = 12345;
        [SerializeField] private float U0_500 = 2f;
        [SerializeField] private float U500_2000 = 8f;
        [SerializeField] private float U2000p = 15f;


        #endregion

        #region Unity 
        private void Start()
        {
            if (_windZone == null)
            {
                _windZone = FindAnyObjectByType<WindZone>();
            }
            _integrator = Create(_integrationMethod);

            _stateHash = ComputeStateHash();
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
            _rightVector = -Vector3.Cross(_straightVector, _directionVector).normalized;

            int sign = MathF.Sign(_directionVector.y);

            double angle = Vector3.Angle(_directionVector, _straightVector);
            return angle * Mathf.Deg2Rad * sign;
        }

        static float SmoothStep(float a, float b, float x)
        {
            float t = Mathf.Clamp01((x - a) / (b - a));
            return t * t * (3f - 2f * t); 
        }
        private float LayeredMean(float h)
        {
            float U0 = (_windZone ? _windZone.windMain : U0_500);
            float u12 = Mathf.Lerp(U0, U500_2000, SmoothStep(0f, 500f, h));
            float u23 = Mathf.Lerp(U500_2000, U2000p, SmoothStep(500f, 2000f, h));
            return (h <= 2000f) ? u12 : u23;
        }

        private float GustValue(double t)
        {
            float u = (float)(gustFreq * t);

            return gustSigma * (2f * Mathf.PerlinNoise(gustSeed, u) - 1f);
        }

        private Vector3 GetWind(State s)
        {
            Vector3 dir = (_windZone != null ? _windZone.transform.forward : Vector3.right).normalized;

            float h = Mathf.Max(0f, (float)s.Y);
            float Umean = LayeredMean(h);
            float Ugust = GustValue(s.T);

            return dir * Mathf.Max(0f, Umean + Ugust);
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

        int ComputeStateHash()
        {
            HashCode hc = new HashCode();
            hc.Add(_origin.position);
            hc.Add(_origin.rotation);

            if (_windZone != null)
            {
                hc.Add(_windZone.windMain);
                hc.Add(_windZone.windTurbulence);
                hc.Add(_windZone.transform.forward);
            }

            // Wind
            hc.Add(gustSigma);
            hc.Add(gustFreq);
            hc.Add(gustSeed);

            // Atmosphere
            hc.Add(_atmosphereProps.Temperature);
            hc.Add(_atmosphereProps.L);
            hc.Add(_atmosphereProps.R);
            hc.Add(_atmosphereProps.g0);

            // Ballistics
            hc.Add(_ballisticsProps.startSpeed);
            hc.Add(_ballisticsProps.dragCoefficent);
            hc.Add(_ballisticsProps.airDensity);
            hc.Add(_ballisticsProps.area);
            hc.Add(_ballisticsProps.mass);
            hc.Add(_ballisticsProps.useGravity);
            hc.Add(_ballisticsProps.useDrag);
            hc.Add(_ballisticsProps.useWind);
            hc.Add(_ballisticsProps.useThrust);
            hc.Add(_ballisticsProps.thrustForce);
            hc.Add(_ballisticsProps.IgnitionTime);
            hc.Add(_ballisticsProps.BurnTime);
            hc.Add(stepSize);
            hc.Add(_integrationMethod);

            // Others
            hc.Add(eps);
            hc.Add(hMin);
            hc.Add(hMax);
            hc.Add(U0_500);
            hc.Add(U500_2000);
            hc.Add(U2000p);

            return hc.ToHashCode();
        }
        #endregion

        #region Properties
        public AxisDirection Direction => _ballisticsProps.axisDirection;
        public WindZone WindZone => _windZone;
        public BallisticsProperties Props => _ballisticsProps;
        #endregion

        #region Core math
        private void RecordLog()
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
                logData.Add(line);
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
        private void Recalculate()
        {
            _trajectory.Clear();
            double angleRad = GetAngle();

            State state = new State(
                0, _origin.position.y, 0,
                _ballisticsProps.startSpeed * Math.Cos(angleRad),
                _ballisticsProps.startSpeed * Math.Sin(angleRad),
                0, 0
            );

            _integrator = Create(_integrationMethod);
            _trajectory.AddRange(_integrator.Calculate(state,
                stepSize,
                maxSteps,
                this,
                eps,
                hMin,
                hMax
                ));

            if (_enableLog)
            {
                RecordLog();
            }
            Debug.Log("Recalculated");
        }
        private void FixedUpdate()
        {
            if (_runtimeCalculate)
            {
                Recalculate();
            }
        }
        public IReadOnlyList<State> GetTrajectory()
        {
            if (_optimizeTrajectoryCalculation)
            {
                int h = ComputeStateHash();
                if (h != _stateHash)
                {
                    _stateHash = h;
                    Recalculate();
                }
            }
            return _trajectory;
        }
        public State Derivatives(State s)
        {
            if (_ballisticsProps.mass == 0) _ballisticsProps.mass = 0.001;

            Vector3 vWorld = _straightVector * (float)s.Vx
                           + Vector3.up * (float)s.Vy
                           + _rightVector * (float)s.Vz;

            Vector3 vRel = vWorld - (_ballisticsProps.useWind ? GetWind(s) : Vector3.zero);

            double vMag = vRel.magnitude;
            float density = Density((float)s.Y);
            double dragFactor = density
                              * _ballisticsProps.dragCoefficent
                              * _ballisticsProps.area * 0.5;

            Vector3 dragAcc = _ballisticsProps.useDrag
                            ? -(float)(dragFactor / _ballisticsProps.mass) * (float)vMag * vRel
                            : Vector3.zero;

            Vector3 gravity = _ballisticsProps.useGravity ? Gravity((float)s.Y) : Vector3.zero;
            Vector3 accWorld = gravity + dragAcc;

            if (_ballisticsProps.useThrust &&
    s.T > _ballisticsProps.IgnitionTime &&
    s.T < _ballisticsProps.IgnitionTime + _ballisticsProps.BurnTime)
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
        private Vector3 Gravity(float h)
        {
            float g = _atmosphereProps.g0 * Mathf.Pow(EarthRadius / (EarthRadius + Mathf.Max(0f, h)), 2f);
            return new Vector3(0f, -g, 0f);
        }
        private float Density(float altitude)
        {
            float b = 1.0f - (_atmosphereProps.L * altitude) / _atmosphereProps.Temperature; 
            if (b <= 0f) return 0f;

            float expo = _atmosphereProps.g0 / (_atmosphereProps.L * _atmosphereProps.R) - 1f;
            return _atmosphereProps.Density * Mathf.Pow(b, expo);
        }
        #endregion
    }
}