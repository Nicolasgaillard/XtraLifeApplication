using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    public TMP_InputField EmailField, PasswordField, PseudoField;

    public Button DisabledLoginButton;

    private UIController _controller;

    void Start()
    {
        EmailField.text = "ginixaf242@geeky83.com";
        PasswordField.text = "azerty1234";
        PseudoField.text = "JD";

        DisabledLoginButton.interactable = false;

        _controller = FindObjectOfType<UIController>();
    }

    public void Register()
    {
        if (!string.IsNullOrEmpty(EmailField.text) && !string.IsNullOrEmpty(PasswordField.text) && !string.IsNullOrEmpty(PseudoField.text))
        {
            FindObjectOfType<GameManager>().Register(EmailField.text, PasswordField.text, PseudoField.text);
        }
        else
        {
            _controller.DisplayInformation("Please fill in the fields above.");
        }
    }

    public void LogIn()
    {
        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.Login;
    }
}
