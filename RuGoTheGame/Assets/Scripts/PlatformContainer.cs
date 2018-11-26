using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformContainer : MonoBehaviour
{
    public Transform VRContainer;
    public Transform ARContainer;

	// Use this for initialization
	void Start ()
    {
#if RUGO_AR
        LoadARContainer();
#elif RUGO_VR
        LoadVRContainer();
#endif
    }

#if RUGO_VR
    private void LoadVRContainer()
    {
        VRContainer.gameObject.SetActive(true);
        ARContainer.gameObject.SetActive(false);

        // This is needed to let VRTK sdk manager to switch to correct Setup
        VRTK.VRTK_SDKSetup arSetup = VRTK.VRTK_SDKManager.instance.setups[0];
        VRTK.VRTK_SDKManager.instance.setups[0] = VRTK.VRTK_SDKManager.instance.setups[1];
        VRTK.VRTK_SDKManager.instance.setups[1] = arSetup;
    }
#endif

#if RUGO_AR
    private void LoadARContainer()
    {
        ARContainer.gameObject.SetActive(true);
        VRContainer.gameObject.SetActive(false);

        GameObject arTable = new GameObject("Table");
        arTable.transform.SetParent(ARContainer);
        arTable.transform.localPosition = Vector3.zero;
        arTable.transform.localRotation = Quaternion.identity;

        mARTableCollider = arTable.AddComponent<BoxCollider>();
    }
#endif

    // Update is called once per frame
    void Update ()
    {
#if RUGO_AR
        AR_Update();
#elif RUGO_VR
#endif
    }

#if RUGO_AR
    public float TableThickness = 0.01f;

    private GameObject mTrackerA;
    private GameObject mTrackerB;
    
    private BoxCollider mARTableCollider;

    private Vector3 mTrackerAPos = Vector3.zero;
    private Vector3 mTrackerBPos = Vector3.zero;
    private Vector3 mTrackerARot = Vector3.zero;
    private Vector3 mTrackerBRot = Vector3.zero;

    private readonly float epsilon = 0.001f;

    private void AR_Update()
    {
        if (mTrackerA == null)
        {
            mTrackerA = GameObject.Find("TrackerA");
        }

        if(mTrackerB == null)
        {
            mTrackerB = GameObject.Find("TrackerB");
        }

        if (mARTableCollider == null || mTrackerA == null || mTrackerB == null)
            return;

        Vector3 newTrackerAPos = mTrackerA.transform.position;
        Vector3 newTrackerARot = mTrackerA.transform.rotation.eulerAngles;
        Vector3 newTrackerBPos = mTrackerB.transform.position;
        Vector3 newTrackerBRot = mTrackerB.transform.rotation.eulerAngles;

        bool trackerAPosDirtied = (newTrackerAPos - mTrackerAPos).sqrMagnitude > epsilon;
        bool trackerBPosDirtied = (newTrackerBPos - mTrackerBPos).sqrMagnitude > epsilon;
        bool trackerARotDirtied = (newTrackerARot - mTrackerARot).sqrMagnitude > epsilon;
        bool trackerBRotDirtied = (newTrackerBRot - mTrackerBRot).sqrMagnitude > epsilon;

        if (!trackerAPosDirtied && !trackerBPosDirtied && !trackerARotDirtied && !trackerBRotDirtied)
        {
            return;
        }

        mTrackerAPos = newTrackerAPos;
        mTrackerBPos = newTrackerBPos;
        mTrackerARot = newTrackerARot;
        mTrackerBRot = newTrackerBRot;

        
        // Updated Table Position
        Vector3 tableLocation = mTrackerAPos * 0.5f + mTrackerBPos * 0.5f;
        mARTableCollider.transform.position = tableLocation;

        // Updated Table Rotation
        float dirToTrackerA = Vector3.Dot(mARTableCollider.transform.forward, mTrackerA.transform.forward);
        mARTableCollider.transform.rotation = mTrackerA.transform.rotation;
        //mARTableCollider.transform.rotation = Quaternion.Euler(0.0f, 0.0f, mARTableCollider.transform.rotation.eulerAngles.z);

        if (dirToTrackerA < 0.0)
        {
            mARTableCollider.transform.Rotate(mARTableCollider.transform.up, 180.0f);
        }


        // Update Table Dimensions
        Vector3 trackerAInTableSpace = mARTableCollider.transform.worldToLocalMatrix * mTrackerAPos;
        Vector3 trackerBInTableSpace = mARTableCollider.transform.worldToLocalMatrix * mTrackerBPos;

        Vector3 bottomLeft = new Vector3(Mathf.Min(trackerAInTableSpace.x, trackerBInTableSpace.x), Mathf.Min(trackerAInTableSpace.y, trackerBInTableSpace.y), Mathf.Min(trackerAInTableSpace.z, trackerBInTableSpace.z));
        Vector3 topRight = new Vector3(Mathf.Max(trackerAInTableSpace.x, trackerBInTableSpace.x), Mathf.Max(trackerAInTableSpace.y, trackerBInTableSpace.y), Mathf.Max(trackerAInTableSpace.z, trackerBInTableSpace.z));

        Vector3 boxSize = topRight - bottomLeft;
        boxSize.z = TableThickness;
        tableLocation.z = tableLocation.z - boxSize.z * 0.5f;

        mARTableCollider.size = boxSize;
    }
#endif
}
