using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScrollMargin {
	public float left;
	public float right;
	public float bottom;
	public float top;
	public float near;
	public float far;

	public ScrollMargin(float l, float r, float b, float t, float n, float f) {
		left = l; right = r; top = t; bottom = b; far = f; near = n;
	}
}

public class CameraController : MonoBehaviour 
{
	public ScrollMargin margin = new ScrollMargin(-1,1,-1,1,-1,1);
	// How much the character have to move in each direction before start scrolling

	public Vector3 stiffness = new Vector3(0.05f,0.05f,0.05f);
	// How fast scroll accompains character
	
	public Transform target;
	// Reference to the scroll target's transform.
	
	
	private bool CheckMargin(float originalPosition, float newPosition, float margin, bool greaterThan){
		// Returns true if the distance between the camera and the Target is greater than the margin.

		if (greaterThan){
			return newPosition - originalPosition > margin;
		}else{
			return newPosition - originalPosition < margin;
		}

	}
	
	
	void Update (){
		TrackTarget();
	}
	
	
	void TrackTarget(){

		if (target == null) return; // No movement if target doesn't exist

		// if margins test fails, final position is the same as initial
		Vector3 finalPosition = transform.position;

		// Set target position to target coordinates
		Vector3 targetPosition = target.position;

		// Check active ScrollPath limits
		ScrollPath activePath = ScrollPathsController.Instance().GetScrollPath();
		if (activePath != null) {
			targetPosition = activePath.EnforceLimits(targetPosition);
		}
		
		// Check margins and apply lerp for smooth movement
		if(CheckMargin(transform.position.x, targetPosition.x, margin.left, false) || CheckMargin(transform.position.x, targetPosition.x, margin.right, true))
			finalPosition.x = Mathf.Lerp(transform.position.x, targetPosition.x, Mathf.Abs(transform.position.x - targetPosition.x) * stiffness.x * Time.deltaTime);
		if(CheckMargin(transform.position.y, targetPosition.y, margin.bottom, false) || CheckMargin(transform.position.y, targetPosition.y, margin.top, true))
			finalPosition.y = Mathf.Lerp(transform.position.y, targetPosition.y, Mathf.Abs(transform.position.y - targetPosition.y) * stiffness.y * Time.deltaTime);
		if(CheckMargin(transform.position.z, targetPosition.z, margin.near, false) || CheckMargin(transform.position.z, targetPosition.z, margin.far, true))
			finalPosition.z = Mathf.Lerp(transform.position.z, targetPosition.z, Mathf.Abs(transform.position.z - targetPosition.z) * stiffness.z * Time.deltaTime);

		transform.position = finalPosition;

	}
}
