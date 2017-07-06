using UnityEngine;
using System.Collections.Generic;


public class PathEventParallel: PathEvent {
	// parallel event wait for all children events to finish

	private List<PathEvent> eventComponents;
	// All children event components

	private int numEventsComplete;
	// keep track of how many events finished

	protected override void OnActivate() {
		// Setup event components
		// They are all set active at start
		eventComponents = new List<PathEvent>();
		numEventsComplete = 0;
		PathEvent pathEvent;
		GameObject obj;
		foreach (Transform tr in transform) {
			obj = tr.gameObject;
			pathEvent = obj.GetComponent<PathEvent>();
			if (pathEvent != null) {
				pathEvent.eventFinishedDelagate = OnSubEventComplete;
				obj.SetActive(true);
				pathEvent.Activate();
				eventComponents.Add(pathEvent);
			}
		}
	}
	

	private void OnSubEventComplete() {
		if (++numEventsComplete == eventComponents.Count) {
			// finished all parallel events
			EventComplete();
		}
	}
	


}
