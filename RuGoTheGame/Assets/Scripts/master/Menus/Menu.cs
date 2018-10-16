using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    protected bool IsVrRun = false;

    void Start () {
		
	}
	
	void Update () {
		
	}

    public void ReparentMenu()
    {
        // Parent the gadget selector menu underneath the main camera
        GameObject menuParent = GameObject.FindGameObjectWithTag("MainCamera");
        transform.SetParent(menuParent.transform);
        if (IsVrRun)
        {
            transform.localPosition = new Vector3(0, 0, 1);
        }
        else
        {
            transform.localPosition = new Vector3(0.2f, 0.02f, 0.7f);
        }
        //transform.localPosition = new Vector3(0, 0, 1);
        transform.localRotation = Quaternion.identity;
    }


    public virtual void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public void SetVRFlag(bool flag)
    {
        IsVrRun = flag;
    }
}
