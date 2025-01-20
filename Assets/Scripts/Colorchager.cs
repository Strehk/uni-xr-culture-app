#if UNITY_EDITOR
using UnityEngine;

public class Colorchanger : MonoBehaviour
{
    [SerializeField] private BottonManager uiPanel;


    void Start()
    {
        
    }

    public void ChangeToGreen()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.green);
        }
    }
    public void ChangeToRed()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.red);
        }
    }

    public void ChangeToBlue()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.blue);
        }
    }

    public void ChangeToYellow()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.yellow);
        }
    }

    public void ChangeToCyan()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.cyan);
        }
    }

    public void ChangeToMagenta()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetColor(Color.magenta);
        }
    }

    public void ChangeRandomColor()
    {
        Wurm wurm = uiPanel.GetWurm();
        if (wurm != null)
        {
            wurm.SetRandomColor();
        }
    }

}
#endif