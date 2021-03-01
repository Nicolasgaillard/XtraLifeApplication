using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;
public class UserDataManager
{
    public void SaveUserBodyColor(Gamer gamer, Color c)
    {
        string color = string.Format("{0}/{1}/{2}/{3}", c.r, c.g, c.b, c.a);

        Bundle value = new Bundle(color);
        gamer.GamerVfs.Domain("private").SetValue("CharacterColor", value)
        .Done(setUserValueRes => {
            Debug.Log("User data set: " + setUserValueRes.ToString());
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set user color due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void GetUserBodyColor(Gamer gamer, Material m)
    {
        gamer.GamerVfs.Domain("private").GetValue("CharacterColor").Done(getUserValueRes => {
            Bundle result = getUserValueRes["result"];

            string color = result["CharacterColor"];
            string[] rgba = color.Split('/');

            Color c = new Color();
            c.r = float.Parse(rgba[0]);
            c.g = float.Parse(rgba[1]);
            c.b = float.Parse(rgba[2]);
            c.a = float.Parse(rgba[3]);

            m.color = c;
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get user color due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void SaveUserTravelledDistance(Gamer gamer, float distance)
    {
        Bundle value = new Bundle(distance);
        gamer.GamerVfs.Domain("private").SetValue("TravelledDistance", value)
        .Done(setUserValueRes => {
            Debug.Log("User data set: " + setUserValueRes.ToString());
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not set user travelled distance due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
    public void GetUserTravelledDistance(Gamer gamer, GameManager manager)
    {
        gamer.GamerVfs.Domain("private").GetValue("TravelledDistance").Done(getUserValueRes => {
            Bundle result = getUserValueRes["result"];

            string distance = result["TravelledDistance"];
            manager.SetPreviousTravelledDistance(float.Parse(distance));
        }, ex => {
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get user travelled distance due to error: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void UserTravelledDistanceTransaction(Gamer gamer, float distance, GameManager manager)
    {
        Bundle value = Bundle.CreateObject("running", distance);
        gamer.Transactions.Post(value).Done(result => {
            Debug.LogWarning(result.TriggeredAchievements.Count);
            foreach (var v in result.TriggeredAchievements)
            {
                Debug.LogWarning(v.Key);
                Debug.LogWarning(v.Value);
            }
        });
    }

    public void GetUserAchievements(Gamer gamer, GameManager manager)
    {
        gamer.Achievements.Domain("private").List().Done(listAchievementsRes => {
            foreach (var achievement in listAchievementsRes)
            {
                Debug.LogWarning(achievement.Key + " : " + achievement.Value.Config.ToString());
            }
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not list achievements: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }
}
