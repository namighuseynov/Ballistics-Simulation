using System;
using UnityEngine;

namespace BallisticsSimulation
{
    [System.Serializable]
    public class State
    {
        #region Consturctors
        public State(double x, double y, double z, double vx, double vy, double vz)
        {
            _x = x; 
            _y = y;
            _z = z;
            _vx = vx;
            _vy = vy;
            _vz = vz;
        }
        public State(State s)
        {
            _x=s._x;
            _y=s._y;
            _z=s._z;
            _vx=s._vx;
            _vy=s._vy;
            _vz=s._vz;
        }
        #endregion

        #region Fields
        [SerializeField] private double _x;
        [SerializeField] private double _y;
        [SerializeField] private double _z;

        [SerializeField] private double _vx;
        [SerializeField] private double _vy;
        [SerializeField] private double _vz;
        #endregion

        #region Properties
        public double X { get { return _x; } }
        public double Y { get { return _y; } }
        public double Z { get { return _z; } }
        public double Vx { get { return _vx; } }
        public double Vy { get { return _vy; } }
        public double Vz { get { return _vz; } }
        #endregion

        #region Methods
        public State Add(State other)
        {
            State s = new State(this);
            s._x += other._x;
            s._y += other._y;
            s._z += other._z;
            s._vx += other._vx;
            s._vy += other._vy;
            s._vz += other._vz;
            return s;
        }

        public State Dot(double scalar)
        {
            State s = new State(this);
            s._x *= scalar;
            s._y *= scalar;
            s._z *= scalar;

            s._vx *= scalar;
            s._vy *= scalar;
            s._vz *= scalar;
            return s;
        }

        public State Sub(State o) =>
            new State(X - o.X, Y - o.Y, Z - o.Z, Vx - o.Vx, Vy - o.Vy, Vz - o.Vz);

        public double Magnitude() =>
            Math.Sqrt(X * X + Y * Y + Z * Z + Vx * Vx + Vy * Vy + Vz * Vz);

        #endregion
    }
}