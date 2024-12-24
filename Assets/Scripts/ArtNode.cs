using System;
using UnityEngine;
using UnityEngine.Splines;

public class ArtNode : MonoBehaviour
{
    [SerializeField] [HideInInspector] private Vector3 position;
    
    public void SetPosition(Vector3 node)
    {
        position = node;
    }

    private void Awake()
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(gameObject.transform, false);
        
    }
}