using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileProperties", menuName = "ScriptableObjects/ProjectileProperties", order = 1)]
public class ProjectileProperties : ScriptableObject
{
    [Header("Ballistics")]
    public float Mass;
    public float Area;
    public float Drag;
    public float Density;
}
