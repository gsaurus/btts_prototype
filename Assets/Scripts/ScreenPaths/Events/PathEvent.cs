using UnityEngine;
using System.Collections;


public abstract class PathEvent : Activable {

	public delegate void OnEventFinished();

	public OnEventFinished eventFinishedDelagate {private get; set;}

	public virtual void OnDrawGizmosSelected(){
		// Nothing by default
	}

	protected void EventComplete() {
		gameObject.SetActive(false);
		eventFinishedDelagate();
	}
	


}
