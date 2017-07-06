using UnityEngine;
using System.Collections;

public class Utils: MonoBehaviour{

	static bool isShuttingDown = false;
	
	public static Object SafeInstantiate(Object original, Vector3 position, Quaternion rotation){
		if (isShuttingDown) return null;
		return Instantiate(original, position, rotation);
	}
	
	void OnApplicationQuit(){
		isShuttingDown = true;
	}


	public static GameObject CreateChild(Transform parent) {
		return CreateChild(parent, "Child (generated)");
	}

	public static GameObject CreateChild(Transform parent, string childName) {
		GameObject child = new GameObject(childName);
		child.layer = parent.gameObject.layer;
		AddChild(parent, child);
		return child;
	}

	public static void AddChild(Transform parent, GameObject child){
		child.transform.parent = parent;
		child.transform.localPosition = Vector3.zero;
		child.transform.localScale = Vector3.one;
		child.transform.localRotation = new Quaternion();
	}

	public static GameObject FindParentWithComponent<T>(GameObject child) where T : Component {
		GameObject obj = child;
		while (obj != null){
			if (obj.GetComponent<T>() != null){
				return obj;
			}
			if (obj.transform != null && obj.transform.parent != null) {
				obj = FindParentWithComponent<T>(obj.transform.parent.gameObject);
			}else{
				obj = null;
			}
		}
		return null;
	}

	public static T FindComponentInParents<T>(GameObject child) where T : Component {
		GameObject obj = child;
		T comp;
		while (obj != null){
			comp = obj.GetComponent<T>();
			if (comp != null){
				return comp;
			}
			if (obj.transform != null && obj.transform.parent != null) {
				obj = obj.transform.parent.gameObject;
			}else{
				obj = null;
			}
		}
		return null;
	}


	public static int BoolToInt(bool b) {
		return b ? 1 : 0;
	}

}
