using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GUIObject), true)]
[CanEditMultipleObjects]
public class GUIObjectEditor : Editor {

	public void OnSceneGUI() {
		GUIObject guiObj = (GUIObject)target;
		List<Vector3> pinPoints = guiObj.GetPinPoints();
		Vector3 newPin;
		if (pinPoints != null) {
			// Position handle for each pin point
			for (int i = 0 ; i < pinPoints.Count ; ++i) {
				newPin = guiObj.transform.position + pinPoints[i];
				newPin = Handles.PositionHandle(newPin, Quaternion.identity);
				newPin -= guiObj.transform.position;
				pinPoints[i] = newPin;
			}
		}
		if (GUI.changed) {
			guiObj.ValidatePinPoints();
			EditorUtility.SetDirty(target);
		}

		Handles.Label(guiObj.transform.position + guiObj.visual.textPosition, guiObj.visual.text, guiObj.visual.style);
	}

	/*
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		GUIObject guiObj4 = (GUIObject)target;
		List<Vector3> pinPoints = guiObj4.GetPinPoints();
		if (pinPoints != null && pinPoints.Count > 0)
		{
			if (pinPoints.Count == 1) {
				EditorGUILayout.Vector2Field("Pin", (Vector2)pinPoints[0]);
			}
			else {
				for (int i = 0 ; i < pinPoints.Count ; ++i) {
					EditorGUILayout.Vector2Field("Pin " + i, (Vector2)pinPoints[i]);
				}
			}
		}
	}
	*/

}
