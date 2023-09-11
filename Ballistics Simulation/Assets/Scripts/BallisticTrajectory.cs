using UnityEngine;

public class BallisticTrajectory : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;

    private Vector3 forwardBasis = Vector3.zero;
    private Vector3 rightBasis = Vector3.zero;
    private Vector3 upBasis = Vector3.zero;

    public void DrawBallisticTrajectory(int corners, int[] xCoords, int[] yCoords)
    {

    }

    private void DrawBasisVectors()
    {
        forwardBasis = projectileSpawnPoint.position + projectileSpawnPoint.up;
        rightBasis = projectileSpawnPoint.position + projectileSpawnPoint.right;

        upBasis = projectileSpawnPoint.up;
        upBasis.y = 0;
        upBasis.Normalize();
        upBasis = projectileSpawnPoint.position + upBasis;


        Debug.DrawLine(projectileSpawnPoint.position, forwardBasis, Color.blue);
        Debug.DrawLine(projectileSpawnPoint.position, rightBasis, Color.red);
        Debug.DrawLine(projectileSpawnPoint.position, upBasis, Color.green);
    }

    private void FixedUpdate()
    {
        DrawBasisVectors();
    }
}
