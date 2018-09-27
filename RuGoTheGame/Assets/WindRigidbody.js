
private var windStrength : float = 10;
public var radius : float = 1;
private var i : int;
var windStrengthMin : float = 5;
var windStrengthMax : float = 25;
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

								hitColliders[i].GetComponent.<Rigidbody>().AddForce(windTransformPosition.transform.forward * windStrength,ForceMode.Acceleration);

							}
						}	
					}
				}
	}
}