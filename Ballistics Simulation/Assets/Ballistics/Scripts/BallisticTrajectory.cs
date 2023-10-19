using UnityEngine;

public class BallisticTrajectory : MonoBehaviour
{
    private Transform projectileSpawnPoint;

    private Vector3 forwardBasis = Vector3.zero;
    private Vector3 rightBasis = Vector3.zero;
    private Vector3 upBasis = Vector3.zero;

    public void DrawBallisticTrajectory(int corners, int[] xCoords, int[] yCoords)
    {
        projectileSpawnPoint = GetComponent<WeaponController>().ProjectileSpawnPoint;
    }

    private void DrawBaseVectors()
    {
        forwardBasis = projectileSpawnPoint.position + projectileSpawnPoint.up;

        upBasis = projectileSpawnPoint.up;
        upBasis.y = 0;
        upBasis.Normalize();
        
        rightBasis = Vector3.Cross(projectileSpawnPoint.up, upBasis);
        rightBasis.Normalize();
        

        upBasis = projectileSpawnPoint.position + upBasis;
        rightBasis = projectileSpawnPoint.position + rightBasis;

        Debug.DrawLine(projectileSpawnPoint.position, forwardBasis, Color.green);
        Debug.DrawLine(projectileSpawnPoint.position, rightBasis, Color.red);
        Debug.DrawLine(projectileSpawnPoint.position, upBasis, Color.blue);
    }

    private void FixedUpdate()
    {
        DrawBaseVectors();
    }
}
