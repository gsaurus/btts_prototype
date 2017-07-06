using UnityEngine;


public class CameraBoundsControl : MonoBehaviour {

	public float extraSideBorder = 0.025f;
	public float extraTopBorder = 0.08f;
	public float extraBottomBorder = 0.0f;



	void Update() {

		Vector3 newPos = Camera.main.WorldToViewportPoint(transform.position);
		
		newPos.x = Mathf.Clamp(newPos.x, extraSideBorder, 1-extraSideBorder);
		newPos.y = Mathf.Clamp(newPos.y, extraBottomBorder, 1-extraTopBorder);

		newPos = Camera.main.ViewportToWorldPoint(newPos);
		newPos.y = transform.position.y;

		// only enforce Z if it's grounded (avoid weird z repositionings while jumping in top edge)
		ExtraColliders colliders = GetComponent<ExtraColliders>();
		if (colliders == null || !colliders.IsGrounded()) {
			newPos.z = transform.position.z;
		}

		transform.position = newPos;
	}

}
