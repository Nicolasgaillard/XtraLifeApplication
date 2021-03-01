using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;
using TMPro;

public class LeaderBoardController : MonoBehaviour
{
    public Transform ContentParent;

    public GameObject ScorePrefab;

    public void SetData(PagedList<Score> scoreList)
    {
        foreach(Transform child in ContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var score in scoreList)
        {
            GameObject go = Instantiate(ScorePrefab, ContentParent);
            go.GetComponent<TextMeshProUGUI>().text = string.Format("{0}. {1} : {2} P\n", score.Rank, score.GamerInfo["profile"]["displayName"], score.Value);
        }
    }
}
