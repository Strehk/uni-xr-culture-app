using UnityEngine;

public class Colorchanger : MonoBehaviour
{
    public GameObject targetObject; // Das Objekt, dessen Farbe geändert wird
    private Renderer targetRenderer;

    void Start()
    {
        // Den Renderer des Zielobjekts abrufen
        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
        }
        else
        {
            Debug.LogError("Kein Zielobjekt zugewiesen!");
        }
    }

    public void ChangeColor()
    {
        Debug.LogError("Gedrueckt");
    }
}