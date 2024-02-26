using Unity.VisualScripting;
using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;

    public GameObject GetLaserPrefab()
    {
        return laserPrefab;
    }
}
