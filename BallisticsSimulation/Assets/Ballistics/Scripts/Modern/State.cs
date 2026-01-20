using System;
using System.Runtime.CompilerServices;

namespace BallisticsSimulation
{
    [System.Serializable]
    public readonly struct State
    {
        #region Fields

        private readonly double _x, _y, _z;
        private readonly double _vx, _vy, _vz;
        private readonly double _t;

        #endregion

        #region Constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State(double x, double y, double z, double vx, double vy, double vz, double t)
        {
            _x = x; _y = y; _z = z;
            _vx = vx; _vy = vy; _vz = vz;
            _t = t;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State(State s) : this(s._x, s._y, s._z, s._vx, s._vy, s._vz, s._t) { }

        #endregion

        #region Properties
        public double X { get { return _x; } }
        public double Y { get { return _y; } }
        public double Z { get { return _z; } }
        public double Vx { get { return _vx; } }
        public double Vy { get { return _vy; } }
        public double Vz { get { return _vz; } }
        public double T { get { return _t; } }
        #endregion

        #region Methods
        public State Add(in State other) =>
            new State(
                _x + other.X,
                _y + other.Y,
                _z + other.Z,
                _vx + other.Vx,
                _vy + other.Vy,
                _vz + other.Vz,
                _t + other.T
            );
        public State Dot(in double scalar) =>
            new State(
                _x*scalar,
                _y*scalar,
                _z*scalar,
                _vx*scalar,
                _vy*scalar,
                _vz*scalar,
                _t*scalar
            );
        public State Sub(in State o) =>
            new State(X - o.X, Y - o.Y, Z - o.Z, Vx - o.Vx, Vy - o.Vy, Vz - o.Vz, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Magnitude() =>
            Math.Sqrt(X * X + Y * Y + Z * Z + Vx * Vx + Vy * Vy + Vz * Vz);

        #endregion

        #region Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State operator +(in State a, in State b) => new(
            a._x + b._x, a._y + b._y, a._z + b._z,
            a._vx + b._vx, a._vy + b._vy, a._vz + b._vz,
            a._t + b._t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static State operator *(in State a, double k) => new(
            a._x * k, a._y * k, a._z * k,
            a._vx * k, a._vy * k, a._vz * k,
            a._t * k);
        #endregion
    }
}