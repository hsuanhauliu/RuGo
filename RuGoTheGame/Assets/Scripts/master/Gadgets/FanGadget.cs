using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanGadget : Gadget
{


    // This mask is used to control objects the wind interacts with, it is set in the editor.
    private LayerMask mLayerMask;
    private AudioSource mAudioData;
    public float windStrengthMin = 5;     public float windStrengthMax = 25;
    private float mWindStrength;
    private Transform blades;
    private float mWindzoneForwardOffset = 0.25f;
    private Vector3 mWindzoneHalfExtents;

     

    private bool mIsFanOn = false;

    new void Start()
    {
        base.Start();
        blades = this.transform.Find("Blades");
        mWindzoneHalfExtents = new Vector3(0.10f, 0.5f, mWindzoneForwardOffset);

        mLayerMask = LayerMask.GetMask(LayerMask.LayerToName(this.gameObject.layer));
        mAudioData = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (mIsFanOn)
        {
            blades.Rotate(new Vector3(0, 0, 45));
            Vector3 windZonePosition = gameObject.transform.position;
            Vector3 fanForward = gameObject.transform.forward * mWindzoneForwardOffset;
            windZonePosition += fanForward;

            //TODO need to change this
            mWindStrength = Random.Range(windStrengthMin, windStrengthMax);

            //TODO Tweak the Wind Zone Overlap Box
            Collider[] hitColliders = Physics.OverlapBox(windZonePosition, mWindzoneHalfExtents, this.transform.rotation, mLayerMask);

            foreach (Collider colliderInWindZone in hitColliders)
            {
                if (colliderInWindZone.GetComponent<Rigidbody>() != null)
                {
                    Debug.DrawRay(colliderInWindZone.GetComponent<Rigidbody>().transform.position, blades.transform.forward * mWindStrength, Color.red);

                    colliderInWindZone.GetComponent<Rigidbody>().AddForce(blades.transform.forward * mWindStrength, ForceMode.Acceleration);
                }
            }
        }
    }

    public override void PerformSwitchAction()
    {
        mIsFanOn = !mIsFanOn;
        if (!mIsFanOn) {
            mAudioData.Stop();
        }
        else {
            mAudioData.Play();
        }
    }

    public override void MakeTransparent()
    {
        base.MakeTransparent();
        mIsFanOn = false;
        if(mAudioData != null) {
            mAudioData.Stop();
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Fan;
    }

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }
}
