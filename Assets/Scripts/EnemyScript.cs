using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public enum AIProfile { DEFENDER, ATTACKER, EXECUTIONER};
public enum AIState { HUNT, KILL, FLAG, CAPTURE}

public class EnemyScript : MonoBehaviour {

    private Transform teamFlag;
    private FlagScript teamFlagComponent;
    private Transform rivalFlag;
    private FlagScript rivalFlagComponent;
    private FlagCaptureScript myFlagManager;
    public NavMeshAgent agent;

    public FieldOfView fow;
    public GunScript gun;
    public IKControl ik;

    public CharacterAnimController character;
    public AIProfile profile;

    private State[] states = new State[4];
    public AIState currentState = AIState.HUNT;

    private bool isRespawning = false;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        myFlagManager = GetComponent<FlagCaptureScript>();
        fow = GetComponentInChildren<FieldOfView>();
        gun = GetComponentInChildren<GunScript>();
        ik = GetComponent<IKControl>();

        initFlags();
        initStates();

        agent.updateRotation = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(!isRespawning)
            states[(int)currentState].action();
	}

    private void initFlags()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Flag"))
        {
            if (obj.GetComponent<FlagScript>().teamFlag == myFlagManager.playerTeam)
            {
                teamFlag = obj.transform;
                teamFlagComponent = teamFlag.GetComponent<FlagScript>();
            }
            else
            {
                rivalFlag = obj.transform;
                rivalFlagComponent = rivalFlag.GetComponent<FlagScript>();
            }
        }
    }

    private void initStates()
    {
        states[(int)AIState.HUNT] = new HuntState(this.gameObject, this);
        states[(int)AIState.KILL] = new KillState(this.gameObject, this);
        states[(int)AIState.FLAG] = new FlagState(this.gameObject, this);
        states[(int)AIState.CAPTURE] = new CaptureState(this.gameObject, this);
    }

    public void dead()
    {
        agent.enabled = false;
        gun.shootCommand(false);
        character.playDeadAnim();
    }

    public void respawn()
    {
        agent.enabled = true;
        currentState = AIState.FLAG;
    }

    public void setCurrentState(AIState nextState)
    {
        currentState = nextState;
    }

    public GameObject getTeamFlag()
    {
        return teamFlag.gameObject;
    }

    public FlagScript getTeamFlagScript()
    {
        return teamFlagComponent;
    }

    public GameObject getRivalFlag()
    {
        return rivalFlag.gameObject;
    }

    public FlagScript getRivalFlagScript()
    {
        return rivalFlagComponent;
    }

    public State getSpecificState(AIState state)
    {
        return states[(int)state];
    }

    public FlagCaptureScript getMyFlagManager()
    {
        return myFlagManager;
    }
}

public abstract class State
{
    protected GameObject self;
    protected EnemyScript selfScript;
    protected IKControl ik;
    protected GunScript gun;
    protected CharacterAnimController character;

    protected GameObject teamFlag;
    protected FlagScript teamFlagScript;
    protected FlagCaptureScript myFlagManager;

    public State(GameObject self, EnemyScript selfScript)
    {
        this.self = self;
        this.selfScript = selfScript;
        this.ik = selfScript.ik;
        this.gun = selfScript.gun;
        this.character = selfScript.character;
        teamFlag = selfScript.getTeamFlag();
        teamFlagScript = selfScript.getTeamFlagScript();
        myFlagManager = selfScript.getMyFlagManager();

    }

    public abstract void action();

    protected void changeState(AIState nextState)
    {
        selfScript.setCurrentState(nextState);
    }

    protected void lookAt(Transform target)
    {
        var lookPos = target.position - self.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        self.transform.rotation = Quaternion.Slerp(self.transform.rotation, rotation, Time.deltaTime * 10);

    }

    protected void moveTo(Transform target)
    {
        if (selfScript.agent.enabled)
        {
            selfScript.agent.SetDestination(target.position);

            if (selfScript.agent.remainingDistance > selfScript.agent.stoppingDistance)
                character.Move(selfScript.agent.desiredVelocity);
            else
                character.Move(Vector3.zero);
        }
    }

    protected void checkTeamFlagStatus()
    {
        if (!teamFlagScript.inPosition)
        {
            changeState(AIState.HUNT);
            return;
        }
    }

    protected bool isSomeoneInFOV()
    {
        return selfScript.fow.visibleTargets.Count > 0;
    }

    protected bool isSpecificInFOV(Transform player)
    {
        return selfScript.fow.visibleTargets.Keys.Contains(player);
    }
}

public class HuntState : State
{
    GameObject flagHolder;
    FlagCaptureScript flagHolderScript;

    bool isShooting = false;

    public HuntState(GameObject self, EnemyScript selfScript)
        : base(self, selfScript)
    {

    }
    
    public override void action()
    {
        flagHolder = teamFlagScript.playerHolder;

        if (!teamFlagScript.inPosition) {

            moveTo(teamFlag.transform);

            if (selfScript.fow.viewAngle != 360)
                selfScript.fow.viewAngle = 360;

            if (flagHolder.transform != null)
            {
                if (isSpecificInFOV(flagHolder.transform))
                {
                    lookAt(flagHolder.transform);
                    if (!isShooting)
                    {
                        ik.lookObj = flagHolder.transform;
                        isShooting = true;
                        gun.shootCommand(true);
                    }
                }
                else
                {
                    if (isShooting)
                    {
                        ik.lookObj = null;
                        isShooting = false;
                        gun.shootCommand(false);
                    }
                }
            }
        }
        else
        {
            if (isShooting)
            {
                ik.lookObj = null;
                isShooting = false;
                gun.shootCommand(false);
            }

            selfScript.fow.viewAngle = 110;
            changeState(AIState.FLAG);
            return;
        }
        
        /*if (teamFlagScript.inPosition)
        {
            //TODO: Change to Default state.
            throw new NotImplementedException();
        }*/
    }
}

public class KillState : State
{

    Transform target;
    Health targetHealth;
    bool isShooting = false;

    public KillState(GameObject self, EnemyScript selfScript)
        : base(self, selfScript)
    {

    }

    public override void action()
    {
        checkTeamFlagStatus();
        checkTarget();

        if (target)
        {
            moveTo(target);
            lookAt(target.transform);
            if (!isShooting)
            {
                ik.lookObj = target.transform;
                isShooting = true;
                gun.shootCommand(true);
            }
        }
        else
        {
            if (isShooting)
            {
                ik.lookObj = null;
                isShooting = false;
                gun.shootCommand(false);
            }
            changeState(AIState.FLAG);
        }
    }

    public void setTarget(Transform target)
    {
        this.target = target;
        targetHealth = target.GetComponent<Health>();
    }

    public void checkTarget()
    {
        if (targetHealth.isDead())
            target = null;
    }
}

public class FlagState : State
{
    GameObject rivalFlag;
    FlagScript rivalFlagScript;

    KillState killState;

    public FlagState(GameObject self, EnemyScript selfScript)
        : base(self, selfScript)
    {
        rivalFlag = selfScript.getRivalFlag();
        rivalFlagScript = selfScript.getRivalFlagScript();

        killState = selfScript.getSpecificState(AIState.KILL) as KillState;
    }

    public override void action()
    {
        checkTeamFlagStatus();

        moveTo(rivalFlag.transform);

        if (isSomeoneInFOV())
        {
            killState.setTarget(selfScript.fow.visibleTargets.Keys.ElementAt(0));
            changeState(AIState.KILL);
            return;
        }

        if (myFlagManager.isHolding)
        {
            changeState(AIState.CAPTURE);
        }
    }
}

public class CaptureState : State
{
    public CaptureState(GameObject self, EnemyScript selfScript)
        : base(self, selfScript)
    {

    }

    public override void action()
    {
        checkTeamFlagStatus();
        moveTo(teamFlag.transform);

        if (!myFlagManager.isHolding)
            changeState(AIState.FLAG);
    }
}