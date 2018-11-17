using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanGadget : Gadget
{
    // This mask is used to control objects the wind interacts with, it is set in the editor.
    private LayerMask mLayerMask;
    private AudioSource mAudioData;

    public float WindZoneRadius = 1.0f;
    public float WindZoneConeHalfAngle = 30.0f;
    public float WindZoneOffset = -0.5f;
    public float WindStrengthMin = 5.0f;     public float WindStrengthMax = 25.0f;
    public float FanSpeed = 5.0f; 

    // Swivel and Blades
    public Transform Blades;
    public Transform Swivel;
    public GameObject AffectVisual;

    public bool ForceFanToggle = false;
    
    private bool mIsFanOn = false;
    private float mCurFanSpeed = 0.0f;
    private float mTargetFanSpeed = 0.0f;
    private Vector3 mOffset = Vector3.zero;
    private float mThreshold = 0.0f;

    new void Start()
    {
        base.Start();

        mLayerMask = LayerMask.GetMask(GadgetLayers.INWORLD);
        mAudioData = GetComponent<AudioSource>();

        mCurFanSpeed = 0.0f;
        mTargetFanSpeed = 0.0f;
        mOffset.x = WindZoneOffset;

        mThreshold = Mathf.Cos(WindZoneConeHalfAngle * Mathf.Deg2Rad);

        ConeHelper coneHelper = AffectVisual.GetComponent<ConeHelper>();
        coneHelper.Length = WindZoneRadius;
        coneHelper.ConeHalfAngle = WindZoneConeHalfAngle;
        AffectVisual.transform.localPosition = mOffset;
        AffectVisual.SetActive(false);
    }

    new void Update()
    {
        base.Update();

        // DEBUG ONLY
        if(ForceFanToggle)
        {
            PerformSwitchAction();
            ForceFanToggle = false;
        }

        Vector3 fanForward = Blades.transform.right;
        mCurFanSpeed = Mathf.Lerp(mCurFanSpeed, mTargetFanSpeed, Time.deltaTime);
        Blades.Rotate(Vector3.right, mCurFanSpeed);

        if (mIsFanOn)
        {
            Vector3 windZonePosition = Blades.transform.position + mOffset;
            
            Collider[] hitColliders = Physics.OverlapSphere(windZonePosition, WindZoneRadius, mLayerMask);

            foreach (Collider colliderInWindZone in hitColliders)
            {
                if (colliderInWindZone.GetComponent<Rigidbody>() != null)
                {
                    if(colliderInWindZone.GetComponentInParent<Gadget>().transform != this.transform)
                    {
                        Vector3 directionToCollider = colliderInWindZone.transform.position - windZonePosition;
                        float affect = directionToCollider.magnitude;
                        directionToCollider.Normalize();

                        float dotToCollider = Vector3.Dot(fanForward, directionToCollider);

                        if (dotToCollider > mThreshold)
                        {
                            affect = affect / WindZoneRadius;
                            affect = affect * dotToCollider;
                            float windStrength = Mathf.Lerp(WindStrengthMin, WindStrengthMax, affect);
                            colliderInWindZone.GetComponent<Rigidbody>().AddForce(fanForward * windStrength, ForceMode.Acceleration);
                        }
                    }
                }
            }
        }
    }
    
    public override void PerformSwitchAction()
    {
        if(CurrentGadgetState == GadgetState.InWorld)
        {
            mIsFanOn = !mIsFanOn;
            if (!mIsFanOn)
            {
                mTargetFanSpeed = 0.0f;
                mAudioData.Stop();
            }
            else
            {
                mTargetFanSpeed = FanSpeed;
                mAudioData.Play();
            }
        }
    }

    public override void MakeSolid()
    {
        base.MakeSolid();
        AffectVisual.SetActive(false);
    }

    public override void MakeTransparent(bool keepCollision = false)
    {
        base.MakeTransparent(keepCollision);
        if (mIsFanOn)
        {
            PerformSwitchAction();
        }

        if (CurrentGadgetState == GadgetState.InWorld || CurrentGadgetState == GadgetState.FirstPlacement) 
        {
            AffectVisual.SetActive(true);
        }
        else
        {
            AffectVisual.SetActive(false);
        }
        
        if (mAudioData != null) {
            mAudioData.Stop();
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Fan;
    }
}
