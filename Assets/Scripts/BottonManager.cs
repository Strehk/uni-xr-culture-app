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
            Wurm lastWorm = worms[worms.Count - 1];
            lastWorm.OnButtonClick();
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
        artObjectScript = newWurm;
    }
    public void OnSelect(Wurm wurm){
        radiuslider.setWurm(wurm);
    }

    
    public void OnViewNodeButtonClick()
    {
        artObjectScript.MoveNodes();
    }

    public Wurm GetWurm()
    {
        return artObjectScript;
    }
}

