using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ScrollOptions {
	public bool lockLeft;
	public bool lockRight;
	public bool lockDown;
	public bool lockUp;
	public bool lockNear;
	public bool lockFar;
	public bool catchX = true;
	public bool catchY = true;
	public bool catchZ = true;
}



public abstract class ScrollPath : GUIObject {

	public ScrollOptions scrollOptions;
	// Flags telling wich coordinates the scroll path must follow
	
	protected Vector3 previousPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	// previous point used to check scroll direction
	

	public virtual void OnValidate() {
		// Called when something changes in Inspector
		visual.text = name;
	}

	public virtual void OnDrawGizmosSelected(){
		// Nothing by default
	}


	protected bool IsThereAPreviousLimitPoint() {
		return previousPoint != new Vector3(float.MinValue, float.MinValue, float.MinValue);
	}


	public virtual Vector3 EnforceLimits(Vector3 point) {
		if (IsThereAPreviousLimitPoint()) {
			if (scrollOptions.lockLeft		&& point.x < previousPoint.x) point.x = previousPoint.x;
			if (scrollOptions.lockRight	&& point.x > previousPoint.x) point.x = previousPoint.x;
			if (scrollOptions.lockDown		&& point.y < previousPoint.y) point.y = previousPoint.y;
			if (scrollOptions.lockUp		&& point.y > previousPoint.y) point.y = previousPoint.y;
			if (scrollOptions.lockNear		&& point.z < previousPoint.z) point.z = previousPoint.z;
			if (scrollOptions.lockFar		&& point.z > previousPoint.z) point.z = previousPoint.z;
		}
		previousPoint = point;
		return point;
	}


	void Start() {

		// If there's a path event attached, setup it's delegate
		PathEvent pathEvent = GetComponent<PathEvent>();
		if (pathEvent != null) {
			pathEvent.eventFinishedDelagate = ScrollPathsController.Instance().NextScrollPath;
		}
	}


}
