using UnityEngine;
using System.Collections.Generic;

public class ColliderTrigger : MonoBehaviour {

	//private int layerMask;
	
	private HashSet<GameObject> triggeredObjects = new HashSet<GameObject>();
	// Keep track of all contacting objects

	// Helper setup methods
	public Collider AddCollider(string colliderType, bool isTrigger) {
		//this.layerMask = mask;
		Collider collider;
		// Note: following line became obsolete, now replaced with an hardcoded if statement just to fix it
		//collider = gameObject.AddComponent<ColliderTrigger>( //(Collider) UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Scripts/Utils/ColliderTrigger.cs (14,34)", colliderType);

		if (colliderType == "BoxCollider") {
				collider = gameObject.AddComponent<BoxCollider> ();
		} else if (colliderType == "SphereCollider") {
				collider = gameObject.AddComponent<SphereCollider> ();
		} else {
			UnityEngine.Debug.LogError("Unsupported collider type " + colliderType);
			return null;
		}
		collider.enabled = true;
		collider.isTrigger = isTrigger;
		return collider;
	}
	public Collider SetupCollider(bool isTrigger) {
		//this.layerMask = mask;
		Collider collider = (Collider) gameObject.GetComponent("Collider");
		collider.enabled = true;
		collider.isTrigger = isTrigger;
		return collider;
	}
	


	void LateUpdate() {
		VerifyDestroyedCollidingObjects();
	}

//	bool CheckObjectMask(GameObject obj) {
//		return ((1 << obj.layer) & layerMask) != 0;
//	}

	void OnTriggerEnter(Collider otherCollider) {
		GameObject obj = otherCollider.gameObject;
		triggeredObjects.Add(obj);
	}
	
	void OnTriggerExit(Collider otherCollider) {
		GameObject obj = otherCollider.gameObject;
		triggeredObjects.Remove(obj);
	}


	void OnCollisionEnter(Collision collision) {
		GameObject obj = collision.collider.gameObject;
		triggeredObjects.Add(obj);
	}
	
	void OnCollisionExit(Collision collision) {
		GameObject obj = collision.collider.gameObject;
		triggeredObjects.Remove(obj);
	}


	void VerifyDestroyedCollidingObjects() {
		List<GameObject> toRemove = new List<GameObject>();
		foreach (GameObject obj in triggeredObjects) {
			if (obj == null) {
				// This object was destroyed
				toRemove.Add(obj);
			}
		}
		foreach (GameObject obj in toRemove) {
			triggeredObjects.Remove(obj);
		}
	}


	public bool IsTriggered() {
		return triggeredObjects.Count > 0;
	}
	
}
