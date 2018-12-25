using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public int maxHealth = 100;
    private float currentHealth;
    public int regenerationRate = 30;
    private bool outOfCombat = true;
    private EnemyScript selfScript;
    private PlayerMovement playerScript;
    private bool isRespawning = false;
    private FlagCaptureScript myFlagManager;

    public int respawnTime = 3;

    public holderType holderType;
    public Image healthBarUI;

	// Use this for initialization
	void Start () {
        currentHealth = maxHealth;
        if (holderType == holderType.BOT)
        {
            selfScript = GetComponent<EnemyScript>();
        }
        else
            playerScript = GetComponent<PlayerMovement>();

        myFlagManager = GetComponent<FlagCaptureScript>();
    }
	
	// Update is called once per frame
	void Update () {
		if(outOfCombat && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min((currentHealth + regenerationRate * Time.deltaTime), maxHealth);
            updateHealthUI();
        }
	}

    public void takeDamage(int damage)
    {
        if (!isRespawning)
        {
            currentHealth -= damage;
            outOfCombat = false;
            checkIfDead();
            updateHealthUI();

            StopCoroutine(exitCombat());
            StartCoroutine(exitCombat());
        }
    }

    private void checkIfDead()
    {
        if(currentHealth <= 0)
        {
            isRespawning = true;
            GetComponent<FlagCaptureScript>().playerDeath();
            StartCoroutine(respawn());
        }
    }

    public IEnumerator respawn()
    {

        isRespawning = true;
        myFlagManager.playerDeath();

        var respawnPoint = MatchManagerScript.instance.getRandowmSpawnPoint(myFlagManager.playerTeam);

        if (holderType == holderType.BOT)
            selfScript.dead();
        else
            playerScript.dead();

        yield return new WaitForSeconds(respawnTime);

        if (holderType == holderType.BOT)
            selfScript.respawn();

        transform.position = respawnPoint;
        isRespawning = false;
        resetHealth();
        if (holderType == holderType.PLAYER)
            updateHealthUI();
    }

    public void resetHealth()
    {
        currentHealth = maxHealth;
        isRespawning = false;
    }

    IEnumerator exitCombat()
    {
        yield return new WaitForSeconds(6f);
        outOfCombat = true;
    }

    void updateHealthUI()
    {
        if (holderType == holderType.PLAYER)
            healthBarUI.fillAmount = currentHealth / maxHealth;
    }

    public bool takeHealthPack(int health)
    {
        bool isTaken = false;
        if (currentHealth < maxHealth)
        {
            currentHealth += health;
            isTaken = true;
        }

        updateHealthUI();
        return isTaken;
    }

    public bool isDead()
    {
        return currentHealth <= 0;
    }
}
