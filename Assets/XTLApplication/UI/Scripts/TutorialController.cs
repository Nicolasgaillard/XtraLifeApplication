using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public void Continue()
    {
        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.Login;
    }
}
