using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchManagerScript : MonoBehaviour {

    private Dictionary<Team, TeamManager> teams = new Dictionary<Team, TeamManager>();

    public int capturesToWin = 5;
    public int mapsToWin = 2;

    public Text blueTeamMapScoreUI;
    public Text redTeamMapScoreUI;

    public Text blueTeamMatchScoreUI;
    public Text redTeamMatchScoreUI;

    public GameObject BlueSpawn;
    public GameObject RedSpawn;

    public static MatchManagerScript instance;

    public LevelChanger levelChanger;

    public Canvas globalCanvas;

    public Team winner;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        initMapScore();
        updateManagerUI();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void initMapScore()
    {
        teams = new Dictionary<Team, TeamManager>();

        var redTeam = new TeamManager(0, 0);
        redTeam.TeamMapScoreUI = redTeamMapScoreUI;
        redTeam.TeamMatchScoreUI = redTeamMatchScoreUI;

        var blueTeam = new TeamManager(0, 0);
        blueTeam.TeamMapScoreUI = blueTeamMapScoreUI;
        blueTeam.TeamMatchScoreUI = blueTeamMatchScoreUI;

        teams.Add(Team.RED, redTeam);
        teams.Add(Team.BLUE, blueTeam);
    } 

    public void scoreForTeam(Team team)
    {
        teams[team].MapScore++;
        checkMapWin();
        updateManagerUI();
    }

    void checkMapWin()
    {
        bool aWin = false;
        if (teams[Team.BLUE].MapScore >= capturesToWin)
        {
            teams[Team.BLUE].MatchScore++;
            aWin = true;
            Debug.Log("Blue wins map");
        }
        else if (teams[Team.RED].MapScore >= capturesToWin)
        {
            teams[Team.RED].MatchScore++;
            aWin = true;
            Debug.Log("Red wins map");
        }

        if (aWin)
        {
            teams[Team.BLUE].MapScore = 0;
            teams[Team.RED].MapScore = 0;
            if (checkMatchWin())
            {
                globalCanvas.gameObject.SetActive(false);
                levelChanger.FadeToLevel(4);
            }
            else
                levelChanger.FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }   

    }

    bool checkMatchWin()
    {
        Debug.Log("Blue: " + teams[Team.BLUE].MatchScore);
        Debug.Log("Red: " + teams[Team.RED].MatchScore);

        bool aWin = false;
        if (teams[Team.BLUE].MatchScore >= mapsToWin)
        {
            winner = Team.BLUE;
            aWin = true;
        }
        else if (teams[Team.RED].MatchScore >= mapsToWin)
        {
            winner = Team.RED;
            aWin = true;
        }

        return aWin;
    }

    void updateManagerUI()
    {
        foreach (TeamManager tm in teams.Values)
            tm.updateMyTeamUI();
    }

    public Vector3 getRandowmSpawnPoint(Team team)
    {
        switch (team)
        {
            case Team.BLUE:
                return getSpawnPoint(BlueSpawn.transform);
            case Team.RED:
                return getSpawnPoint(RedSpawn.transform);
            default:
                return Vector3.up;
        }
    }

    private Vector3 getSpawnPoint(Transform spawn)
    {
        var child = Random.Range(0, spawn.transform.childCount);
        return spawn.GetChild(child).position;
    }

    public void setSpawns()
    {
        Debug.Log("In set Spawns");
        BlueSpawn = GameObject.FindGameObjectWithTag("BlueSpawn");
        RedSpawn = GameObject.FindGameObjectWithTag("RedSpawn");
    }

    public void setLevelChanger(LevelChanger levelChanger)
    {
        this.levelChanger = levelChanger;
    }
}

public class TeamManager
{
    public int MapScore { get; set; }
    public int MatchScore { get; set; }
    public Text TeamMapScoreUI { get; set; }
    public Text TeamMatchScoreUI { get; set; }

    public TeamManager(int mapScore, int matchScore)
    {
        MapScore = mapScore;
        MatchScore = matchScore;
    }

    public void updateMyTeamUI()
    {
        TeamMapScoreUI.text = MapScore.ToString();
        TeamMatchScoreUI.text = MatchScore.ToString();
    }
}
