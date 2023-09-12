using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileProperties", menuName = "ScriptableObjects/ProjectileProperties", order = 1)]
public class ProjectileProperties : ScriptableObject
{
    [Header("Ballistics")]
    public float Mass = 1;
    public float Area = 0.5f;
    public float Drag = 0.5f;
    public float Density = 1.25f;
    public float startingSpeed = 10f;

    [Header("Time Update")]
    public float deltaTime = 0.5f;

    [Header("Projectile")]
    public float liveTime = 15f;
}
