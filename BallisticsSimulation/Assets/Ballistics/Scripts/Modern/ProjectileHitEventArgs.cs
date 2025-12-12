using System;
using UnityEngine;

namespace BallisticsSimulation
{
    public class ProjectileHitEventArgs : EventArgs
    {
        public GameObject OtherObject { get; }
        public Collider OtherCollider { get; }
        public string Tag { get; }
        public Vector3 HitPoint { get; }
        public Vector3 ProjectilePosition { get; }
        public Vector3 Normal { get; }

        public ProjectileHitEventArgs(
            GameObject other,
            Collider col, 
            Vector3 hitPoint, 
            Vector3 normal, 
            Vector3 projectilePos
            )
        {
            OtherObject = other;
            OtherCollider = col;
            Tag = other != null ? other.tag : "";
            HitPoint = hitPoint;
            Normal = normal;
            ProjectilePosition = projectilePos;
        }
    }
}