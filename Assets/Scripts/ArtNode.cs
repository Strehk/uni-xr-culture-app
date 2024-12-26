using UnityEngine;

public class ArtNode : MonoBehaviour
{
    [SerializeField] [HideInInspector] private Vector3 position;
    
    public void SetPosition(Vector3 node)
    {
        position = node;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    private void Awake()
    {
        
    }
}