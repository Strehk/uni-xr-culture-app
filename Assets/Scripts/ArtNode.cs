using System;
using UnityEngine;
using UnityEngine.Splines;

public class ArtNode : MonoBehaviour
{
    [SerializeField] [HideInInspector] public Vector3 position;
    [SerializeField] [HideInInspector] private int index;
    [SerializeField] [HideInInspector] private Color color;

    public void Start()
    {
        color = GetComponent<MeshRenderer>().material.color;
    }

    public void SetPosition(Vector3 node)
    {
        position = node;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public void OnHover()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void OnUnHover()
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    [SerializeField] [HideInInspector] private float timer = 0f;
    
    private void Update()
    {
        if (timer <= 0.5f)
            timer += Time.deltaTime;
        if (transform.hasChanged && timer > 0.5f)
        {
            var wurm = GetComponentInParent<Wurm>();
            wurm.OnNodeChanged(this);
            transform.hasChanged = false;
        }
    }
}