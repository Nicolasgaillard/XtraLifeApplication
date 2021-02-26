using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResetPasswordController : MonoBehaviour
{
    public TMP_InputField EmailField;

    private void Start()
    {
        EmailField.text = "ginixaf242@geeky83.com";
    }
    public void ResetPassword()
    {
        if (!string.IsNullOrEmpty(EmailField.text))
        {
            FindObjectOfType<CotCManager>().ResetPassword(EmailField.text);
        }
    }
}
