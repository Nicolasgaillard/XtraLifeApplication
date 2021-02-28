using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckPointManager : MonoBehaviour
{
    public Material CheckPoint, NextCheckPOint;

    private TextMeshProUGUI _score;

    private int _activeCheckPoint = 0;

    public float Timer = 0;
    private bool _isPlaying = false;

    void Start()
    {
        SetNextPoint();
    }

    void Update()
    {
        if(_isPlaying)
        {
            Timer += Time.deltaTime;

            _score.text = Timer.ToString("F1");
        }
    }

    public void StartGame()
    {
        _isPlaying = true;

        if(_score == null)
        {
            _score = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        }
    }

    public void StopGame()
    {
        _isPlaying = false;

        FindObjectOfType<GameManager>().SaveScore(Timer);
        FindObjectOfType<GameManager>().DisplayBestScore();

    }

    public void Trigger(GameObject go)
    {
        if (go.transform == transform.GetChild(_activeCheckPoint))
        {
            if (go.transform == transform.GetChild(0) && !_isPlaying)
            {
                StartGame();
            }
            else if(go.transform == transform.GetChild(transform.childCount -1) && _isPlaying)
            {
                StopGame();
            }

            if(_isPlaying)
            {
                SetCheckPointMaterial();

                _activeCheckPoint++;

                if (_activeCheckPoint < transform.childCount)
                {
                    SetNextPoint();
                }
            }
        }
    }

    private void SetCheckPointMaterial()
    {
        SetMaterial(transform.GetChild(_activeCheckPoint), CheckPoint);
    }

    private void SetNextPoint()
    {
        SetMaterial(transform.GetChild(_activeCheckPoint), NextCheckPOint);
    }

    private void SetMaterial(Transform parent, Material m)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = m;
        }
    }
}
