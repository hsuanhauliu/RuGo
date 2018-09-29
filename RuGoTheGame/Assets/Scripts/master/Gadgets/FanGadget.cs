using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanGadget : Gadget {

    protected override List<Renderer> GetRenderers()
    {
        List<Renderer> renderers = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
        return renderers;
    }

    private Transform blades;
    private Transform wind;     private float windStrength = 10;     public float radius = 1;     private int i;     public float windStrengthMin = 5;     public float windStrengthMax = 25;     public Transform windTransformPosition;     public Transform windTransformRotation;

    void Start()
    {
        blades = this.transform.Find("Blades");
    }

    void Update()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        if (Input.GetKey(KeyCode.L))
        {
            blades.Rotate(new Vector3(0, 0, 45));
            if (windTransformPosition != null && windTransformRotation != null)             {                 RaycastHit hit;                 windStrength = Random.Range(windStrengthMin, windStrengthMax);                 windTransformRotation.rotation = transform.rotation;                  var hitColliders = Physics.OverlapSphere(windTransformPosition.transform.position, radius);                 for (i = 0; i < hitColliders.Length; i++)                 {                     if (hitColliders[i].GetComponent<Rigidbody>() != null)                     {                          var rayDirection = hitColliders[i].GetComponent<Rigidbody>().gameObject.transform.position - windTransformPosition.transform.position;                         if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))                          {                             if (hit.transform.GetComponent<Rigidbody>())                             {                                  hitColliders[i].GetComponent<Rigidbody>().AddForce(windTransformPosition.transform.forward * windStrength, ForceMode.Acceleration);                              }                         }                     }                 }             } 
        }
    }

    public override GadgetInventory GetGadgetType()
    {
        return GadgetInventory.Fan;
    }
}
