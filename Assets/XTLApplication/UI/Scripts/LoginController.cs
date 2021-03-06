﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public TMP_InputField EmailField, PasswordField;

    public Button DisabledLoginButton, DisabledResetPasswordButton;

    private UIController _controller;

    void Start()
    {
        EmailField.text = PlayerPrefs.GetString("userId");
        PasswordField.text = PlayerPrefs.GetString("userPassword");

        DisabledLoginButton.interactable = false;
        DisabledResetPasswordButton.interactable = false;

        _controller = FindObjectOfType<UIController>();
    }

    public void Login()
    {
        if(!string.IsNullOrEmpty(EmailField.text) && !string.IsNullOrEmpty(PasswordField.text))
        {
            FindObjectOfType<GameManager>().LogIn(EmailField.text, PasswordField.text);
        }
        else
        {
            _controller.DisplayInformation("Please fill in the fields above.");
        }
    }

    public void UnableToFindPlayerId()
    {
        _controller.DisplayInformation("Unable to retrieve player account, please verify your email or create an account.");
    }

    public void WrongUserCredentials()
    {
        _controller.DisplayInformation("Unable to connect to your account : wrong credentials.");
    }

    public void ResetPassword()
    {
        _controller.ActivePanel = UIController.UIPanel.ResetPassword;
    }

    public void Register()
    {
        _controller.ActivePanel = UIController.UIPanel.Register;
    }
}
