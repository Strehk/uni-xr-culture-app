#if UNITY_EDITOR
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
            slider.maxValue = 0.4f;
        }
    }

    public void setWurm(Wurm newWurm)
    {
        this.wurm = newWurm;
        slider.value = wurm.GetRadius(); 
    }

    public void onValueChanged()
    {
        if (wurm == null)
        {
            Debug.LogWarning("Wurm ist null. Setze zuerst einen gültigen Wurm, bevor der Slider geändert wird.");
            return;
        }

        wurm.SetRadius(slider.value);
    }
}
#endif