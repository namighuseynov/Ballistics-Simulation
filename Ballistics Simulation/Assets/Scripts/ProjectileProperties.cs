using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileProperties", menuName = "ScriptableObjects/ProjectileProperties", order = 1)]
public class ProjectileProperties : ScriptableObject
{
    [Header("Ballistics")]
    public float Weight                     = 1.00f;   //kg
    public float Area                       = 0.50f;   //m^2
    public float dragCoefficient            = 0.50f;   //
    public float Density                    = 1.25f;   //
    public float StartingSpeed              = 10.0f;   //m/s

    [Header("Time Update")]
    public float deltaTime                  = 0.5f;

    [Header("Projectile")]
    public float liveTime                   = 15f;
    public ProjectileUpdateMode updateMode  = ProjectileUpdateMode.VELOCITY_CHANGE;
}
