using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using System;
using NaughtyAttributes;

public class ChatGPT : MonoBehaviour
{

    [ShowNonSerializedField] private string apiKey = "";
    
    [SerializeField] private InputActionAsset controls;
    [SerializeField] private string apiUrl;
    [SerializeField] private string setnewkey;
    private string worms;

    
    // Systemnachricht zur Festlegung des Assistenten-Charakters
    private string systemMessage = "The Platform the user is working with is unity. You are an Assitant, that provides the user with suitable data for the Objects the user want to generate in Unity. Your Output is  like this: atribute: amount; atribute:amount. You have no other output than the user is suggesting. You do not have any explaining sentence.";

    public event Action<string> MessageReceived;

    // Funktion zur Abfrage der ChatGPT API
    public void SendMessageToChatGPT(string userMessage)
    {
        StartCoroutine(SendRequest(userMessage));
       
    }
    public void SendMessageToChatGPT(string userMessage, string worms)
    {
            StartCoroutine(SendRequest(userMessage));
           this.worms = worms;
    }

    void OnValidate()
    {
       
        if(PlayerPrefs.HasKey("ApiKey"))
        {
            Debug.Log("ApiKey loaded");
           apiKey = PlayerPrefs.GetString("ApiKey");
        }
    }
    void Start()
    {

        apiKey = PlayerPrefs.GetString("ApiKey");

    }
    [Button]
    void SafeNewKey()
    {
        PlayerPrefs.SetString("ApiKey", setnewkey);
        setnewkey = string.Empty;
        Debug.Log($"ApiKey saved: {PlayerPrefs.GetString("ApiKey")}");
    }
    

    private IEnumerator SendRequest(string userMessage)
    {
        // Setze die Anfragedaten entsprechend der erwarteten Struktur, einschließlich der Systemnachricht
        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = systemMessage },  // Systemnachricht für den Assistenten-Charakter
                new { role = "user", content = userMessage }
            },
            max_tokens = 1500
        };

        // Serialisiere das JSON
        string jsonData = JsonConvert.SerializeObject(requestData);

        // Erstelle die Webanfrage
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Sende die Anfrage und warte auf die Antwort
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Anfrage Fehler: " + request.error);
            }
            else
            {
                // Verarbeite die API-Antwort
                var responseJson = request.downloadHandler.text;
                OpenAIResponse responseData = null;

                try
                {
                    responseData = JsonConvert.DeserializeObject<OpenAIResponse>(responseJson);
                }
                catch (JsonException jsonEx)
                {
                    Debug.LogError("Fehler beim Parsen der JSON-Antwort: " + jsonEx.Message);
                }

                // Ausgabe der Antwort im Debug
                if (responseData != null && responseData.choices.Count > 0)
                {
                    string responseText = responseData.choices[0].message.content;
                    Debug.Log("ChatGPT Antwort: " + responseText);
                    MessageReceived?.Invoke(responseText+worms);
                    //spawner.Spawn(responseText);

                }
                else
                {
                    Debug.LogWarning("Die Antwort enthält keine gültigen Daten.");
                }
            }
        }

    }

    


    // Hilfsklassen zum Verarbeiten der Antwort
    [System.Serializable]
    public class OpenAIResponse
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
}
