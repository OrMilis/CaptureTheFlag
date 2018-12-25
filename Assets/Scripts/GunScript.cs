using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum holderType { PLAYER, BOT};

public class GunScript : MonoBehaviour {

    public Transform muzzle;
    public GameObject bullet;
    public int rangeInMeter;
    public float rateOfFire;
    public float bulletTravleSpeed;
    public int damagePerBullet;
    public int magazinSize;
    public float reloadTime;
    public float MaxSpread;
    public float recoilRate; //How many bullets till you feel the recoil hitting hard.

    private int currentMagazin;
    private float currentSpread;
    private Team gunTeam;

    public ParticleSystem mainParticleSystem;

    public holderType holderType;
    public Text magazinSizeUI;
    public Text ammoCountUI;
    public Image ammoBarUI;

    // Use this for initialization
    void Start () {
        gunTeam = transform.GetComponentInParent<FlagCaptureScript>().playerTeam;
        currentMagazin = magazinSize;
        if (holderType == holderType.PLAYER)
            magazinSizeUI.text = magazinSize.ToString();
        updateAmmoUI();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void shootCommand(bool toShoot)
    {
        if (toShoot)
        {
            InvokeRepeating("Shoot", 0f, 1f / rateOfFire);
            CancelInvoke("recoilTranquillize");
        }
        else
        {
            InvokeRepeating("recoilTranquillize", 0f, 1f / rateOfFire * recoilRate);
            CancelInvoke("Shoot");
        }
    }

    private void Shoot()
    {
        if (currentMagazin > 0)
        {
            --currentMagazin;

            if (currentSpread < MaxSpread)
                currentSpread = Mathf.Min(currentSpread + (1 / recoilRate), MaxSpread);

            var bulletDir = Quaternion.Euler(0, Random.Range(-currentSpread, currentSpread), 0);

            GameObject obj = Instantiate(bullet, muzzle.position, Quaternion.Euler(muzzle.rotation.eulerAngles + bulletDir.eulerAngles), mainParticleSystem.transform);

            var bulletComponent = obj.GetComponent<BulletScript>();
            bulletComponent.damage = damagePerBullet;
            bulletComponent.teamBullet = gunTeam;

            obj.GetComponent<Rigidbody>().velocity = obj.transform.right * bulletTravleSpeed;
            Destroy(obj, rangeInMeter / bulletTravleSpeed);
        }
        else
        {
            StartCoroutine(reloadGun());
        }

        updateAmmoUI();
    }

    private void recoilTranquillize()
    {
        if (currentSpread > 0)
        {
            currentSpread = Mathf.Max(currentSpread - recoilRate, 0);
        }
        else
            CancelInvoke("recoilTranquillize");
    }

    private IEnumerator reloadGun()
    {
        yield return new WaitForSeconds(reloadTime);
        currentMagazin = magazinSize;
        updateAmmoUI();
    }

    private void updateAmmoUI()
    {
        if (holderType == holderType.PLAYER)
        {
            ammoCountUI.text = currentMagazin.ToString();
            ammoBarUI.fillAmount = currentMagazin  * 1f/ magazinSize;
        }
    } 

}
