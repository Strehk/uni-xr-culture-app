using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WurmButton : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button newArtObjectButton;
    [SerializeField] private Button viewNodeButton;
    
    [SerializeField] private Button newOwnWurmButton;
    
    [SerializeField] private Wurm artObjectScript;

    [SerializeField] [HideInInspector] private Wurm newWurm;

    [SerializeField] public InputActionAsset inputActionAsset;

    void Start()
    {
        regenerateButton.gameObject.SetActive(false);
        newArtObjectButton.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
        newOwnWurmButton.gameObject.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        Debug.Log("OnMainButtonClick");
        mainButton.gameObject.SetActive(false);

        
        regenerateButton.gameObject.SetActive(true);
        newArtObjectButton.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);
        newOwnWurmButton.gameObject.SetActive(true);

        CreateWorm();
        newWurm.OnButtonClick();
    }

    public void OnRegenerateButtonClick()
    {
        Debug.Log("OnRegenerateButtonClick");
        newWurm.OnButtonClick();
    }

    public void OnNewWormButtonClick()
    {
        newWurm.NodePlacementMode(false);
        Debug.Log("OnNewWormButtonClick");
        CreateWorm();
        newWurm.OnButtonClick();
    }

    public void OnNewOwnWurmButtonClick()
    {
        newWurm.NodePlacementMode(true);
    }

    private void CreateWorm()
    {
        newWurm = Instantiate(artObjectScript);
    }

    public void OnViewNodeButtonClick()
    {
        Debug.Log("OnViewNodeButtonClick");
        newWurm.ViewNodes(true);
    }
}