
private var windStrength : float = 4;
public var radius : float = 2;
private var i : int;
var windStrengthMin : float = 10;
var windStrengthMax : float = 30;
var windTransformPosition : Transform;
var windTransformRotation : Transform;
function Update()
{
	/*if(windTransformPosition != null && windTransformRotation != null)
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
						if(Physics.Raycast(windTransformPosition.transform.position, rayDirection, hit)) 
						{
							if(hit.transform.GetComponent.<Rigidbody>())
							{

								hitColliders[i].GetComponent.<Rigidbody>().AddForce(windTransformPosition.transform.forward * windStrength,ForceMode.Acceleration);

							}
						}	
					}
				}
	}*/
}