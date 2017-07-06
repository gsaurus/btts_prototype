using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandlerComponent : MonoBehaviour {

	public enum CoordinatesFlag {X, Y, Z, XY, XZ, YZ, XYZ, None}

	public const string inputAffectorName = "_inputAffector";
	
	private Animator animator;
	// Reference to the character animator
	
	private InputProvider inputProvider;
	// Reference to the input provider

	private BasicPhysicsComponent physics;
	// Reference to physics component


	//[HideInInspector]
	public bool manualFlip = false;
	// when true, flip must be performed manually
	// when false, flips automatically with velocity
	
	//[HideInInspector]
	public Vector3 inputVelocity = Vector3.zero;
	// Maximum velocity factor allowed from input control


	// TODO: control animation velocity for smooth movements


	void Awake () {

		animator		= GetComponent<Animator>();
		inputProvider	= GetComponent<InputProvider>();
		physics			= GetComponent<BasicPhysicsComponent>();

	}
	
	void Update() {

		// Update animator input parameters
		// We set bools because it can be detected during several frames
		Vector3 inputVel = inputProvider.GetInputMovement();
		animator.SetBool("inputMovement", inputVel.magnitude > 0		);
		animator.SetBool("inputHoldingForward",  Mathf.Abs(inputVel.x) > 0 && physics.isFacingRight == inputVel.x > 0);
		animator.SetBool("inputHoldingBackward", Mathf.Abs(inputVel.x) > 0 && physics.isFacingRight == inputVel.x < 0);
		animator.SetBool("inputHoldingUp",       Mathf.Abs(inputVel.z) > 0 && inputVel.z > 0);
		animator.SetBool("inputHoldingDown",     Mathf.Abs(inputVel.z) > 0 && inputVel.z < 0);
		animator.SetBool("inputAttackA"	, inputProvider.HasInputAttackA()	);
		animator.SetBool("inputAttackB"	, inputProvider.HasInputAttackB()	);
		animator.SetBool("inputAttackC"	, inputProvider.HasInputAttackC()	);
		animator.SetBool("inputJump"	, inputProvider.HasInputJump()		);
		animator.SetBool("inputSpecial"	, inputProvider.HasInputSpecial()	);
		animator.SetBool("inputExtra"	, inputProvider.HasInputExtra()		);
		animator.SetBool("inputDoubleForward"	, physics.isFacingRight ? inputProvider.HasInputDoubleRight() : inputProvider.HasInputDoubleLeft());
		animator.SetBool("inputDoubleBackward"	, physics.isFacingRight ? inputProvider.HasInputDoubleLeft()  : inputProvider.HasInputDoubleRight());
		animator.SetBool("inputDoubleUp"		, inputProvider.HasInputDoubleUp());
		animator.SetBool("inputDoubleDown"		, inputProvider.HasInputDoubleDown());

		animator.SetBool("inputHoldingAttackA"	, inputProvider.IsHoldingInputAttackA()	);
		animator.SetBool("inputHoldingAttackB"	, inputProvider.IsHoldingInputAttackB()	);
		animator.SetBool("inputHoldingAttackC"	, inputProvider.IsHoldingInputAttackC()	);
		animator.SetBool("inputHoldingJump"		, inputProvider.IsHoldingInputJump()	);
		animator.SetBool("inputHoldingSpecial"	, inputProvider.IsHoldingInputSpecial()	);
		animator.SetBool("inputHoldingExtra"	, inputProvider.IsHoldingInputExtra()	);

		// Apply input velocity only after processing input keys
		inputVel = Vector3.zero;
		if (inputVelocity != Vector3.zero) {
			inputVel = inputProvider.GetInputMovement();
			inputVel = ProcessInputVelocity(inputVel);
			if (inputVel.x != 0){
				CheckFlip(inputVel);
			}
		}
		// apply input velocity affector
		physics.SetVelocityAffector(inputAffectorName, inputVel);

	}


//	void LateUpdate () {
//		// Apply input velocity only after processing input keys
//		Vector3 inputVel = Vector3.zero;
//		if (inputVelocity != Vector3.zero) {
//			inputVel = inputProvider.GetInputMovement();
//			inputVel = ProcessInputVelocity(inputVel);
//			if (inputVel.x != 0){
//				CheckFlip(inputVel);
//			}
//		}
//		// apply input velocity affector
//		physics.SetVelocityAffector(inputAffectorName, inputVel);
//		
//	}


	void CheckFlip(Vector3 velocity) {
		if (!manualFlip) {
			// Face to where it's moving to
			if(velocity.x != 0 && (velocity.x > 0 != physics.isFacingRight)) {
				physics.Flip();
			}
		}
	}


	Vector3 ProcessInputVelocity(Vector3 velocity) {
		Vector3 inputVel = velocity;
		inputVel.x *= inputVelocity.x;
		inputVel.y *= inputVelocity.y;
		inputVel.z *= inputVelocity.z;
		return inputVel;
	}

}
