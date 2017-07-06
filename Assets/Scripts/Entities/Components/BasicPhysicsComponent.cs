using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicPhysicsComponent: MonoBehaviour{
	// Simple physics based on a rigid body
	// TODO: an interface or abstract class over this one, for generic physics operations


	public const string impulseAffectorName = "_impulseAffector";

	private const float minWallDistanceFactor = 1.0f;

	// ---- References to other components ----
	
	private Animator animator;
	// Reference to the animator


	// ---- Movement control ----
	
	[HideInInspector]
	public Vector3 velocity = Vector3.zero;
	// Component of velocity that is directly controlled by animator

	private Dictionary<string, Vector3> velocityAffectors = new Dictionary<string, Vector3>();
	// Velocity applied from various sources such as input, wind, treadmills, etc
	// TODO: is a dictionary necessary here?

	public bool isFacingRight { get; private set; }
	// Tell if the character is facing right


	void Awake () {
		isFacingRight = true;
		animator = GetComponent<Animator>();
		GetComponent<Rigidbody>().freezeRotation = true;
	}
	
	void Update() {
		
		// Update animator parameters

	}


	
	void FixedUpdate () {
					
		if (!GetComponent<Rigidbody>().isKinematic) {

			Vector3 finalVelocity = Vector3.zero;

			// Check animator velocity
			Vector3 realAnimatorVelocity = velocity;
			if (!isFacingRight) realAnimatorVelocity.x *= -1;
			finalVelocity += realAnimatorVelocity;

			// Add velocity from extra affectors
			foreach (Vector3 extraVel in velocityAffectors.Values) {
				finalVelocity += extraVel;
			}

			finalVelocity = EnforceVelocityOnWalls(finalVelocity);

			// apply final velocity
			//rigidbody.position += finalVelocity * Time.fixedDeltaTime; // works badly
			//rigidbody.MovePosition(rigidbody.position + finalVelocity * Time.fixedDeltaTime); // works badly too
			transform.position += finalVelocity * Time.fixedDeltaTime; // works fine

			// prevent it from sliding on hills
			RestrictNaturalPhysicsVelocity();

			// Update animator parameters
			finalVelocity += GetComponent<Rigidbody>().velocity;
			// TODO: do this on lateUpdate?
			animator.SetFloat("velX", finalVelocity.x * (isFacingRight ? 1 : -1));
			animator.SetFloat("velY", finalVelocity.y);
			animator.SetFloat("velZ", finalVelocity.z);

		}

	}
	

	public void Flip(){

		isFacingRight = !isFacingRight;

		// Always scale flip main node
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

	}


	public void UseHitDirection(){
		// Ugly indirection because we can't access Hittable methods from animator
		Hittable hittableComponent = GetComponentInChildren<Hittable>();
		if (hittableComponent != null) {
			hittableComponent.UseHitDirection();
		}
	}
	

	Vector3 EnforceVelocityOnWalls(Vector3 velocity){

		ExtraColliders colliders = GetComponent<ExtraColliders>();
		if (colliders == null) return velocity;

		// TODO: take in consideration speed of moving walls
			

		bool isHittingLeftWall = isFacingRight ? colliders.IsHittingBackWall() : colliders.IsHittingFrontWall();
		bool isHittingRightWall = isFacingRight ? colliders.IsHittingFrontWall() : colliders.IsHittingBackWall();
		if (isHittingLeftWall && velocity.x < 0) {
			velocity.x = 0;
		}
		if (isHittingRightWall && velocity.x > 0) {
			velocity.x = 0;
		}
		if (colliders.IsHittingNearWall() && velocity.z < 0) {
			velocity.z = 0;
		}
		if (colliders.IsHittingFarWall() && velocity.z > 0) {
			velocity.z = 0;
		}
		return velocity;
	}


	void RestrictNaturalPhysicsVelocity() {

		ExtraColliders colliders = GetComponent<ExtraColliders>();
		if (colliders == null) return;

		if (colliders.IsGrounded()) {
			// force rigidbody x and z velocity to avoid sliding on hills
			GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
		}
	}
	

	public void AddImpulse(Vector3 impulse) {
		if (!GetComponent<Rigidbody>().isKinematic) {
			Vector3 finalImpulse = GetVelocityAffector(impulseAffectorName);
			finalImpulse += impulse;
			SetVelocityAffector(impulseAffectorName, finalImpulse);
		}
	}

	public Vector3 GetVelocityAffector(string affectorID) {
		Vector3 outVec;
		if (velocityAffectors.TryGetValue(affectorID, out outVec)) {
			return outVec;
		}
		return Vector3.zero;
	}
	
	public void SetVelocityAffector(string affectorID, Vector3 velocity) {
		velocityAffectors[affectorID] = velocity;
	}
	
	public void RemoveVelocityAffector(string affectorID) {
		velocityAffectors.Remove(affectorID);
	}
	
	
	// ------------------------------
	// Events to be used in Animator


	public void AddImpulseX(float x) {
		AddImpulse(new Vector3(isFacingRight ? x : -x, 0, 0));
	}

	public void AddImpulseY(float y) {
		// Y input directly applied on velocity because it's affected by gravity
		GetComponent<Rigidbody>().velocity += new Vector3(0, y, 0);
	}

	public void AddImpulseZ(float z) {
		AddImpulse(new Vector3(0, 0, z));
	}


	public void ClearImpulseXZ() {
		RemoveVelocityAffector(impulseAffectorName);
	}


	public void ResetVelocity(){
		ClearImpulseXZ();
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}
	
	
}
