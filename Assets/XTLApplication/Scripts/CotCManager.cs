using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class CotCManager : MonoBehaviour
{
    #region Properties
    private CotcGameObject _cotc;
    private Cloud _cloud;
    public Cloud Cloud { get => _cloud; }
    private Gamer _gamer;
    public Gamer Gamer { get => _gamer; set => _gamer = value; }
    private DomainEventLoop _eventLoop;
    #endregion //Properties

    #region Monobehaviour
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
        });
    }

    void Update()
    {
        
    }
    #endregion //Monobehaviour

    #region Public Methods
    public void LogIn(string email, string password)
    {
        // check if user exists
        _cloud.UserExists(LoginNetwork.Email.Describe(), email).Done(userExistsRes => {

            // connect the user
            _cloud.Login(LoginNetwork.Email.Describe(), email, password, Bundle.CreateObject("preventRegistration", true)).Done(gamer => {
                _gamer = gamer;

                LoggedIn();

            }, ex => {
                CotcException error = (CotcException)ex;

                // wrong credentials
                if (error.HttpStatusCode == 400 && error.ServerData["name"] == "BadUserCredentials")
                {
                    Debug.LogWarning("test");
                    FindObjectOfType<LoginController>().WrongUserCredentials();

                    return;
                }

                Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
            });

        }, ex => {
            CotcException error = (CotcException)ex;

            // user does not exists
            if (error.HttpStatusCode == 400 && error.ServerData["name"] == "BadGamerID")
            {
                FindObjectOfType<LoginController>().UnableToFindPlayerId();

                return;
            }

            Debug.LogError("Failed to check user: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });


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

    public void ResetPassword(string email)
    {
        _cloud.SendResetPasswordEmail(email, "nicolas.gaillard@viacesi.fr", "Reset your password", "You can login with this shortcode: [[SHORTCODE]]")
        .Done(resetPasswordRes => {
            Debug.Log("Short code sent");
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Short code sending failed due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void Register(string email, string password, string pseudo)
    {
        _cloud.Login(LoginNetwork.Email.Describe(), email, password).Done(gamer => {
            _gamer = gamer;

            Bundle profileUpdates = Bundle.CreateObject();
            profileUpdates["displayName"] = new Bundle(pseudo);

            _gamer.Profile.Set(profileUpdates);

            LoggedIn();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }
    #endregion //Public Methods

    #region Private methods
    private void Loop_ReceivedEvent(DomainEventLoop sender, EventLoopArgs e)
    {
        Debug.Log("Received event of type " + e.Message.Type + ": " + e.Message.ToJson());
    }

    private void LoggedIn()
    {
        if (_eventLoop != null)
            _eventLoop.Stop();

        _eventLoop = _gamer.StartEventLoop();
        _eventLoop.ReceivedEvent += Loop_ReceivedEvent;

        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.None;
    }
    #endregion //Private Methods


}
