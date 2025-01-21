using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Oculus.Interaction;
using System.Collections.Generic;
using System;

public class BottonManager : MonoBehaviour
{   
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button generate_button;
    [SerializeField] private Toggle viewNodeButton;
    [SerializeField] private Toggle exit_View_Nodes_Button;
    [SerializeField] private Toggle connect_Worms_Button;
    [SerializeField] private Toggle exit_Connect_Worms_Button; 
    [SerializeField] private ChatGPT chatGPT;

    [SerializeField] private Button gptGenerate_Button;
    [SerializeField] private Button gpt_ReGen_Button;

    [SerializeField] private Toggle start_draw_worm_Button;
    [SerializeField] private Toggle end_draw_worm_Button;

    [SerializeField] private Camera cam;
    
    [SerializeField] private Wurm artObjectScript;
    //[SerializeField] private Radiuslider radiuslider;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private UnityEngine.UI.Slider SmothnesSlider;

    [SerializeField] private Toggle changeColorButton;

    [SerializeField] private Toggle changeapperance_Button;

    [SerializeField] private GameObject apperance_Panel;

    [SerializeField] private Colorchanger ColorPanel;
    private bool color_panelActive;

    [SerializeField] public InputActionAsset inputActionAsset;
    

    [SerializeField] private List<Wurm> worms;
    
    private bool changeapperance_Button_state;

    private Wurm currentWorm;
    private GameObject gptspawn;
    

    void Start()
    {   
        
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;
        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;
        chatGPT.MessageReceived += MessageReceived;

        currently_posseble_operations();

        if (worms==null)
        {
            worms = new List<Wurm>();

        }
        exit_View_Nodes_Button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(true);

        Vector3 spawnpsition = cam.transform.position+cam.transform.forward*1.5f;
        gptspawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gptspawn.transform.position = spawnpsition;
        gptspawn.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        gptspawn.transform.parent = cam.transform;
        gptspawn.name = "GPTspawn";
        gptspawn.SetActive(false);
    }

    public void onDrawmodebuttonClick()
    {
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        

        start_draw_worm_Button.gameObject.SetActive(false);
        DeaktivateButtons();
        slider.gameObject.SetActive(true);
        SmothnesSlider.gameObject.SetActive(true);
        changeapperance_Button.gameObject.SetActive(true);
        changeColorButton.gameObject.SetActive(true);
        Wurm wurm = Instantiate(artObjectScript);
        OnSelect(wurm);
        end_draw_worm_Button.gameObject.SetActive(true);
        wurm.NodePlacementMode(true);

        worms.Add(wurm);

        InteractableUnityEventWrapper eventWrapper = wurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(wurm));
        
    }

    public void PlaceNode(GameObject hand)
    {
        if (isDrawModeActive())
            currentWorm.PlaceNode(hand);
    }
    
    public void onEndDrawmodebuttonClick(){
        currentWorm.NodePlacementMode(false);
        AktivateButtons();
        end_draw_worm_Button.gameObject.SetActive(false);
    }

    
    public bool isDrawModeActive () {
        if (end_draw_worm_Button.gameObject.activeSelf == true){
            return true;
        }else return false;
    }

    

    //darf nur von gpt aufgerufen werden 
    private void MessageReceived(string message){
        string ChatgbtResponse;
        string Response;
        if(message.Contains(">")){
            Debug.Log("Connection Mode__");
            ChatgbtResponse = message.Split(">")[0];
            string Worms = message.Split(">")[1];
            string wurm1 = Worms.Split(";")[0];
            string wurm2 = Worms.Split(";")[1];
            string Wurm1 = "";
            string Wurm2 = "";
            string[] wurm1array = wurm1.Split(")");
            for (int i = 0; i < wurm1array.Length; i++)
            {
                wurm1array[i] = wurm1array[i].Replace("(", "").Replace(" ", "").Replace(";", "");
                Wurm1 += wurm1array[i];
                if(i!= wurm1array.Length-2){
                    Wurm1 += ";";
                }
            }
            string[] wurm2array = wurm2.Split(")");
            for (int i = 0; i < wurm2array.Length; i++)
            {
               wurm2array[i] = wurm2array[i].Replace("(", "").Replace(" ", "").Replace(";","");
               Wurm2 += wurm2array[i];
               if(i!= wurm2array.Length-2){
                    Wurm2 += ";";
               }

            }
            Wurm2 = Wurm2.Remove(Wurm2.Length -1);


            Debug.Log("Worms:"+Wurm1 + Wurm2);
            Worms = Wurm1+Wurm2;

            string[] wormsArray = Worms.Split(';');


            foreach (string worm in wormsArray)
            {
             ChatgbtResponse = ChatgbtResponse.Replace(worm, "");

            }
            Response = Wurm1+ChatgbtResponse+Wurm2;
            Debug.Log("Response:"+Response);

        }else{
            Response = message;
        }


        Vector3[] list = formate(Response);

        Wurm newWurm = Instantiate(artObjectScript);
        newWurm.SetNodes(list);
        newWurm.SetRadius(0.05f);
        newWurm.SetRandomColor();
        newWurm.CreateNodes();

        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        currentWorm = newWurm;
        worms.Add(currentWorm);
        currently_posseble_operations();
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

        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        Vector3 center = gptspawn.transform.position;
        Vector3 extents = gptspawn.transform.localScale / 2.0f;
        Debug.Log(center);
        Debug.Log(extents);

        Vector3 minCorner = center - extents;
        Vector3 maxCorner = center + extents;

        string box = "Generiere mir einen spline innerhalb folgender Begrenzung:  X-Achse: " + minCorner.x + "<x<" + maxCorner.x +
            " Y-Achse: " + minCorner.y + "<y<" + maxCorner.y + " Z-Achse:" + minCorner.z + "<z<" + maxCorner.z;


       String promt = "Der Spline soll möglichst dynamisch und organisch verlaufen, dabei dürfen die Grenzen nie überschritten werden. Der Spline soll einen Wurm darstellen, dennoch soll der Spline nicht linear verlaufen, sondern eine große Vielfalt darstellen. Erstelle mir eine Liste an Positionen, die die Position von Nodes des Splines darstellen.Dein Output sind die Positionen der Nodes des Splines. Gib mindestens 8 Positionen an. Des weiteren soll der Wurm möglichst komplex sein, das heißt , dass er möglichst viele Kurven hat. Der Spline soll mindestens zwei kurven haben.Beispiel Output: -2.85, 0.5, 0.325; -1.96, 0.5, 0.324; -1.15,0.817, 0.325;-0.51, 0.817, 0.90;0.135, 0.817, 1.099;0.8,1.148,1,1;1.5, 1.33, 1,1;2,1.33,0.35;2.35, 1.41, -0.3;2.35, 1.68, -1.Dein Output hat folgendes format: x,y,z;x,y,z;x,y,z. Tausche dabei x, y und z durch die jeweiligen Koordinaten aus.Der Beispiel Output muss nicht mit den Begrenzungs Parametern übereinstimmen, dein Output jedoch schon.";

       chatGPT.SendMessageToChatGPT(box +promt);
    }
    public void Connect(string Wurms){
        string Wurm1 = Wurms.Split(";")[0];
        string Wurm2 = Wurms.Split(";")[1];
        string Worms = ">"+Wurm1+";"+Wurm2;
        Debug.Log("Worms:"+Wurm1+Wurm2);

        chatGPT.SendMessageToChatGPT("Erstelle mir einen Spline der folgende zwei Splines miteinander verbinden Soll: Wurm1:"+ Wurm1+
        "Wurm2:"+ Wurm2 + "Der Spline soll nicht geschlossen sein. Der Startpunkt des einen Wurms und der Endpunkt des anderen sollen verbunden sein. Die Verbindung hat mindestens 8 Positionen und verläuft dynamisch. Startpunkt ist der erste Wert jedes Wurms und der Endpunkt der letzte. Du darfst nur Koordinaten zwischen dem Startpunkt und Endpunkt hinzufügen.Dein Output beinhaltet nur die Verbindung zwischen den Würmern.  Dein Output hat folgendes Format: x,y,z;x,y,z;x,y,z.", Worms);
    }

    public void OnGPTREGenerateButtonClick(){
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

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
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        if (worms.Count > 0)
        {
            //Wurm lastWorm = worms[worms.Count - 1];
            //lastWorm.OnButtonClick();
            currentWorm.OnButtonClick();
        }
    }

    public void OnNewWormButtonClick()
    {   
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        CreateWorm();
    }

    public void OnDeleteButtonClick()
    {   
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        if (worms.Count > 0 && worms.Contains(currentWorm))
        {
            int i = worms.FindIndex(x => x == currentWorm);
            Destroy(worms[i].gameObject);
            worms.RemoveAt(i);
            currentWorm = null;
            currently_posseble_operations();
        }
    }

     public void OnDeleteButtonClick(Wurm wurm)
    {  
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        if (worms.Count > 0 && worms.Contains(wurm))
        {
            int i = worms.FindIndex(x => x == wurm);
            Destroy(worms[i].gameObject);
            worms.RemoveAt(i);
            if(currentWorm == wurm){currentWorm = null;}
            currently_posseble_operations();
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
    public void OnSelect(Wurm wurm)
    {
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        if (currentWorm != null)
        {
            if (wurm != currentWorm)
            {
                currentWorm.EnableOutline(false);
                SmothnesSlider.value = currentWorm.GetInstanceSpace();
                Wurm oldWurm = currentWorm;
                if (IsConnectModeActive() == true)
                {
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    SmothnesSlider.value = currentWorm.GetInstanceSpace();
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
                    SmothnesSlider.value = currentWorm.GetInstanceSpace();
                    OnViewNodeButtonClick(true);

                } else if (isDrawModeActive()==true)
                {   
                    Debug.Log("Drwamode vor end");
                    onEndDrawmodebuttonClick();
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    slider.value = wurm.GetRadius();
                    SmothnesSlider.value = currentWorm.GetInstanceSpace();
                    Debug.Log("Drwamode nach end");
                    return;
                }
                else
                {
                    currentWorm = wurm;
                    currentWorm.EnableOutline(true);
                    slider.value = wurm.GetRadius();
                    SmothnesSlider.value = currentWorm.GetInstanceSpace();
                    currently_posseble_operations();
                    Debug.Log("weder drawmode noch connectmode noch viewmode aber curent wurm !=0");
                }
            }else if (isDrawModeActive() == false && IsConnectModeActive() == false && AreNodesVisible() == false)
            {
                currentWorm.EnableOutline(false);
                SmothnesSlider.value = currentWorm.GetInstanceSpace();
                currentWorm = null;
                currently_posseble_operations();
                Debug.Log("wurm == current wurm");
            }
        }
        else
        {   

            currentWorm = wurm;
            currentWorm.EnableOutline(true);
            SmothnesSlider.value = currentWorm.GetInstanceSpace();
            slider.value = wurm.GetRadius();
            currently_posseble_operations();
            Debug.Log("current wurm == null");
        }
    }

    public void OnColoreButtonClick(){
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        color_panelActive = !color_panelActive;
        ColorPanel.gameObject.SetActive(color_panelActive);
    }

    
    public void OnViewNodeButtonClick(bool view)
    {   
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(true);
        Debug.Log("OnViewNodeButtonClick");
        currentWorm.ViewNodes(view);
        
    }

    //radiusslider-Methode
    public void OnValueChanged()
    {
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        if (currentWorm == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        currentWorm.SetRadius(slider.value);
        if (AreNodesVisible() == true)
        {
            currentWorm.ViewNodes(false);
            currentWorm.ViewNodes(true);
        }
    }

    public void OnItemsChanged()
    {
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        if (currentWorm == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        if (currentWorm.GetInstanceCount() <= 0)
        {
            return;
        }

        currentWorm.SetSpacing(SmothnesSlider.value);


    }
    
    //methode um moove/viewNode Modus zu verlassen
    //true wenn:  view nodes aktiv ist, also die nodes sichtbar sind.
    //false wenn: view nodes deaktiv ist, also die nodes nicht sichtbar sind.
    public void OnExitViewNodeButtonClick()
    {
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        currentWorm.ViewNodes(false);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(true);
    }

    private bool AreNodesVisible()
    {
        if (exit_View_Nodes_Button.gameObject.activeSelf == true){
            return true;
        }else
        {
         return false;   
        }
        
    }

    public void ConnectMode(){
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;

        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;


        currentWorm.ViewNodes(true);
        connect_Worms_Button.gameObject.SetActive(false);
        DeaktivateButtons();
        exit_Connect_Worms_Button.gameObject.SetActive(true);
        Debug.Log(exit_Connect_Worms_Button.gameObject.activeSelf);
    }
    public bool IsConnectModeActive()
    {
        if (exit_Connect_Worms_Button.gameObject.activeSelf == true)
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

    private bool Connect_possible()
    {
        if (currentWorm != null && worms.Count > 1)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void currently_posseble_operations()
    {
        if (currentWorm != null)
        {
            regenerateButton.interactable = true;
            gpt_ReGen_Button.interactable = true;
            viewNodeButton.interactable = true;
            slider.gameObject.SetActive(true);
            SmothnesSlider.gameObject.SetActive(true);
            connect_Worms_Button.interactable = Connect_possible();
            changeapperance_Button.interactable = true;
            changeColorButton.interactable = true;
            deleteButton.interactable = true;
        }
        else
        {
            regenerateButton.interactable = false;
            gpt_ReGen_Button.interactable = false;
            viewNodeButton.interactable = false;
            slider.gameObject.SetActive(false);
            SmothnesSlider.gameObject.SetActive(false);
            connect_Worms_Button.interactable = false;
            changeapperance_Button.interactable = false;
            changeColorButton.interactable = false;
            deleteButton.interactable = false;
        }
    }

    private void DeaktivateButtons()
    {
        regenerateButton.gameObject.SetActive(false);
        generate_button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        gpt_ReGen_Button.gameObject.SetActive(false);
        gptGenerate_Button.gameObject.SetActive(false);
        start_draw_worm_Button.gameObject.SetActive(false);
        end_draw_worm_Button.gameObject.SetActive(false);
        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;
        apperance_Panel.gameObject.SetActive(false);
        changeapperance_Button_state = false;	
    }

    private void AktivateButtons()
    {
        regenerateButton.gameObject.SetActive(true);
        generate_button.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        SmothnesSlider.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
        gpt_ReGen_Button.gameObject.SetActive(true);
        gptGenerate_Button.gameObject.SetActive(true);
        start_draw_worm_Button.gameObject.SetActive(true);
        end_draw_worm_Button.gameObject.SetActive(false);
        currently_posseble_operations();
    }

    public void on_Change_Apperance_Button_click(){
        ColorPanel.gameObject.SetActive(false);
        color_panelActive = false;

        changeapperance_Button_state = !changeapperance_Button_state;
        apperance_Panel.SetActive(changeapperance_Button_state);
    }


    ///
    ///
    ///Florans Space
    ///
    ///
    //nach moeglichkeit bitte keiene wuermer loeschen. Wenn loeschen eiziger weg, bitte onDeleteButtonClick() aufrufen und neuen Wurm zu worms[] hinzufuegen!!!
    public void onSphere_buttonClick(GameObject artObject)
    {
        currentWorm.AddInstantiateObject(artObject);
        Debug.Log("OnSphere_buttonClick");
    }
    public void onCylinder_buttonClick(GameObject artObject)
    {
        currentWorm.AddInstantiateObject(artObject);
        Debug.Log("OnCylinder_buttonClick");
    }

    public void onWormTexture_buttonClick(GameObject artObject)
    {
        currentWorm.RemoveInstantiateObjects();
        Debug.Log("OnWormTexture_buttonClick");
    }
}
