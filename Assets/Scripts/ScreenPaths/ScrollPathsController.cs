using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollPathsController : MonoBehaviour {
	
	private SequentialObjects<ScrollPath> pathObjects = new SequentialObjects<ScrollPath>();
	// All objects containing a path component

	// Singleton
	private static ScrollPathsController __instance;
	public static ScrollPathsController Instance() {
		return __instance;
	}

	void Awake() {
		// initialize singleton
		__instance = this;
	}



	void Start () {
		// Setup path objects
		pathObjects.StartWithAllObjectsInScene();
	}


	public ScrollPath GetScrollPath() {
		return pathObjects.GetCurrentComponent();
	}



	public void NextScrollPath() {
		if (!pathObjects.Next()){
			// All paths iterated, end of the level
			// TODO: end of the level
			Debug.Log("End of the Level!");
		}
	}



	public bool SetScrollPath(string objName) {
		return pathObjects.SetCurrent(objName);
	}


}
