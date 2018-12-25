using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Team { RED, BLUE };

public class FlagScript : MonoBehaviour {
    public Team teamFlag = Team.RED;
    public bool inPosition = true;
    public bool isHeld = false;

    public Transform BasePosition;
    private Vector3 initPosition;
    private Quaternion initRotation;
    private Vector3 initScale;
    private Vector3 initWorldPosition;

    public GameObject playerHolder;

    public Image flagUI;

	// Use this for initialization
	void Start () {
        initPosition = transform.localPosition;
        initRotation = transform.localRotation;
        initScale = transform.localScale;
        initWorldPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        flagUI.rectTransform.position = new Vector3(transform.position.x, 50, transform.position.z);
    }

    public void grabFlag(Transform FlagHolder, GameObject playerHolder)
    {
        this.inPosition = false;
        this.isHeld = true;

        transform.SetParent(FlagHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.GetComponentInChildren<Cloth>().damping = 0.5f;

        this.playerHolder = playerHolder;
    }

    public void resetFlag()
    {
        this.inPosition = true;
        this.isHeld = false;
        transform.SetParent(BasePosition);
        transform.localPosition = initPosition;
        transform.localRotation = initRotation;
        transform.localScale = initScale;
        transform.GetComponentInChildren<Cloth>().damping = 0f;

        playerHolder = null;
    }

    public void dropFlag()
    {
        this.inPosition = false;
        this.isHeld = false;
        transform.SetParent(null, true);
        transform.localPosition = new Vector3(transform.localPosition.x, initWorldPosition.y, transform.localPosition.z);
        transform.localRotation = initRotation;
        transform.GetComponentInChildren<Cloth>().damping = 0f;
    }
}
