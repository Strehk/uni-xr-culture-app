using UnityEngine;

public class FollowerCanvas : MonoBehaviour
{
    public RectTransform WurmUI_Panel;
    private Vector3 initialOffset; 

    void Start()
    {
        if (WurmUI_Panel != null)
        {
            initialOffset = WurmUI_Panel.InverseTransformPoint(transform.position);
        }
        else
        {
            Debug.LogError("Canvas A ist nicht zugewiesen!");
        }
    }

    void Update()
    {
        if (WurmUI_Panel != null)
        {
            transform.position = WurmUI_Panel.TransformPoint(initialOffset);
            transform.rotation = WurmUI_Panel.rotation;
        }
    }

}
