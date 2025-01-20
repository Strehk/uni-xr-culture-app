#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class cube : MonoBehaviour
{
    [SerializeField] private ChatGPT chatGPT;
    [SerializeField] private string messageGenerate;
    [SerializeField] private Wurm wurm;
    [SerializeField] public InputActionAsset controls;
    private Wurm newWurm;

    private Vector3 minCorner;
    private Vector3 maxCorner;


    void Start()
    {
        chatGPT.MessageReceived += Message;


        Vector3 center = this.transform.position;
        Vector3 extents = this.transform.localScale / 2.0f;
        Debug.Log(center);
        Debug.Log(extents);

        minCorner = center - extents;
        maxCorner = center + extents;

        var debugActionMap = controls.FindActionMap("Debug");
        var generateInputAction = debugActionMap.FindAction("Generate");
        generateInputAction.performed += Generate;
        Vector3[] Debugwurm1 = {new Vector3(1.40f, 1.33f, -1.71f),
                                    new Vector3(2.54f, 1.50f, -0.94f),
                                    new Vector3(3.67f, 1.68f, -1.17f),
                                    new Vector3(4.80f, 1.86f, -0.39f)};

        Vector3[] Debugwurm2 = new Vector3[]
        {
            new Vector3(-2.46f, 1.33f, -1.71f),
            new Vector3(-2.06f, 1.33f, -0.90f),
            new Vector3(-1.66f, 1.33f, -0.09f),
            new Vector3(-1.26f, 1.50f, 0.72f),
            new Vector3(-0.86f, 1.50f, 1.52f),
            new Vector3(-0.46f, 1.50f, 0.99f)
        };
        Vector3[] Debugwurm3 = new Vector3[]
        {
            new Vector3(-2.46f, 1.33f, -1.71f),
                new Vector3(-2.06f, 1.33f, -0.90f),
                new Vector3(-1.66f, 1.33f, -0.09f),
                new Vector3(-1.26f, 1.50f, 0.72f),
                new Vector3(-0.86f, 1.50f, 1.52f),
                new Vector3(-0.46f, 1.50f, 0.99f),
                new Vector3(0.0f, 1.50f, 0.0f),
                new Vector3(1.40f, 1.33f, -1.71f),
                new Vector3(2.54f, 1.50f, -0.94f),
                new Vector3(3.67f, 1.68f, -1.17f),
                new Vector3(4.80f, 1.86f, -0.39f)
        };
        /*
        newWurm = Instantiate(wurm);
        newWurm.SetNodes(Debugwurm1);
        newWurm.SetRadius(0.05f);
        newWurm.SetColor(Color.blue);

        newWurm2 = Instantiate(wurm);
        newWurm2.SetNodes(Debugwurm2);
        newWurm2.SetRadius(0.05f);
        newWurm2.SetColor(Color.red);

        newWurm3 = Instantiate(wurm);
        newWurm3.SetNodes(Debugwurm3);
        newWurm3.SetRadius(0.05f);
        newWurm3.SetColor(Color.green);
        */

    }

    private void Generate(InputAction.CallbackContext context)
    {
        string box = "Generiere mir einen spline innerhalb folgender Begrenzung:  X-Achse: " + minCorner.x + "<x<" + maxCorner.x +
            " Y-Achse: " + minCorner.y + "<y<" + maxCorner.y + " Z-Achse:" + minCorner.z + "<z<" + maxCorner.z;
        Debug.Log(box);

        chatGPT.SendMessageToChatGPT(box + messageGenerate);
    }

    public void Connect(string Wurms){
        string Wurm1 = Wurms.Split(";")[0];
        string Wurm2 = Wurms.Split(";")[1];

        chatGPT.SendMessageToChatGPT("Erstelle mir einen Spline der folgende zwei Splines miteinander verbinden Soll: Wurm1:"+ Wurm1+
        "Wurm2:"+ Wurm2 + "Der Spline soll nicht geschlossen sein. Der Startpunkt des einen Wurms und der Endpunkt des anderen sollen verbunden sein.  Dein Output hat folgendes format: x,y,z;x,y,z;x,y,z.");
    }


    private void Message(string message)
    {
        Debug.Log(message);
        Vector3[] list = formate(message); 

        newWurm = Instantiate(wurm);
        newWurm.SetNodes(list);
        newWurm.SetRadius(0.05f);
        newWurm.SetColor(Color.blue);

        Vector3[] output = wurm.GetNodes();
        string result = "Vector3 Array: ";
        foreach (Vector3 v in output)
        {
            result += v + " ";
        }

        Debug.Log(result);

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

                    // F�ge die Werte zur Ergebnisliste hinzu
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
# endif