using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementController : MonoBehaviour
{
    public TextMeshProUGUI Achievement;

    private bool _isFirstClick = true;

    private Dictionary<string, string> _achievementMessage = new Dictionary<string, string>()
    {
        { "tortoise", "Congratulation for your first 10 meters travelled !" },
        { "jack rabbit", "Congratulation for your first 100 meters travelled.\n Your are now faster than a tortoise !" },
    };

    public void PopupAchievement(string achivementName)
    {
        Achievement.text = achivementName;
        GetComponent<Animator>().SetTrigger("FadeInPopup");

        _isFirstClick = true;
    }

    private void DisplayAchivementInformation()
    {
        Achievement.text = _achievementMessage[Achievement.text];
        GetComponent<Animator>().SetTrigger("FadeInInformation");
    }

    private void ClosePopup()
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public void Click()
    {
        if(_isFirstClick)
        {
            DisplayAchivementInformation();
            _isFirstClick = false;
        }
        else
        {
            ClosePopup();
        }
    }
}
