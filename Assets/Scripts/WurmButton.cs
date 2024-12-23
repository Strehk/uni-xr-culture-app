using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WurmButton : MonoBehaviour
{
    [SerializeField] private Button mainButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button newArtObjectButton;
    [SerializeField] private Wurm artObjectScript;

    [SerializeField] public InputActionAsset inputActionAsset;

    void Start()
    {
        
        mainButton.onClick.AddListener(OnMainButtonClick);
        regenerateButton.onClick.AddListener(OnRegenerateButtonClick);
        newArtObjectButton.onClick.AddListener(OnNewWormButtonClick);

        
        regenerateButton.gameObject.SetActive(false);
        newArtObjectButton.gameObject.SetActive(false);
    }

    public void OnMainButtonClick()
    {
        
        mainButton.gameObject.SetActive(false);

        
        regenerateButton.gameObject.SetActive(true);
        newArtObjectButton.gameObject.SetActive(true);

        
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
}