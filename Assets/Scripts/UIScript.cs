using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {

    public Text deathStatics;
    public Text captureStatics;
    public Text retrieveStatics;

    public Text WinLoseText;

    public GameObject characterStatistics;
    public PlayerStatistics statistics;

    public Color redColor;
    public Color blueColor;

    private void Awake()
    {
        characterStatistics = MatchManagerScript.instance.gameObject;
        statistics = characterStatistics.GetComponent<PlayerStatistics>();
    }

    // Use this for initialization
    void Start () {

        Cursor.visible = true;

        int temp = statistics.deaths / 2;
        deathStatics.text = temp.ToString();

        temp = statistics.captures;
        captureStatics.text = temp.ToString();

        temp = statistics.retrieves;
        retrieveStatics.text = temp.ToString();

        Team team = MatchManagerScript.instance.winner;

        WinLoseText.text = team.ToString() + " WINS!";

        if (team == Team.BLUE)
            WinLoseText.color = blueColor;
        else
            WinLoseText.color = redColor;
    }
	
}
