using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class SphereSpawner : MonoBehaviour
{
    public GameObject spherePrefab; // Prefab für die Kugeln
    
    public void Spawn(List<Vector3> Input)
    {
        if (spherePrefab == null)
        {
            Debug.LogError("spherePrefab is not assigned!");
            return;
        }
        for (int i = 0; i < Input.Count; i++)
        {
            GameObject sphere = Instantiate(spherePrefab, Input[i], Quaternion.identity);
        }
        
       
    }
}