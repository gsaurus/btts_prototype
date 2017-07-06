using UnityEngine;
using System.Collections.Generic;



public class ExtraColliders: ColliderTrigger {

	private const float colliderSizeFraction = 0.98f;

	//public int collisionMask = (1 << 10) | 1;

	public float groundHeight			= 1f;
	public float wallWidth				= 3f;
	public float wallHeightExclusion	= 5f;
	public float timeToStabilize		= 0.051f;
	public float landingVelocityThreshold = 3.0f;
	public float stableVelocityMagnitude = 3.0f;

	private Vector3 previousVelocity;
	public bool isVelocityStable {private get; set;}

	private ColliderTrigger groundCheck;
	private ColliderTrigger backWallCheck;
	private ColliderTrigger frontWallCheck;
	private ColliderTrigger farWallCheck;
	private ColliderTrigger nearWallCheck;
	// Collision checkers

	private Animator animator;
	// Reference to the animator

	private void SetupTriggers() {
		
		Collider mainCollider = SetupCollider(false); 
		Vector3 size = new Vector3(mainCollider.bounds.size.x * colliderSizeFraction / transform.localScale.x,
		                           mainCollider.bounds.size.y / transform.localScale.y,
		                           mainCollider.bounds.size.z * colliderSizeFraction / transform.localScale.z
		               );
		Vector3 center = transform.InverseTransformPoint(mainCollider.bounds.center);

		GameObject obj;

		// Setup ground checker
		obj = Utils.CreateChild(transform, "groundChecker");
		groundCheck = obj.AddComponent<ColliderTrigger>();

		if (mainCollider.GetType() == typeof(BoxCollider)) {
			BoxCollider groundCollider = (BoxCollider) groundCheck.AddCollider("BoxCollider", true);
			groundCollider.center = new Vector3(0, center.y -size.y*0.5f, 0);
			groundCollider.size = new Vector3(size.x, groundHeight, size.z);
		}else if (mainCollider.GetType() == typeof(CapsuleCollider)) {
			SphereCollider groundCollider = (SphereCollider) groundCheck.AddCollider("SphereCollider", true);
			groundCollider.radius = size.x * 0.5f;
			groundCollider.center = new Vector3(0, center.y - size.y*0.5f + groundCollider.radius * 0.25f, 0);
		}
		
		// Setup back wall checker
		obj = Utils.CreateChild(transform, "backWallChecker");
		backWallCheck = obj.AddComponent<ColliderTrigger>();
		BoxCollider wallCollider = (BoxCollider) backWallCheck.AddCollider("BoxCollider", true);
		wallCollider.center = new Vector3(-size.x*0.5f, center.y + wallHeightExclusion*0.45f, center.z);
		wallCollider.size = new Vector3(wallWidth, size.y - wallHeightExclusion, size.z);
		
		// Setup front wall checker
		obj = Utils.CreateChild(transform, "frontWallChecker");
		frontWallCheck = obj.AddComponent<ColliderTrigger>();
		wallCollider = (BoxCollider) frontWallCheck.AddCollider("BoxCollider", true);
		wallCollider.center = new Vector3(size.x*0.5f, center.y + wallHeightExclusion*0.45f, center.z);
		wallCollider.size = new Vector3(wallWidth, size.y - wallHeightExclusion, size.z);
		
		// Setup near wall checker
		obj = Utils.CreateChild(transform, "nearWallChecker");
		nearWallCheck = obj.AddComponent<ColliderTrigger>();
		wallCollider = (BoxCollider) nearWallCheck.AddCollider("BoxCollider", true);
		wallCollider.center = new Vector3(center.x, center.y + wallHeightExclusion*0.45f, -size.z*0.5f);
		wallCollider.size = new Vector3(size.x, size.y - wallHeightExclusion, wallWidth);
		
		// Setup far wall checker
		obj = Utils.CreateChild(transform, "farWallChecker");
		farWallCheck = obj.AddComponent<ColliderTrigger>();
		wallCollider = (BoxCollider) farWallCheck.AddCollider("BoxCollider", true);
		wallCollider.center = new Vector3(center.x, center.y + wallHeightExclusion*0.45f, size.z*0.5f);
		wallCollider.size = new Vector3(size.x, size.y - wallHeightExclusion, wallWidth);
		
	}


	void Awake () {
		animator = GetComponent<Animator>();
		SetupTriggers();
	}
	
	void Update() {
		//Debug.Log("Update animator");
		animator.SetBool("isGrounded", IsGrounded());
		animator.SetBool("isHittingFrontWall", IsHittingFrontWall());
		animator.SetBool("isHittingBackWall", IsHittingBackWall());
		animator.SetBool("isHittingNearWall", IsHittingNearWall());
		animator.SetBool("isHittingFarWall", IsHittingFarWall());
		// TODO: tell animator about impact force (wall velocity - char velocity)

		// Hack, crazy fix for a crazy bug: fall animation was getting stuck,
		// ground check was failing due to lack of position update
		GetComponent<Rigidbody>().position = GetComponent<Rigidbody>().position;
	}

	void FixedUpdate(){
		animator.SetBool("landed", Landed());
		isVelocityStable = GetComponent<Rigidbody>().velocity.magnitude < stableVelocityMagnitude && previousVelocity.magnitude < stableVelocityMagnitude;
		previousVelocity = GetComponent<Rigidbody>().velocity;
	}


	public bool Landed(){
		// TODO: check multiple IsTriggered() in short time to fix issues with velocity
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		return info.normalizedTime * info.length > timeToStabilize
			&& IsGrounded()
			&& ((IsTriggered() && GetComponent<Rigidbody>().velocity.y < landingVelocityThreshold) || isVelocityStable)
		;
	}

	
	public bool IsGrounded() {
		return groundCheck.IsTriggered();
	}
	
	public bool IsHittingBackWall() {
		return backWallCheck.IsTriggered();
	}
	
	public bool IsHittingFrontWall() {
		return frontWallCheck.IsTriggered();
	}
	
	public bool IsHittingNearWall() {
		return nearWallCheck.IsTriggered();
	}
	
	public bool IsHittingFarWall() {
		return farWallCheck.IsTriggered();
	}

}
