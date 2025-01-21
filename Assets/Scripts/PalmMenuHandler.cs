using System.Collections;
using UnityEngine;

public class PalmMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject ui_panel;
    [SerializeField] private GameObject ContainerObject;
    [SerializeField] private GameObject hide_icon;
    [SerializeField] private GameObject text_hide;

    [SerializeField] private GameObject show_icon;
    [SerializeField] private GameObject text_show;
    

    [SerializeField] private Camera cam;

    private bool hidingEnabled;

    private Vector3 camPos;


    void Start()
    {   
        camPos = cam.transform.position;
        hidingEnabled = false;
        ToggleHideUiEnabled();
        StartCoroutine(CheckCameraPosition());
    }

    public void ToggleHideUiEnabled()
    {   
        if (ContainerObject != null)
        {    
            hidingEnabled = !hidingEnabled;
            show_icon.SetActive(!hidingEnabled);
            text_show.SetActive(!hidingEnabled);

            hide_icon.SetActive(hidingEnabled);
            text_hide.SetActive(hidingEnabled);
            UiVisebility(hidingEnabled);//Status von ui panel aendern
        }
    }

    private void UiVisebility (bool state){
        ContainerObject.SetActive(state);
    }


    //Ui panel (ContainerObject wird sichtbar gemacht und im sichtbereich gespawned)
    public void ToleportUi(){

        
            ContainerObject.transform.position = cam.transform.position + cam.transform.forward * 1f;
            ui_panel.transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            if (ContainerObject.activeSelf == false)
            { 
                ToggleHideUiEnabled();
            }
    }
    private IEnumerator CheckCameraPosition()
    {
        // Solange sich die Position nicht geändert hat, warte weiter
        while (cam.transform.position == camPos)
        {
            yield return null; // Warten auf den nächsten Frame
        }
        ToleportUi();
    }

} 
