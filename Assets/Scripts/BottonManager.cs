using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Oculus.Interaction;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using System.Threading;
using System.Threading.Tasks;
using System;


public class BottonManager : MonoBehaviour
{   
    [SerializeField] private Button deleteButton;
    [SerializeField] private Toggle safe_Button;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button generate_button;
    [SerializeField] private Toggle viewNodeButton;
    [SerializeField] private Toggle exit_View_Nodes_Button;
    [SerializeField] private Toggle connect_Worms_Button;
    [SerializeField] private Toggle exit_Connect_Worms_Button; 
    [SerializeField] private ChatGPT chatGPT;

    [SerializeField] private Button gptGenerate_Button;

    [SerializeField] private Camera cam;
    
    [SerializeField] private Wurm artObjectScript;
    //[SerializeField] private Radiuslider radiuslider;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private UnityEngine.UI.Slider SmothnesSlider;

    [SerializeField] public InputActionAsset inputActionAsset;
    

    [SerializeField] private List<Wurm> worms;
    private Wurm currentWorm;
    private GameObject gptspawn;
    

    void Start()
    {   
        chatGPT.MessageReceived += MessageReceived;
        if (worms==null)
        {
            worms = new List<Wurm>();

        }
        exit_View_Nodes_Button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(true);

        Vector3 spawnpsition = cam.transform.position+cam.transform.forward*1.5f;
        gptspawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gptspawn.transform.position = spawnpsition;
        gptspawn.transform.localScale = new Vector3(1f,1f,1f);
        gptspawn.transform.parent = cam.transform;
        gptspawn.name = "GPTspawn";
        gptspawn.SetActive(false);
    }

    //darf nur von gpt aufgerufen werden 
    private void MessageReceived(string message){
        Vector3[] list = formate(message);

        Wurm newWurm = Instantiate(artObjectScript);
        newWurm.SetNodes(list);
        newWurm.SetRadius(0.05f);
        newWurm.SetColor(Color.blue);
        newWurm.CreateNodes();

        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        currentWorm = newWurm;
        worms.Add(currentWorm);
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

                    // Fuege die Werte zur Ergebnisliste hinzu
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

    public void OnGPTGenerateButtonClick(){

        Vector3 center = gptspawn.transform.position;
        Vector3 extents = gptspawn.transform.localScale / 2.0f;
        Debug.Log(center);
        Debug.Log(extents);

        Vector3 minCorner = center - extents;
        Vector3 maxCorner = center + extents;

        string box = "Generiere mir einen spline innerhalb folgender Begrenzung:  X-Achse: " + minCorner.x + "<x<" + maxCorner.x +
            " Y-Achse: " + minCorner.y + "<y<" + maxCorner.y + " Z-Achse:" + minCorner.z + "<z<" + maxCorner.z;


       String promt = "Der Spline soll möglichst dynamisch und organisch verlaufen, dabei dürfen die Grenzen der box nie überschritten werden. Der Spline soll einen Wurm darstellen. Erstelle mir eine Liste an Positionen, die die Position von Nodes des Splines darstellen.Dein Output sind die Positionen der Nodes des Splines. Beispiel Output: -2.85, 0.5, 0.325; -1.96, 0.5, 0.324; -1.15,0.817, 0.325;-0.51, 0.817, 0.90;0.135, 0.817, 1.099;0.8,1.148,1,1;1.5, 1.33, 1,1;2,1.33,0.35;2.35, 1.41, -0.3;2.35, 1.68, -1.Dein Output hat folgendes format: x,y,z;x,y,z;x,y,z. Tausche dabei x, y und z durch die jeweiligen Koordinaten aus.Der Beispiel Output muss nicht mit den Begrenzungs Parametern übereinstimmen.";

       chatGPT.SendMessageToChatGPT(box +promt);
    }
    public void Connect(string Wurms){
        string Wurm1 = Wurms.Split(";")[0];
        string Wurm2 = Wurms.Split(";")[1];

        chatGPT.SendMessageToChatGPT("Erstelle mir einen Spline der folgende zwei Splines miteinander verbinden Soll: Wurm1:"+ Wurm1+
        "Wurm2:"+ Wurm2 + "Der Spline soll nicht geschlossen sein. Der Startpunkt des einen Wurms und der Endpunkt des anderen sollen verbunden sein start punkt ist der erste wert jedes Wurms und Endpunkt der letzte. Uebernehme die Koordinaten der ohne diese zu aendern. Fuege deine Koordinaten nur zwischen wuermern ein.  Dein Output hat folgendes format: x,y,z;x,y,z;x,y,z.");
    }

    public void OnGPTREGenerateButtonClick(){

        Vector3 center = gptspawn.transform.position;
        Vector3 extents = gptspawn.transform.localScale / 2.0f;
        Debug.Log(center);
        Debug.Log(extents);

        Vector3 minCorner = center - extents;
        Vector3 maxCorner = center + extents;

        string box = "Generiere mir einen spline innerhalb folgender Begrenzung:  X-Achse: " + minCorner.x + "<x<" + maxCorner.x +
            " Y-Achse: " + minCorner.y + "<y<" + maxCorner.y + " Z-Achse:" + minCorner.z + "<z<" + maxCorner.z;


       String promt = "Der Spline soll möglichst dynamisch und organisch verlaufen, dabei dürfen die Grenzen der box nie überschritten werden. Der Spline soll einen Wurm darstellen. Erstelle mir eine Liste an Positionen, die die Position von Nodes des Splines darstellen.Dein Output sind die Positionen der Nodes des Splines. Beispiel Output: -2.85, 0.5, 0.325; -1.96, 0.5, 0.324; -1.15,0.817, 0.325;-0.51, 0.817, 0.90;0.135, 0.817, 1.099;0.8,1.148,1,1;1.5, 1.33, 1,1;2,1.33,0.35;2.35, 1.41, -0.3;2.35, 1.68, -1.Dein Output hat folgendes format: x,y,z;x,y,z;x,y,z. Tausche dabei x, y und z durch die jeweiligen Koordinaten aus.Der Beispiel Output muss nicht mit den Begrenzungs Parametern übereinstimmen.";
       
       OnDeleteButtonClick();

       chatGPT.SendMessageToChatGPT(box +promt);
    }




    public void OnRegenerateButtonClick()
    {
        if (worms.Count > 0)
        {
            //Wurm lastWorm = worms[worms.Count - 1];
            //lastWorm.OnButtonClick();
            currentWorm.OnButtonClick();
        }
    }

    public void OnNewWormButtonClick()
    {
        CreateWorm();
    }

    public void OnDeleteButtonClick()
    {
        if (worms.Count > 0 && worms.Contains(currentWorm))
        {
            int i = worms.FindIndex(x => x == currentWorm);
            Destroy(worms[i].gameObject);
            worms.RemoveAt(i);
        }
    }

     public void OnDeleteButtonClick(Wurm wurm)
    {
        if (worms.Count > 0 && worms.Contains(wurm))
        {
            int i = worms.FindIndex(x => x == wurm);
            Destroy(worms[i].gameObject);
            worms.RemoveAt(i);
        }
    }

    private void CreateWorm()
    {
        Wurm newWurm = Instantiate(artObjectScript);

        worms.Add(newWurm);
        
        newWurm.OnButtonClick();
        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        
        OnSelect(newWurm);
    }
    public void OnSelect(Wurm wurm){
        if (currentWorm != null)
        {
            if (wurm != currentWorm)
            {
                currentWorm.EnableOutline(false);
                Wurm oldWurm = currentWorm;
                if (IsConnectModeActive() == true)
                {
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    GameObject[] oldnodes = oldWurm.getNodes();
                    GameObject[] newnodes = wurm.getNodes();
                    String nodes = " ";
                    foreach (GameObject node in oldnodes)
                    {
                        nodes += node.transform.position + " ";
                    }

                    nodes += " ;";
                    foreach (GameObject node in newnodes)
                    {
                        nodes += node.transform.position + " ";
                    }

                    OnDeleteButtonClick();
                    OnDeleteButtonClick(oldWurm);
                    Connect(nodes);
                    return;
                }
                else if (AreNodesVisible() == true)
                {
                    oldWurm.ViewNodes(false);
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    slider.value = wurm.GetRadius();
                    OnViewNodeButtonClick(true);
                }
                else
                {
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    slider.value = wurm.GetRadius();
                }
            }
        }
        else
        {
            currentWorm = wurm;
            currentWorm.EnableOutline(true);
            slider.value = wurm.GetRadius();
        }
    }

    
    public void OnViewNodeButtonClick(bool view)
    {
        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(true);
        Debug.Log("OnViewNodeButtonClick");
        currentWorm.ViewNodes(view);
        
    }

    public void OnValueChanged()
    {
        if (currentWorm == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        currentWorm.SetRadius(slider.value);
    }
    
    //methode um moove/viewNode Modus zu verlassen
    //true wenn:  view nodes aktiv ist, also die nodes sichtbar sind.
    //false wenn: view nodes deaktiv ist, also die nodes nicht sichtbar sind.
    public void OnExitViewNodeButtonClick()
    {
        
            currentWorm.ViewNodes(false);
            exit_View_Nodes_Button.gameObject.SetActive(false);
            viewNodeButton.gameObject.SetActive(true);
    }

    private bool AreNodesVisible()
    {
        if (viewNodeButton.gameObject.activeSelf == true){
            return false;
        }else
        {
         return true;   
        }
        
    }

    public void ConnectMode(){
        currentWorm.ViewNodes(true);
        connect_Worms_Button.gameObject.SetActive(false);
        DeaktivateButtons();
        exit_Connect_Worms_Button.gameObject.SetActive(true);
        Debug.Log(exit_Connect_Worms_Button.gameObject.activeSelf);
    }
    public bool IsConnectModeActive()
    {
        if (connect_Worms_Button.gameObject.activeSelf == false)
        {
            return true;
        }else
        {
            return false;
        }
    }

    
    public void ExitConnectMode()
    {
        OnViewNodeButtonClick(false);
        connect_Worms_Button.gameObject.SetActive(true);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
        AktivateButtons();
    }

    public Wurm GetWurm()
    {
        return currentWorm;
    }

    private void DeaktivateButtons()
    {
        regenerateButton.gameObject.SetActive(false);
        generate_button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(false);
        safe_Button.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        SmothnesSlider.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
    }

    private void AktivateButtons()
    {
        regenerateButton.gameObject.SetActive(true);
        generate_button.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(true);
        safe_Button.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        SmothnesSlider.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
    }
}

