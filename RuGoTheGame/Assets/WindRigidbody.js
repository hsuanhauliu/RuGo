/*
WindRigidbody v.1.0 by youtube.com/vulgerstal
How to use
1) Place 'Wind.Rigidbody' prefab in 0,0,0
2) Extract and Use 'Wind.Remote.Control'
*/
private var windStrength : float = 4;
private var radius : float = 1234567;
private var i : int;
var windStrengthMin : float = 0;
var windStrengthMax : float = 5;
var windTransformPosition : Transform;
var windTransformRotation : Transform;
function Update()
{
	if(windTransformPosition != null && windTransformRotation != null)
	{
		windStrength = Random.Range(windStrengthMin, windStrengthMax);
		windTransformRotation.rotation = transform.rotation;

			var hitColliders = Physics.OverlapSphere(windTransformPosition.transform.position, radius);
				for (i = 0; i < hitColliders.Length; i++)
				{
					if(hitColliders[i].GetComponent.<Rigidbody>() != null)
					{
					var hit : RaycastHit;
					var rayDirection = hitColliders[i].GetComponent.<Rigidbody>().gameObject.transform.position - windTransformPosition.transform.position;
						if(Physics.Raycast(windTransformPosition.transform.position, rayDirection, hit)) //there was ',hit, 100' is from an old test.
						{
							if(hit.transform.GetComponent.<Rigidbody>())
							{
								//AddExplosionForce(512, transform.position, radius, 3.0); //More garbage from old tests . . . 
								hitColliders[i].GetComponent.<Rigidbody>().AddForce(windTransformPosition.transform.forward * windStrength,ForceMode.Acceleration);
								//there was '32' instead of windStrength //just a note for myself.
							}
						}	
					}
				}
	}
}