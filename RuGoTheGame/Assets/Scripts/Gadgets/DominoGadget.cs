using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominoGadget : Gadget
{
	protected AudioSource mAudioData;

    public Material[] DominoColors;

    public static int DEFAULT_COLOR_INDEX = 0;

    public int DominoColorIndex = DEFAULT_COLOR_INDEX;

	new void Awake()
    {
    	base.Awake();
        mAudioData = this.GetComponent<AudioSource>();
    }

    protected override int GetColorInfo()
    {
        return DominoColorIndex;
    }

    public void SetDominoColor(int index)
    {
        DominoColorIndex = index;
        Renderer r = this.GetComponent<Renderer>();
        r.material = DominoColors[index];
    }

    public static int GetRandomColorIndex()
    {
        return Random.Range(0, 4);
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Domino;
    }

    protected override void RestoreColorInfo(int colorInfo)
    {
        SetDominoColor(colorInfo);
    }

    public void SetDominoInWorld(int colorIndex)
    {
        this.SetDominoColor(colorIndex);
        ChangeState(GadgetState.InWorld);
        transform.SetParent(World.Instance.transform);
        MakeSolid();
    }

    void OnCollisionEnter(Collision col)
    {
        // print("\ncalling OnCollisionEnter with " + this.name + " and " + col.gameObject.name);
        Gadget g = col.gameObject.GetComponent<Gadget>();

        if (g != null && g is DominoGadget)
        {
            mAudioData.Play();
        }
    }
}
