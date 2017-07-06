using UnityEngine;


public class Hittable : MonoBehaviour {

	// Note: disable hittable when invincible.

	private Animator parentAnimator;
//	private ExtraColliders parentColliders;
	private BasicPhysicsComponent parentPhysics;
	private GameObject parent;
	private EntityData parentEntityData;

	private HitFaceDirectionType directionWhenHit = HitFaceDirectionType.none;
	private GameObject colliderObj;

	void Awake () {
		parent = Utils.FindParentWithComponent<Animator>(gameObject);
		if (parent != null){
			parentAnimator = parent.GetComponent<Animator>();
//			parentColliders = parent.GetComponent<ExtraColliders>();
			parentPhysics = parent.GetComponent<BasicPhysicsComponent>();
			parentEntityData = parent.GetComponent<EntityData>();
		}
	}


	public void UseHitDirection(){
		// only apply direction after all updates so we won't flip again
		// between hit and start of hit animation
		// also wait for trigger to be consumed
		if (directionWhenHit != HitFaceDirectionType.none
		    && parentPhysics != null
		    && colliderObj != null
		) {

//			Hitter theHitter = colliderObj.GetComponent<Hitter>();
			GameObject hitterParent = Utils.FindParentWithComponent<Animator>(colliderObj);
			BasicPhysicsComponent colliderPhysics = hitterParent.GetComponent<BasicPhysicsComponent>();
			bool hitterIsFacingRight = colliderPhysics != null ? colliderPhysics.isFacingRight : false;
			switch (directionWhenHit){
				case HitFaceDirectionType.useHitterDirection:{
				if (parentPhysics.isFacingRight != hitterIsFacingRight)
						parentPhysics.Flip();
				}break;
				case HitFaceDirectionType.useOppositeHitterDirection:{
				if (parentPhysics.isFacingRight == hitterIsFacingRight)
						parentPhysics.Flip();
				}break;
				case HitFaceDirectionType.faceToHitter:{
					if (parentPhysics.isFacingRight != (parent.transform.position.x < colliderObj.transform.position.x))
						parentPhysics.Flip();
				}break;
				case HitFaceDirectionType.faceOppositeToHitter:{
					if (parentPhysics.isFacingRight != (parent.transform.position.x > colliderObj.transform.position.x))
						parentPhysics.Flip();
				}break;
				default:{
					// Nothing by default
				}break;
			}
			colliderObj = null;
			directionWhenHit = HitFaceDirectionType.none;
		}
	}

	void OnTriggerEnter(Collider otherCollider) {
		colliderObj = otherCollider.gameObject;
		if (colliderObj != null){
			Hitter theHitter = colliderObj.GetComponent<Hitter>();
			GameObject hitterParent = Utils.FindParentWithComponent<Animator>(colliderObj);
			//BasicPhysicsComponent colliderPhysics = hitterParent.GetComponent<BasicPhysicsComponent>();
			EntityData hitterData = hitterParent.GetComponent<EntityData>();

			float damageMultiplier = 1.0f;

			if (parentEntityData != null && hitterData != null && hitterData.team == parentEntityData.team) {
				damageMultiplier = EntitiesManager.Instance().teamsFriendlyFire[hitterData.team];
				if (damageMultiplier < 0.0001f) {
					// Friendly fire 100%, ignore collision
					return;
				}
			}

			// Check were we should face at
			// store that info here and use on late update
			directionWhenHit = theHitter.GetHitFaceDirectionType();
//			if (parentPhysics != null) {
//				switch (theHitter.GetHitFaceDirectionType()){
//					case HitFaceDirectionType.useHitterDirection:{
//					if (colliderPhysics != null && parentPhysics.isFacingRight != colliderPhysics.isFacingRight)
//						parentPhysics.Flip();
//					}break;
//					case HitFaceDirectionType.useOppositeHitterDirection:{
//						if (colliderPhysics != null && parentPhysics.isFacingRight == colliderPhysics.isFacingRight)
//							parentPhysics.Flip();
//					}break;
//					case HitFaceDirectionType.faceToHitter:{
//						if (parentPhysics.isFacingRight != (parent.transform.position.x < colliderObj.transform.position.x))
//							parentPhysics.Flip();
//					}break;
//					case HitFaceDirectionType.faceOppositeToHitter:{
//						if (parentPhysics.isFacingRight != (parent.transform.position.x > colliderObj.transform.position.x))
//							parentPhysics.Flip();
//					}break;
//					default:{
//						// Nothing by default
//					}break;
//				}
//			}


			// notify animator about the hit
			switch (theHitter.GetHitType()){
				case HitType.knockOut:{
					parentAnimator.SetTrigger("knocked down");
				}break;
				case HitType.normal:{
					parentAnimator.SetTrigger("got hit");
				}break;
				default:{
					parentAnimator.SetTrigger("got hit");
				}break;
			}

			// Apply the damage
			if (parentEntityData != null) {
				parentEntityData.energy -= theHitter.damage * damageMultiplier;
			}

			SimpleHud.Instance().OnDirectInteraction(parentEntityData, hitterData);

			// tell hitter that he got a sucessful hit
			theHitter.OnSuccessfulHit();
		}
	}

}
