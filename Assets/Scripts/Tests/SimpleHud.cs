using UnityEngine;
using System.Collections;

public class SimpleHud : MonoBehaviour {

	public GameObject myPlayer;

	public float hpAnimationSpeed = 0.05f;
	public float maxEnergy = 500;
	public GameObject playerHP;
	public GameObject enemyHP;

	private float playerHPMaxWidth;
	private float enemyHPMaxWidth;
	private float playerHPInitialPositionX;
	private float enemyHPInitialPositionX;

	private float currentPlayerHP;
	private float currentEnemyHP;
	private float targetPlayerHP;
	private float targetEnemyHP;
	private EntityData currentEnemyData;

	// Singleton
	private static SimpleHud __instance;
	public static SimpleHud Instance() {
		return __instance;
	}
	
	void Awake() {
		// initialize singleton
		__instance = this;

		playerHPMaxWidth = playerHP.transform.localScale.x;
		enemyHPMaxWidth = enemyHP.transform.localScale.x;
		playerHPInitialPositionX = playerHP.transform.localPosition.x;
		enemyHPInitialPositionX = enemyHP.transform.localPosition.x;
		enemyHP.SetActive(false);

		OnDirectInteraction(myPlayer.GetComponent<EntityData>(), null);
	}



	void Update() {
		float speed = hpAnimationSpeed * maxEnergy;
		if (currentPlayerHP != targetPlayerHP) {
			if (currentPlayerHP > targetPlayerHP + speed) {
				currentPlayerHP -= speed;
			}else if (currentPlayerHP < targetPlayerHP - speed) {
				currentPlayerHP += speed;
			}else {
				currentPlayerHP = targetPlayerHP;
			}

			float newPlayerScale = playerHPMaxWidth * currentPlayerHP / maxEnergy;
			if (newPlayerScale > 0) {
				playerHP.SetActive(true);
				playerHP.transform.localScale = new Vector3(newPlayerScale, playerHP.transform.localScale.y, playerHP.transform.localScale.z);
				playerHP.transform.localPosition = new Vector3(playerHPInitialPositionX - playerHPMaxWidth*0.5f + newPlayerScale*0.5f,
				                                               playerHP.transform.localPosition.y,
				                                               playerHP.transform.localPosition.z
				                                               );
			}else {
				playerHP.SetActive(false);
			}
		}

		if (currentEnemyData != null && currentEnemyHP != targetEnemyHP) {
			if (currentEnemyHP > targetEnemyHP + speed) {
				currentEnemyHP -= speed;
			}else if (currentPlayerHP < targetEnemyHP - speed) {
				currentEnemyHP += speed;
			}else {
				currentEnemyHP= targetEnemyHP;
			}
			
			float newEnemyScale = currentEnemyData == null ? 0 : enemyHPMaxWidth * currentEnemyHP / maxEnergy;
			if (newEnemyScale > 0) {
				enemyHP.SetActive(true);
				enemyHP.transform.localScale = new Vector3(newEnemyScale, playerHP.transform.localScale.y, playerHP.transform.localScale.z);
				enemyHP.transform.localPosition = new Vector3(enemyHPInitialPositionX - enemyHPMaxWidth*0.5f + newEnemyScale*0.5f,
				                                              enemyHP.transform.localPosition.y,
				                                              enemyHP.transform.localPosition.z
				                                              );
			}else {
				enemyHP.SetActive(false);
			}
		}else if (currentEnemyData == null){
			enemyHP.SetActive(false);
		}

	}




	public void OnDirectInteraction(EntityData entityOne, EntityData entityTwo) {
		if ((entityOne != null && entityOne.gameObject == myPlayer) || (entityTwo != null && entityTwo.gameObject == myPlayer)) {
			// Player 1 is involved, update our simple HUD
			EntityData playerData = entityOne != null && entityOne.gameObject == myPlayer ? entityOne : entityTwo;
			EntityData enemyData = entityOne != null && entityOne.gameObject == myPlayer ? entityTwo : entityOne;

			targetPlayerHP = playerData != null ? playerData.energy : 0;
			targetEnemyHP = enemyData != null ? enemyData.energy : 0;
			if (enemyData != currentEnemyData) {
				currentEnemyData = enemyData;
				currentEnemyHP = enemyData.energy + 0.01f;
			}
		}
	}



}
