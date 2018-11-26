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

    private Transform mTrackerA;
    private Transform mTrackerB;
    
    private BoxCollider mARTableCollider;

    private Vector3 mTrackerAPos = Vector3.zero;
    private Vector3 mTrackerBPos = Vector3.zero;

    private readonly float epsilon = 0.001f;

    private void AR_Update()
    {
        if (mTrackerA == null)
        {
            mTrackerA = GameObject.Find("TrackerA").transform;
        }

        if(mTrackerB == null)
        {
            mTrackerB = GameObject.Find("TrackerB").transform;
        }

        if (mARTableCollider == null || mTrackerA == null || mTrackerB == null)
            return;

        Vector3 newTrackerAPos = mTrackerA.position;
        Vector3 newTrackerBPos = mTrackerB.position;

        bool trackerADirtied = (newTrackerAPos - mTrackerAPos).sqrMagnitude > epsilon;
        bool trackerBDirtied = (newTrackerBPos - mTrackerBPos).sqrMagnitude > epsilon;

        if (trackerADirtied || trackerBDirtied)
        {
            mTrackerAPos = newTrackerAPos;
            mTrackerBPos = newTrackerBPos;

            Vector3 bottomLeft = new Vector3(Mathf.Min(mTrackerAPos.x, mTrackerBPos.x), Mathf.Min(mTrackerAPos.y, mTrackerBPos.y), Mathf.Min(mTrackerAPos.z, mTrackerBPos.z));
            Vector3 topRight = new Vector3(Mathf.Max(mTrackerAPos.x, mTrackerBPos.x), Mathf.Max(mTrackerAPos.y, mTrackerBPos.y), Mathf.Max(mTrackerAPos.z, mTrackerBPos.z));

            Vector3 boxSize = topRight - bottomLeft;
            boxSize.y = TableThickness;

            Vector3 tableLocation = bottomLeft * 0.5f + topRight * 0.5f;
            tableLocation.y = tableLocation.y - boxSize.y * 0.5f;

            mARTableCollider.transform.position = tableLocation;
            mARTableCollider.size = boxSize;
        }
    }
#endif
}
