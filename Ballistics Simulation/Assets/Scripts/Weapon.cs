using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject ProjectileObject;
    public Transform ProjectileSpawnPoint;

    private Vector3 forwardVector;
    private Vector3 rightVector;
    private Vector3 upVector;

    private void FixedUpdate()
    {
        forwardVector = ProjectileSpawnPoint.position + ProjectileSpawnPoint.up;
        rightVector = ProjectileSpawnPoint.position + ProjectileSpawnPoint.right;

        upVector = ProjectileSpawnPoint.up;
        upVector.y = 0;
        upVector.Normalize();
        upVector = ProjectileSpawnPoint.position + upVector;

        


        Debug.DrawLine(ProjectileSpawnPoint.position, forwardVector, Color.blue);
        Debug.DrawLine(ProjectileSpawnPoint.position, rightVector, Color.red);
        Debug.DrawLine(ProjectileSpawnPoint.position, upVector, Color.green);
    }
    public void Shot()
    {
        GameObject Projectile = Instantiate(ProjectileObject, ProjectileSpawnPoint);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shot();
        }
    }

    private void DrawBasisVectors()
    {

    }
}