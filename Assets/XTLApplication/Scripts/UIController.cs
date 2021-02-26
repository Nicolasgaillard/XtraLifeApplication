﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject InformationPanel;
    public TextMeshProUGUI TextInformation;

    private Coroutine InformationPanelCoroutine;
    public enum UIPanel
    {
        None = 0,
        Login = 1,
        ResetPassword = 2,
        Register = 3,
    }

    public UIPanel BasePanel;

    [System.Serializable]
    public struct UIPanelMap
    {
        public UIPanel Panel;
        public GameObject GameObject;
    }
    public List<UIPanelMap> PanelList;

    private UIPanel _panel = UIPanel.None;
    public UIPanel ActivePanel { get => _panel;
        set
        {
            if (value != _panel)
            {
                if (_panel != UIPanel.None)
                {
                    PanelList[(int)_panel].GameObject.SetActive(false);
                }

                if (value != UIPanel.None)
                {
                    PanelList[(int)value].GameObject.SetActive(true);
                }

                _panel = value;

                if(InformationPanelCoroutine != null)
                {
                    StopCoroutine(InformationPanelCoroutine);
                    InformationPanel.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        ActivePanel = BasePanel;
    }

    public void DisplayInformation(string message)
    {
        InformationPanelCoroutine = StartCoroutine(FadeInPanel(message));
    }

    private IEnumerator FadeInPanel(string message)
    {
        InformationPanel.SetActive(true);
        TextInformation.text = message;
        InformationPanel.GetComponent<Animator>().SetTrigger("FadeIn");

        yield return new WaitForSeconds(4.5f);

        InformationPanel.SetActive(false);
    }
}
