using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WurmButton : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button newArtObjectButton;
    [SerializeField] private Button viewNodeButton;
    
    [SerializeField] private Wurm artObjectScript;

    [SerializeField] public InputActionAsset inputActionAsset;

    void Start()
    {
        regenerateButton.gameObject.SetActive(false);
        newArtObjectButton.gameObject.SetActive(false);
        viewNodeButton.gameObject.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        mainButton.gameObject.SetActive(false);

        
        regenerateButton.gameObject.SetActive(true);
        newArtObjectButton.gameObject.SetActive(true);
        viewNodeButton.gameObject.SetActive(true);

        CreateWorm();
        artObjectScript.OnButtonClick();
    }

    public void OnRegenerateButtonClick()
    {
        artObjectScript.OnButtonClick();
    }

    public void OnNewWormButtonClick()
    {
        CreateWorm();
    }

    private void CreateWorm()
    {
        Wurm newWurm = Instantiate(artObjectScript);
        artObjectScript = newWurm;
        newWurm.OnButtonClick();
    }

    public void OnViewNodeButtonClick()
    {
        artObjectScript.MoveNodes();
    }
}