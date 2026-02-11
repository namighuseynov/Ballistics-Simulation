using System;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace BallisticsSimulation
{
    [System.Serializable]
    public readonly struct State
    {
        #region Fields

        private readonly Vector3 _position;
        private readonly Vector3 _velocity;
        private readonly double _t;

        #endregion

        #region Constructors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public State(Vector3 pos, Vector3 vel, double t)
        {
            _position = pos;
            _velocity = vel;
            _t = t;
        }
        #endregion

        #region Properties
        public double T { get { return _t; } }
        public Vector3 Position { get { return _position; } } 
        public Vector3 Velocity { get { return _velocity; } }
        #endregion

        #region Operators
        public static State operator +(State a, State b) =>
        new State(a.Position + b.Position, a.Velocity + b.Velocity, a.T + b.T);

        public static State operator *(State a, double k) =>
            new State(a.Position * (float)k, a.Velocity * (float)k, a.T * k);
        #endregion
    }
}