
using UnityEngine;
using UnityEngine.UI;

public class SceneUI : MonoBehaviour
{
    [SerializeField] GameObject objectSettingsPage;
    [SerializeField] Button objects_Button;

    [SerializeField] Button object_Settings_Button;

    [SerializeField] Button generation_Button;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectSettingsPage.SetActive(true);
        objects_Button.gameObject.SetActive(true);
        object_Settings_Button.gameObject.SetActive(true);
        generation_Button.gameObject.SetActive(true);
        OnObjectSettingsButtonClick();
    }

    public void OnObjectsButtonClick()
    {
        if (objects_Button.interactable == true)
        {
            objects_Button.interactable = false;
            object_Settings_Button.interactable = true;
            generation_Button.interactable = true;
        }
    }
    public void OnObjectSettingsButtonClick()
    {
        if (object_Settings_Button.interactable == true)
        {
            objects_Button.interactable = true;
            object_Settings_Button.interactable = false;
            generation_Button.interactable = true;
        }
    }

    public void OnGenerationButtonClick()
    {
        if (generation_Button.interactable == true)
        {
            objects_Button.interactable = true;
            object_Settings_Button.interactable = true;
            generation_Button.interactable = false;
        }
    }
}