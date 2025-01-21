using UnityEngine;

public class WindowDrag : MonoBehaviour
{
    [SerializeField] private GameObject obj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obj.SetActive(false);
    }

    public void onbegindrag(){
        obj.SetActive(true);
    }

    public void onenddrag(){
        obj.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}