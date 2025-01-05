using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Oculus.Interaction;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using System.Threading;
using System.Threading.Tasks;

public class BottonManager : MonoBehaviour
{   
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button regenerate_settinngs_Button;
    [SerializeField] private Button generate_settings_Button;
    [SerializeField] private Toggle safe_Button;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button generate_button;
    [SerializeField] private Toggle viewNodeButton;
    [SerializeField] private Toggle exit_View_Nodes_Button;
    [SerializeField] private Toggle connect_Worms_Button;
    [SerializeField] private Toggle exit_Connect_Worms_Button; 
    
    [SerializeField] private Wurm artObjectScript;
    //[SerializeField] private Radiuslider radiuslider;
    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private UnityEngine.UI.Slider SmothnesSlider;

    [SerializeField] public InputActionAsset inputActionAsset;
    

    [SerializeField] private List<Wurm> worms;

    

    void Start()
    {   
        if (worms==null)
        {
            worms = new List<Wurm>();
        }
        exit_View_Nodes_Button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(true);
    }

    public void OnRegenerateButtonClick()
    {
        if (worms.Count > 0)
        {
            //Wurm lastWorm = worms[worms.Count - 1];
            //lastWorm.OnButtonClick();
            artObjectScript.OnButtonClick();
        }
    }

    public void OnNewWormButtonClick()
    {
        CreateWorm();
    }

    public void OnDeleteButtonClick()
    {
        if (worms.Count > 0 && worms.Contains(artObjectScript))
        {
            int i = worms.FindIndex(x => x == artObjectScript);
            Destroy(worms[i].gameObject);
            worms.RemoveAt(i);
        }
    }

    private void CreateWorm()
    {
        Wurm newWurm = Instantiate(artObjectScript);

        artObjectScript = newWurm;

        worms.Add(newWurm);
        
        newWurm.OnButtonClick();
        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        
        OnSelect(newWurm);
    }
    public void OnSelect(Wurm wurm){
        Wurm oldWurm = artObjectScript;
        if (IsConnectModeActive() == true)
        {
            artObjectScript = wurm;
            slider.value = wurm.GetRadius();
            artObjectScript.ViewNodes(true);	
            
            return;
        } else if (AreNodesVisible() == true)
        {   
            oldWurm.ViewNodes(false);
            artObjectScript = wurm;
            slider.value = wurm.GetRadius();
            OnViewNodeButtonClick(true);
        }else
        {
            artObjectScript = wurm;
            slider.value = wurm.GetRadius();
        }
        
    }

    
    public void OnViewNodeButtonClick(bool view)
    {
        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(true);
        Debug.Log("OnViewNodeButtonClick");
        artObjectScript.ViewNodes(view);
        
    }

    public void OnValueChanged()
    {
        if (artObjectScript == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        artObjectScript.SetRadius(slider.value);
    }
    
    //methode um moove/viewNode Modus zu verlassen
    //true wenn:  view nodes aktiv ist, also die nodes sichtbar sind.
    //false wenn: view nodes deaktiv ist, also die nodes nicht sichtbar sind.
    public void OnExitViewNodeButtonClick()
    {
        
            artObjectScript.ViewNodes(false);
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
        artObjectScript.ViewNodes(true);
        connect_Worms_Button.gameObject.SetActive(false);
        DeaktivateButtons();
        exit_Connect_Worms_Button.gameObject.SetActive(true);
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
        return artObjectScript;
    }

    private void DeaktivateButtons()
    {
        regenerateButton.gameObject.SetActive(false);
        generate_button.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(false);
        generate_settings_Button.gameObject.SetActive(false);
        safe_Button.gameObject.SetActive(false);
        slider.gameObject.SetActive(false);
        SmothnesSlider.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        regenerate_settinngs_Button.gameObject.SetActive(false);
    }

    private void AktivateButtons()
    {
        regenerateButton.gameObject.SetActive(true);
        generate_button.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);
        exit_View_Nodes_Button.gameObject.SetActive(false);
        connect_Worms_Button.gameObject.SetActive(true);
        generate_settings_Button.gameObject.SetActive(true);
        safe_Button.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);
        SmothnesSlider.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(true);
        regenerate_settinngs_Button.gameObject.SetActive(true);
        exit_Connect_Worms_Button.gameObject.SetActive(false);
    }
}

