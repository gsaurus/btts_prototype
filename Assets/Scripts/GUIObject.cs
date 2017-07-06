using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Visual {
	public GUIStyle style;
	public string icon;
	public string text;
	public Vector3 iconPosition;
	public Vector3 textPosition;
}

public class GUIObject : MonoBehaviour {
	
	public Visual visual;
	
	public List<Vector3> pinPoints = null;

	public virtual void OnDrawGizmos() {
		Gizmos.DrawIcon(transform.position + visual.iconPosition, visual.icon, false);
	}

	public List<Vector3> GetPinPoints() {
		return pinPoints;
	}

	public virtual void ValidatePinPoints() {
		// Nothing by default
	}

}
