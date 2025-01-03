using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Oculus.Interaction;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BottonManager : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button newArtObjectButton;
    [SerializeField] private Button viewNodeButton;
    
    [SerializeField] private Wurm artObjectScript;
    [SerializeField] private Radiuslider radiuslider;
    [SerializeField] private UnityEngine.UI.Slider slider;

    [SerializeField] public InputActionAsset inputActionAsset;
    

    private List<Wurm> worms = new List<Wurm>();

    

    void Start()
    {   
        
        regenerateButton.gameObject.SetActive(false);
        newArtObjectButton.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
        worms.Add(artObjectScript);
    }


   public void OnMainButtonClick()
    {
        mainButton.gameObject.SetActive(false);

        
        regenerateButton.gameObject.SetActive(true);
        newArtObjectButton.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);

        CreateWorm();
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

    private void CreateWorm()
    {
        Wurm newWurm = Instantiate(artObjectScript);
        
        artObjectScript = newWurm;

        worms.Add(newWurm);
        newWurm.OnButtonClick();
        radiuslider.setWurm(newWurm);
        InteractableUnityEventWrapper eventWrapper = newWurm.GetComponentInChildren<InteractableUnityEventWrapper>();
        eventWrapper.WhenSelect.AddListener(() => OnSelect(newWurm));
        OnSelect(newWurm);
    }
    public void OnSelect(Wurm wurm){
        artObjectScript = wurm;
        slider.value = wurm.GetRadius();
    }

    
    public void OnViewNodeButtonClick(bool view)
    {
         Debug.Log("OnViewNodeButtonClick");
        artObjectScript.ViewNodes(view);
    }

    public void onValueChanged()
    {
        if (artObjectScript == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        artObjectScript.SetRadius(slider.value);
    }

    public Wurm GetWurm()
    {
        return artObjectScript;
    }
}

