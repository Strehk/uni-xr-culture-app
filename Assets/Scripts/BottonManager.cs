using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BottonManager : MonoBehaviour
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
        //viewNodeButton.gameObject.SetActive(true);

        
        artObjectScript.OnButtonClick();
    }

    public void OnRegenerateButtonClick()
    {
        artObjectScript.OnButtonClick();
    }

    public void OnNewWormButtonClick()
    {
        // Create a new Wurm instance
        GameObject newWurmObject = new GameObject();
        Wurm newWurm = Instantiate(artObjectScript, newWurmObject.transform);
        artObjectScript = newWurm;
        newWurm.OnButtonClick();
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

