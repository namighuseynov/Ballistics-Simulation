using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject ProjectileObject;
    public Transform ProjectileSpawnPoint;


    public BallisticSettings ballisticSettings;

    
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

    
}