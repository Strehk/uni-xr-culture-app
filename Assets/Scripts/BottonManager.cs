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
    private Wurm currentWorm;

    

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

    private void CreateWorm()
    {
        Wurm newWurm = Instantiate(artObjectScript);
        currentWorm = newWurm;

        worms.Add(newWurm);
        
        newWurm.OnButtonClick();
        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        
        OnSelect(newWurm);
    }
    public void OnSelect(Wurm wurm){
        Wurm oldWurm = currentWorm;
        if (IsConnectModeActive() == true)
        {
            currentWorm = wurm;
            slider.value = wurm.GetRadius();
            currentWorm.ViewNodes(true);	
            
            return;
        } else if (AreNodesVisible() == true)
        {   
            oldWurm.ViewNodes(false);
            currentWorm = wurm;
            slider.value = wurm.GetRadius();
            OnViewNodeButtonClick(true);
        }else
        {
            currentWorm = wurm;
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

