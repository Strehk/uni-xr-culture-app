using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

public class PalmMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject ui_panel;
    [SerializeField] private GameObject hide_icon;
    [SerializeField] private GameObject show_icon;

    [SerializeField] private Camera cam;

    private bool hidingEnabled;
    void Start()
    {
        hidingEnabled = false;
        ToggleHideUiEnabled();
    }

    public void ToggleHideUiEnabled()
    {
        show_icon.SetActive(!hidingEnabled);
        hide_icon.SetActive(hidingEnabled);
        HideUi(!hidingEnabled);//Status von ui panel aendern
        hidingEnabled = !hidingEnabled;
    }

    private void HideUi (bool hide){
        ui_panel.SetActive(hide);
    }

    public void ToleportUi(){
        ui_panel.transform.position = cam.transform.position + cam.transform.forward * 1.5f;
        ui_panel.transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
    }
}
