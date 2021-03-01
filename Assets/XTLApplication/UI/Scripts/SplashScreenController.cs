using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : MonoBehaviour
{
    public void Continue()
    {
        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.Tutorial;
    }
}
