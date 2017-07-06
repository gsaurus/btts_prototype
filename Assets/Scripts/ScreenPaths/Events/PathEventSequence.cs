using UnityEngine;
using System.Collections;


public class PathEventSequence: PathEvent {

	private SequentialObjects<PathEvent> eventObjects = new SequentialObjects<PathEvent>();
	// All childs containing an event component

	protected override void OnActivate() {
		// Setup event objects
		eventObjects.StartWithAllObjectsInChildren(gameObject);
		foreach (PathEvent eventComponent in eventObjects.GetAllComponents()) {
			eventComponent.eventFinishedDelagate = OnSubEventComplete;
		}
	}
	
	
	public PathEvent GetCurrentEvent() {
		GameObject eventObj = eventObjects.GetCurrent();
		if (eventObj == null) return null;
		return eventObj.GetComponent<PathEvent>();
	}
	
	
	
	public PathEvent NextEvent() {
		eventObjects.Next();
		return GetCurrentEvent();
	}


	private void OnSubEventComplete() {
		if (!eventObjects.Next()) {
			// finished the whole sequence
			EventComplete();
		}
	}
	


}
