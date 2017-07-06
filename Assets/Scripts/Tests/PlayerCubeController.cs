using UnityEngine;
using System.Collections;

public class PlayerCubeController : MonoBehaviour {

	public float velocity = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float vx = Input.GetAxis("Horizontal");
		float vz = Input.GetAxis("Vertical");
		transform.position += new Vector3(vx*velocity, 0, vz*velocity);

		//ScreenBoundsControl();
	}


	void ScreenBoundsControl() {
		Vector3 newPos = Camera.main.WorldToViewportPoint(transform.position);

		//Debug.Log("Viewpoint coordinates: " + newPos);

		newPos.x = Mathf.Clamp(newPos.x, 0, 1);
		newPos.y = Mathf.Clamp(newPos.y, 0, 1);

		transform.position =  Camera.main.ViewportToWorldPoint(newPos);
	}
}
