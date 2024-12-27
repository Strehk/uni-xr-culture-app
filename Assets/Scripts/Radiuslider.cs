
using Unity.VisualScripting;
using UnityEngine;

public class Radiuslider : MonoBehaviour
{
    [SerializeField] private Wurm wurm;
    [SerializeField] private UnityEngine.UI.Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (slider != null&& wurm != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 0.6f;
        }
    }
    public void setstartvalue()
    {
         slider.value = wurm.GetRadius(); 
    }

    public void setWurm(BottonManager buttonmanager)
    {
        this.wurm = buttonmanager.GetWurm();
        setstartvalue();
    }

    public void onValueChanged()
    {
        wurm.SetRadius(slider.value);
    }
}
