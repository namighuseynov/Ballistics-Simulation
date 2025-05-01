using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallisticsSimulation
{
    public class RK4BallisticsHandler : Handler
    {
        #region Fields
        [Header("Ballistics")]
        [SerializeField] private BallisticsProperties _ballisticsProps;

        [Header("RK4")]
        public double stepSize = 0.1f;
        public int maxSteps = 10000;

        [Header("Trajectory")]
        public List<State> trajectory = new List<State>();
        private Vector3 _directionVector = Vector3.zero;
        private Vector3 _straightVector = Vector3.zero;
        private Vector3 _rightVector = Vector3.zero;

        [Header("Wind")]
        [SerializeField] private WindZone _windZone;
        #endregion

        #region Methods

        private void FixedUpdate()
        {
            CalculateTrajectory();
        }

        private void CalculateTrajectory()
        {
            double angleRad = GetAngle();
            int counter = 0;
            trajectory.Clear();

            State state = new State(
                0, 0, 0,
                _ballisticsProps.startSpeed * Math.Cos(angleRad),
                _ballisticsProps.startSpeed * Math.Sin(angleRad),
                0
            );
            trajectory.Add(new State(state));

            while (counter < maxSteps && state.Y >= 0.0)
            {
                State k1 = Derivatives(state);
                State k2 = Derivatives(state.Add(k1.Dot(stepSize*0.5)));
                State k3 = Derivatives(state.Add(k2.Dot(stepSize * 0.5)));
                State k4 = Derivatives(state.Add(k3.Dot(stepSize)));

                State delta = (k1.Add(k2.Dot(2)).Add(k3.Dot(2)).Add(k4)).Dot(stepSize / 6.0);
                state = state.Add(delta);

                trajectory.Add(new State(state));
                counter++;
            }
        }

        private State Derivatives(State s)
        {
            if (_ballisticsProps.mass == 0) _ballisticsProps.mass = 0.001;

            Vector3 vWorld = _straightVector * (float)s.Vx + Vector3.up * (float)s.Vy + _rightVector * (float)s.Vz;
            Vector3 vRel = vWorld - (_ballisticsProps.useWind ? GetWind() : Vector3.zero);

            double vMag = vRel.magnitude;
            double dragFactor = _ballisticsProps.airDensity * _ballisticsProps.dragCoefficent * _ballisticsProps.area * 0.5;
            Vector3 dragAcc = _ballisticsProps.useDrag ? -(float)(dragFactor/_ballisticsProps.mass) * (float)vMag * vRel : Vector3.zero;
            Vector3 gravity = _ballisticsProps.useGravity ? new Vector3(0, -9.81f, 0) : Vector3.zero;

            Vector3 accWorld = gravity + dragAcc;

            double ax = Vector3.Dot(accWorld, StraightVector);
            double ay = Vector3.Dot(accWorld, Vector3.up);
            double az = Vector3.Dot(accWorld, RightVector);

            return new State(
                s.Vx,
                s.Vy,
                s.Vz,
                ax,
                ay,
                az);
        }

        private double GetAngle()
        {
            if (Direction == AxisDirection.Forward)
            {
                _directionVector = transform.forward;
            }
            else if (Direction == AxisDirection.Right)
            {
                _directionVector = transform.right;
            }
            else
            {
                _directionVector = transform.up;
            }
            _straightVector = new Vector3(_directionVector.x, 0, _directionVector.z).normalized;
            _rightVector = -Vector3.Cross(_straightVector, _directionVector);
            double angle = Vector3.Angle(_directionVector, _straightVector);
            return Math.PI * angle / 180;
        }

        private Vector3 GetWind()
        {
            Vector3 main = _windZone.transform.forward;

            Vector3 turbDir = Quaternion.AngleAxis(
                UnityEngine.Random.Range(0f, 360f),
                Vector3.up                    
            ) * main;

            float windSpeed = _windZone.windMain;
            float turbulenceAbs = _windZone.windTurbulence *
                                  UnityEngine.Random.Range(-1f, 1f);

            return main * windSpeed + turbDir * turbulenceAbs;
        }
        #endregion

        #region Properties
        public AxisDirection Direction { get { return _ballisticsProps.axisDirection; } }
        public Vector3 DirectionVector { get { return _directionVector; } }
        public Vector3 StraightVector { get { return _straightVector; } }
        public Vector3 RightVector { get { return _rightVector; } }
        #endregion

    }
}
