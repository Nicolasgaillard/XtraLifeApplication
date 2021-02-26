using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class GameManager : MonoBehaviour
{
    private CotcGameObject _cotc;
    private Cloud _cloud;
    private Gamer _gamer;

    private bool _isCloudSetup = false;


    private string _userEmail, _userPassword;
    public string UserEmail { get => _userEmail; set => _userEmail = value; }
    public string UserPassword { get => _userPassword; set => _userPassword = value; }


    void Start()
    {
        DontDestroyOnLoad(gameObject);

        Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
            Debug.LogError("Unhandled exception: " + e.Exception.ToString());
        };

        _cotc = GetComponent<CotcGameObject>();
        _cotc.GetCloud().Done(cloud => { 
            _cloud = cloud;

            cloud.HttpRequestFailedHandler = (HttpRequestFailedEventArgs e) => {
                if (e.UserData == null)
                {
                    e.UserData = new object();
                    e.RetryIn(1000);
                }
                else
                    e.Abort();
            };

            _isCloudSetup = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LogIn()
    {
        if(string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword))
        {

            return;
        }

        _cloud.Login(LoginNetwork.Email.Describe(), UserEmail, UserPassword).Done(gamer => {
            Debug.Log(gamer);

            Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
            Debug.Log("Login data: " + gamer);
            Debug.Log("Server time: " + gamer["servertime"]);
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }

    public void OnDestroy()
    {
        LogOut();
    }

    public void LogOut()
    {
        if (_gamer != null)
        {
            _cloud.Logout(_gamer).Done(result =>
            {
                Debug.Log("Logout succeeded");
            }, ex =>
            {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });
        }
    }
}
