using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class cube : MonoBehaviour
{
    [SerializeField] private ChatGPT chatGPT;
    [SerializeField] private string message;
    [SerializeField] private Wurm wurm;
    [SerializeField] public InputActionAsset controls;
    private Vector3 minCorner;
    private Vector3 maxCorner;


    void Start()
    {
        Vector3 center = this.transform.position;
        Vector3 extents = this.transform.localScale / 2.0f;
        Debug.Log(center);
        Debug.Log(extents);

        minCorner = center - extents;
        maxCorner = center + extents;

        var debugActionMap = controls.FindActionMap("Debug");
        var generateInputAction = debugActionMap.FindAction("Generate");
        generateInputAction.performed += Generate;

    }

    private void Generate(InputAction.CallbackContext context)
    {
        string box = "Generiere mir einen spline innerhalb folgender Begrenzung:  X-Achse: " + minCorner.x + "<x<" + maxCorner.x +
            " Y-Achse: " + minCorner.y + "<y<" + maxCorner.y + " Z-Achse:" + minCorner.z + "<z<" + maxCorner.z;
        Debug.Log(box);
        chatGPT.MessageReceived += Message;
        chatGPT.SendMessageToChatGPT(box + message);
    }


    private void Message(string message)
    {
        Debug.Log(message);
        Vector3[] list = formate(message); 

        wurm.SetNodes(list);
    }
    private Vector3[] formate(string input)
    {
        List<Vector3> result = new List<Vector3>();
        string[] lines = input.Split(';'); // Zeilen trennen
        string[] coordinates;

        for (int i = 0; i < lines.Length; i++)
        {
            try
            {
                // Debugging: Logge die aktuelle Zeile
                Debug.Log($"Processing line {i}: {lines[i]}");

                // Trenne die Koordinaten anhand von Kommas
                coordinates = lines[i].Split(',');

                // Stelle sicher, dass genau 3 Werte vorhanden sind
                if (coordinates.Length == 3)
                {
                    // Parsen der Werte unter Verwendung von CultureInfo.InvariantCulture
                    float x = float.Parse(coordinates[0].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    float y = float.Parse(coordinates[1].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    float z = float.Parse(coordinates[2].Trim(), System.Globalization.CultureInfo.InvariantCulture);

                    // Füge die Werte zur Ergebnisliste hinzu
                    result.Add(new Vector3(x, y, z));
                }
                else
                {
                    Debug.LogWarning($"Line {i} does not contain exactly 3 coordinates: {lines[i]}");
                }
            }
            catch (System.Exception ex)
            {
                // Fehler beim Parsen der aktuellen Zeile melden
                Debug.LogError($"Error processing line {i}: {lines[i]}. Exception: {ex.Message}");
            }
        }

        return result.ToArray();
    }

    private void OnDestroy() { 
    chatGPT.MessageReceived -= Message;
    }

}
