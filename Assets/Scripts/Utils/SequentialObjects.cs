using UnityEngine;
using System.Linq;
using System.Collections.Generic;


//public class SequentialObjects<ObjectType>: MonoBehaviour where ObjectType: MonoBehaviour {
// Generic Monobehaviours are not supported

public class SequentialObjects<ObjectType> where ObjectType: MonoBehaviour {
	// Keep track of a list of objects that contain a certain type of component
	// Objects list is ordered by object name
	// Only one is enabled at a time, and it let you "move" to the next object in the list


	public GameObject[] objects {get; private set;}
	// Objects are sorted by name
	
	private int currentIndex;
	// Current object in use

	

	public bool StartWithAllObjectsInScene(int initialIndex = 0) {
		// Find all objects in scene and sort them by name
		
		// get all objects
		Object[] allObjects;
		allObjects = GameObject.FindObjectsOfType<ObjectType>();
		HashSet<GameObject> allRelevantGameObjects = new HashSet<GameObject>();
		
		foreach (ObjectType typedObj in allObjects) {
			allRelevantGameObjects.Add(typedObj.gameObject);
		}

		currentIndex = initialIndex;
		return StartWithObjects(allRelevantGameObjects.ToArray());

	}



	public bool StartWithAllObjectsInChildren(GameObject parent, int initialIndex = 0) {
		// Find all objects in scene and sort them by name
		
		List<GameObject> allObjects = new List<GameObject>();

		GameObject obj;
		foreach(Transform tr in parent.transform) {
			obj = tr.gameObject;
			if (obj.GetComponent<ObjectType>() != null) {
				allObjects.Add(obj);
			}
		}

		currentIndex = initialIndex;
		return StartWithObjects(allObjects.ToArray());
		
	}


	private void DeactivateAll() {
		if (objects == null) return;

		Activable component;
		foreach (GameObject obj in objects) {
			obj.SetActive(false);
			component = obj.GetComponent<Activable>();
			if (component != null){
				component.DeActivate();
			}
		}
	}


	private bool StartWithObjects(GameObject[] allObjects) {

		DeactivateAll();

		allObjects = allObjects.OrderBy(t=>t.name).ToArray();

		objects = allObjects;

		// disable all
		foreach (GameObject obj in objects) {
			obj.SetActive(false);
		}

		return SetCurrentActive(true);
	}

	
	private bool SetCurrentActive(bool activate){
		if (objects.Count() > currentIndex) {
			objects[currentIndex].SetActive(activate);

			Activable component = objects[currentIndex].GetComponent<Activable>();
			if (component != null) {
				if (activate) component.Activate();
				else 	 	  component.DeActivate();
			}
			return true;
		}
		return false;
	}
	

	
	public bool Next() {
		SetCurrentActive(false);
		++currentIndex;
		return SetCurrentActive(true);
	}
	


	public GameObject GetCurrent() {
		if (objects.Count() > currentIndex) {
			return objects[currentIndex];
		}
		return null;
	}

	public ObjectType GetCurrentComponent() {
		if (objects.Count() > currentIndex) {
			return objects[currentIndex].GetComponent<ObjectType>();
		}
		return null;
	}

	
	public bool SetCurrent(string objName) {
		GameObject obj;
		for (int i = 0 ; i < objects.Count() ; ++i) {
			obj = objects[i];
			if (obj.name == objName){
				SetCurrentActive(false);
				currentIndex = i;
				return SetCurrentActive(true);
			}
		}

		return false;
	}

	public ObjectType[] GetAllComponents() {
		ObjectType[] res = new ObjectType[objects.Count()];
		for (int i = 0 ; i < objects.Count() ; ++i) {
			res[i] = objects[i].GetComponent<ObjectType>();
		}
		return res;
	}

	
}
