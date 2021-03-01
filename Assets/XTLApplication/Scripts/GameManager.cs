using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Properties
    private CotcGameObject _cotc;
    private Cloud _cloud;
    public Cloud Cloud { get => _cloud; }
    private Gamer _gamer;
    public Gamer Gamer { get => _gamer; set => _gamer = value; }
    private DomainEventLoop _eventLoop;

    private UserDataManager _dataManager;

    private bool _isLoggedIn = false;

    public Material CharacterMaterial;
    public GameObject CharacterBody;
    private GameObject _playerBody;

    private CameraController _cameraController;

    public float _currentTravelledDistance, _previousTravelledDistance;
    private Vector3 _previousPlayerPosition;
    #endregion //Properties

    #region Monobehaviour
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        _dataManager = new UserDataManager();

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

        _cameraController = FindObjectOfType<CameraController>();
        _cameraController.StartRotateAroundTheWorld();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LogOut();
            StartCoroutine(Exit());
        }

        if (_isLoggedIn)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.Skin;
            }

            _currentTravelledDistance += Vector3.Distance(_playerBody.transform.position, _previousPlayerPosition);
            _previousPlayerPosition = _playerBody.transform.position;

            if(_currentTravelledDistance >= 10)
            {
                SaveUserTravelledDistance();
                _previousTravelledDistance += _currentTravelledDistance;
                _currentTravelledDistance = 0;
            }
        }
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
            SaveUserTravelledDistance();

            _cloud.Logout(_gamer).Done(result =>
            {
                Debug.Log("Logout succeeded");
                _isLoggedIn = false;
            }, ex =>
            {
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

            SaveCredentials(email, password);

            LoggedIn();
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to login: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }

    public void SaveScore(float score)
    {
        _gamer.Scores.Domain("private").Post((int)score * 100, "runBoard", ScoreOrder.LowToHigh,
            "context for score", false)
            .Done(postScoreRes => {
                Debug.Log("Post score: " + postScoreRes.ToString());
            }, ex => {
                        // The exception should always be CotcException
                        CotcException error = (CotcException)ex;
                Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
    }

    public void DisplayBestScore()
    {
        _gamer.Scores.Domain("private").BestHighScores("runBoard", 10, 1)
            .Done(bestHighScoresRes =>
            {
                FindObjectOfType<LeaderBoardController>().SetData(bestHighScoresRes);
                Debug.LogWarning("score");
            }, ex =>
            {
                // The exception should always be CotcException
                CotcException error = (CotcException)ex;
                Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
            });
    }

    public void SetPreviousTravelledDistance(float distance)
    {
        _previousTravelledDistance = distance;
    }

    public void GetNewAchievement()
    {

    }
#endregion //Public Methods

#region Private methods
    private void Loop_ReceivedEvent(DomainEventLoop sender, EventLoopArgs e)
    {
        Debug.LogWarning("Received event of type " + e.Message.Type + ": " + e.Message.ToJson());
    }

    private void LoggedIn()
    {
        if (_eventLoop != null)
            _eventLoop.Stop();

        _eventLoop = _gamer.StartEventLoop();
        _eventLoop.ReceivedEvent += Loop_ReceivedEvent;

        _isLoggedIn = true;

        FindObjectOfType<UIController>().ActivePanel = UIController.UIPanel.Skin;
        _cameraController.StopRotateAroundTheWorld();
        _playerBody = Instantiate(CharacterBody).transform.GetChild(1).gameObject;
        _cameraController.SetupPlayerCamera();

        foreach(Transform child in GameObject.Find("IgnorePanel").transform)
        {
            child.gameObject.SetActive(true);
        }
        

        GetUserData();
        DisplayBestScore();
    }

    private void SaveCredentials(string email, string password)
    {
        PlayerPrefs.SetString("userId", email);
        PlayerPrefs.SetString("userPassword", password);
    }

    #region User Data
    private void GetUserData()
    {
        GetUserColor();
        GetUserTravelledDistance();
        GetUserAchievments();
    }

    public void SaveUserBodyColor(Color c)
    {
        _dataManager.SaveUserBodyColor(_gamer, c);
    }

    private void GetUserColor()
    {
        _dataManager.GetUserBodyColor(_gamer, CharacterMaterial);
    }

    private void SaveUserTravelledDistance()
    {
        _dataManager.SaveUserTravelledDistance(_gamer, _previousTravelledDistance + _currentTravelledDistance);
        _dataManager.UserTravelledDistanceTransaction(_gamer, _currentTravelledDistance, this);
    }

    private void GetUserTravelledDistance()
    {
        _dataManager.GetUserTravelledDistance(_gamer, this);
    }

    private void GetUserAchievments()
    {
        _dataManager.GetUserAchievements(_gamer, this);
    }
    #endregion //User Data

    private IEnumerator Exit()
    {
        yield return new WaitWhile(IsLoggedIn);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private bool IsLoggedIn()
    {
        return _isLoggedIn;
    }
#endregion //Private Methods


}
