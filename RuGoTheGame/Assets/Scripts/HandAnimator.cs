using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandSide
{
    Left,
    Right
}

public enum Finger
{
    Thumb,
    Index,
    Middle,
    Ring,
    Pinky,
    Num
}

public class HandAnimator : MonoBehaviour
{
    public HandSide Hand;
    public Color HandDefaultColor;
    public Color HandDeleteColor;

    private readonly float INTERP_SPEED = 0.6f;
    private Animator mHandAnimator;
    private Renderer mHandRenderer;

    private float[] mFingerLayerWeights = new float[(int)Finger.Num];

    // Use this for initialization
    void Awake ()
    {
		if(Hand == HandSide.Left)
        {
            // Do we need control reference to update scale of model instead?
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        mHandAnimator = GetComponent<Animator>();
        mHandRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        mHandRenderer.material.SetInt("_ZWrite", 1);
        
        MakeHandIdleAndGhost();
    }
	
	// Update is called once per frame
	void Update ()
    {
        for(int fingerIndex = 0; fingerIndex < (int)Finger.Num; fingerIndex++)
        {
            AnimateFinger(fingerIndex);
        }
    }

    private void AnimateFinger(Finger finger)
    {
        AnimateFinger((int)finger);
    }

    private void AnimateFinger(int fingerIndex)
    {
        int fingerLayer = fingerIndex + 1;

        float currentLayerWeight = mHandAnimator.GetLayerWeight(fingerLayer);
        float newLayerWeight = Mathf.Lerp(currentLayerWeight, mFingerLayerWeights[fingerIndex], INTERP_SPEED); // 0.6f is the Interp speed

        if(!Mathf.Approximately(currentLayerWeight, newLayerWeight))
        {
            mHandAnimator.SetLayerWeight(fingerLayer, newLayerWeight);
        }   
    }

    public void MakeHandIdleAndGhost()
    {
        MakeHandIdle();
        SetHandGhost(true);
    }

    public void MakeHandIdle()
    {
        mFingerLayerWeights[(int)Finger.Index]  = 0.0f;
        mFingerLayerWeights[(int)Finger.Middle] = 0.0f;
        mFingerLayerWeights[(int)Finger.Ring]   = 0.0f;
        mFingerLayerWeights[(int)Finger.Thumb]  = 0.0f;
        mFingerLayerWeights[(int)Finger.Pinky]  = 0.0f;
    }

    public void MakeHandLaser()
    {
        mFingerLayerWeights[(int)Finger.Index]  = 0.0f;
        mFingerLayerWeights[(int)Finger.Middle] = 1.0f;
        mFingerLayerWeights[(int)Finger.Ring]   = 1.0f;
        mFingerLayerWeights[(int)Finger.Thumb]  = 1.0f;
        mFingerLayerWeights[(int)Finger.Pinky]  = 1.0f;
    }

    public void MakeHandGrab()
    {
        mFingerLayerWeights[(int)Finger.Index]  = 1.0f;
        mFingerLayerWeights[(int)Finger.Middle] = 1.0f;
        mFingerLayerWeights[(int)Finger.Ring]   = 1.0f;
        mFingerLayerWeights[(int)Finger.Thumb]  = 1.0f;
        mFingerLayerWeights[(int)Finger.Pinky]  = 1.0f;
    }

    public void MakeHandDelete()
    {
        mFingerLayerWeights[(int)Finger.Index]  = 0.5f;
        mFingerLayerWeights[(int)Finger.Middle] = 0.5f;
        mFingerLayerWeights[(int)Finger.Ring]   = 0.5f;
        mFingerLayerWeights[(int)Finger.Thumb]  = 0.5f;
        mFingerLayerWeights[(int)Finger.Pinky]  = 0.5f;

        SetHandGhost(false, false);
    }

    // Right now we use isDefault as a way to spoof for delete. Should refactor to take into account hand actions.
    public void SetHandGhost(bool isGhost, bool isDefault=true)
    {
        Color currentColor = isDefault ? HandDefaultColor : HandDeleteColor;
        currentColor.a = isGhost ? 0.5f : 1.0f;
        mHandRenderer.material.color = currentColor;
    }
}
