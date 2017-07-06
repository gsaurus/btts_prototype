using UnityEngine;
using System.Collections;


public enum HitType{
	normal = 0,
	knockOut = 1
	//...
};


public enum HitFaceDirectionType {
	none = 0,
	faceToHitter = 1,
	faceOppositeToHitter = 2,
	useHitterDirection = 3,
	useOppositeHitterDirection = 4
	// ...
}

public class Hitter: MonoBehaviour {
	
	// TODO: use a delegate to notify on collisions
	// Example: on special cause self damage if hit is detected

	public float hitDelay = 0.1f;
	// How long the animation should pause when hitting someone

	public float damage = 10.0f;
	// How much damage to cause in the opponent
	// TODO: right now unity doesn't support int as animatable so we use float

	public float hitTypeIndex = (float)HitType.normal;
	// What kind of effect it causes in the opponent
	// Examples: normal hit, knock out, burn, etc
	// TODO: right now unity doesn't support custom animatable properties so we use float

	public float hitFaceDirectionIndex = (float)HitFaceDirectionType.none;
	// What direction the opponent looks at when hit
	// TODO: right now unity doesn't support custom animatable properties so we use float



	public string[] onSuccessSounds;
	// This contains the possible success sounds to be plaid by the hitter owner
	// TODO: this is an ugly workaround because of unity lack of string animatable properties


	public float onSuccessSoundIndex = 0;
	// Sound to play if the hittable is sucessfully hit
	// Note: hittable may be blocking and decide to play a different sound instead
	// TODO: right now unity doesn't support string animatable properties so we use float as indexes to an array...

	private Animator parentAnimator;
	private AudioComponent parentAudio;
	private Rigidbody parentBody;
	private ComboCounter comboCounter;
	private EntityData parentEntityData;


	private Vector3 previousPhysicsVelocity;
	private bool previousRigidbodyIsKinematic;
	

	void Awake () {
		GameObject obj = Utils.FindParentWithComponent<Animator>(gameObject);
		if (obj != null){
			parentAnimator = obj.GetComponent<Animator>();
			parentAudio = obj.GetComponent<AudioComponent>();
			parentBody = obj.GetComponent<Rigidbody>();
			comboCounter = obj.GetComponent<ComboCounter>();
			parentEntityData = obj.GetComponent<EntityData>();
		}
	}


	void OnEnable() {
		// Nothing for now
	}


	IEnumerator DelayAnimation(){

		if (parentBody == null) yield break;

		// Stop it
		if (parentAnimator.enabled) {
			// hack because of possible velocity reset from animator?..
			previousPhysicsVelocity = parentBody.velocity;
			previousRigidbodyIsKinematic = parentBody.isKinematic;
			parentAnimator.enabled = false;
			parentBody.isKinematic = true;
		}

		// wait...
		yield return new WaitForSeconds(hitDelay);

		// restore state
		if (!parentAnimator.enabled){
			parentAnimator.enabled = true;
			parentBody.isKinematic = previousRigidbodyIsKinematic;
			parentBody.velocity = previousPhysicsVelocity;
		}

	}

	void OnTriggerEnter(Collider otherCollider) {

		GameObject colliderObj = otherCollider.gameObject;
		if (colliderObj != null){
			GameObject hitterParent = Utils.FindParentWithComponent<Animator>(colliderObj);
			EntityData hitterData = hitterParent.GetComponent<EntityData>();

			if (parentEntityData != null && hitterData != null && hitterData.team == parentEntityData.team) {
				float damageMultiplier = EntitiesManager.Instance().teamsFriendlyFire[hitterData.team];
				if (damageMultiplier < 0.0001f) {
					// Friendly fire 100%, ignore collision
					return;
				}
			}
		}

		StopCoroutine(DelayAnimation());
		StartCoroutine(DelayAnimation());

	}

	public HitType GetHitType(){
		return (HitType) hitTypeIndex;
	}


	public HitFaceDirectionType GetHitFaceDirectionType(){
		return (HitFaceDirectionType) hitFaceDirectionIndex;
	}


	public void OnSuccessfulHit(){
		if (parentAudio != null) {
			int soundIndex = (int) onSuccessSoundIndex;
			if (soundIndex >= 0 && soundIndex < onSuccessSounds.Length) {
				parentAudio.PlaySound(onSuccessSounds[soundIndex]);
			}
		}

		if (comboCounter != null) {
			comboCounter.OnComboHit();
		}
	}

}
