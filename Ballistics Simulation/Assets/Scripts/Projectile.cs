using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/CustomProjectile", order = 1)]
public class Projectile : ScriptableObject
{
    [Header("Ballistics")]
    public float Mass;
    public float Area;
    public float Drag;
    public float Density;


}
