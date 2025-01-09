using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class CallChatgbtWorm : MonoBehaviour
{
    [SerializeField] public InputActionAsset controls;
    [SerializeField] public ChatGPT chat;
    [SerializeField] private string message;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
}
