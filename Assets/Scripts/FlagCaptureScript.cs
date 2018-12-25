using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCaptureScript : MonoBehaviour {

    public bool isHolding = false;
    public Team playerTeam = Team.RED;

    public Transform FlagHolder;
    private FlagScript HeldFlag;
    private Health myHealth;
    private PlayerStatistics stats;

	// Use this for initialization
	void Start () {
        myHealth = GetComponent<Health>();

        if (myHealth.holderType == holderType.PLAYER)
            stats = GameObject.FindGameObjectWithTag("Manager").GetComponent<PlayerStatistics>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        //string message = "";
        if (other.CompareTag("Flag"))
        {
            //message += "Touched flag. \n";
            FlagScript flag = other.GetComponent<FlagScript>();
            /*message += "Flag is " + flag.teamFlag.ToString() + ". \n";
            message += "My Team is " + playerTeam.ToString() + ". \n";*/
            if (flag.teamFlag != playerTeam)
            {
                if (!isHolding && !flag.isHeld && !myHealth.isDead())
                {
                    grabFlag(flag);
                    //message += "Grabbing Flag.";
                }
            }

            else if(flag.teamFlag == playerTeam)
            {
                if (isHolding && flag.inPosition)
                {
                    captureFlag();
                    //message += "Capturing Flag.";
                }
                else if (!flag.inPosition && !flag.isHeld)
                {
                    retrieveFlag(flag);
                }
            }
        }
        //Debug.Log(message);
    }

    public void retrieveFlag(FlagScript flag)
    {
        flag.resetFlag();
        if (stats != null)
            stats.retrieves++;
    }

    public void grabFlag(FlagScript flag)
    {
        flag.grabFlag(FlagHolder, this.gameObject);

        isHolding = true;
        HeldFlag = flag;
    }

    public void captureFlag()
    {
        HeldFlag.resetFlag();
        isHolding = false;

        if (stats != null)
            stats.captures++;

        MatchManagerScript.instance.scoreForTeam(this.playerTeam);
    }

    public void playerDeath()
    {
        if (isHolding)
        {
            HeldFlag.dropFlag();
            isHolding = false;
        }

        if (stats != null)
            stats.deaths++;
    }
}
