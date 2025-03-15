using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public Text titleText;
    public Text messageText;

    public Text option1Text;
    public Button option1Button;
    
    public Text option2Text;
    public Button option2Button;
    
    public static PopupUI Instance;

    void Awake() 
    {
        if (Instance != null && Instance != this) 
        {
            Destroy (this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad (this.gameObject);
        }
    }
    
    void Start()
    {
        this.GetComponent<Canvas>().enabled = true;
        this.gameObject.SetActive(false);
    }

    public void ShowPopUp(string title, string message, string op1Message, Action op1Action = null, string op2Message = null, Action op2Action = null)
    {
        this.gameObject.SetActive(true);
        
        titleText.text = title;
        messageText.text = message;

        option1Text.text = op1Message;
        option1Button.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            op1Action?.Invoke();
        });

        if (string.IsNullOrEmpty(op2Message))
        {
            option2Text.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
            return;
        }

        option2Text.text = op2Message;
        option2Button.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            op2Action?.Invoke();
        });

    }
    
    
}
