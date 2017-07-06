using UnityEngine;


public class ComboCounter : MonoBehaviour {
	
	public float coolDown = 0.6f;
	// time needed to reset the combo

	public bool alwaysIncrement = false;
	// If set on, value is always equal to desiredValue

	[HideInInspector]
	public uint value { get; set; }
	// combo value is set when we hit someone, unless alwaysIncrement is true
	
	private uint desiredValue;
	// desired value is what the combo should be once we hit someone
	// only used if alwaysIncrement is false

	private float timer;
	// when timer reaches the cool down, combo value is reset

	// ---- References to other components ----
	
	private Animator animator;
	// Reference to the animator


	void Awake () {
		animator = GetComponent<Animator>();
		ResetCombo();
	}

	void Update() {

		// Doesn't need to be exctremely accurate, so we do this on Update
		if (desiredValue > 0) {
			timer += Time.deltaTime;
			if (timer >= coolDown) {
				ResetCombo();
			}
		}
		
	}

	private void UpdateComboValue(uint newValue) {
		value = newValue;
		animator.SetInteger("combo", (int) value);
	}

	public void ResetCombo() {
		timer = 0;
		desiredValue = 0;
		UpdateComboValue(0);
	}

	public void IncrementCombo() {
		SetCombo(value + 1);
	}

	public void SetCombo(uint comboValue) {
		if (alwaysIncrement) {
			UpdateComboValue(comboValue);
		}else {
			desiredValue = comboValue;
		}
		// reset timer when combo value changes
		timer = 0;
	}

	public void OnComboHit(){
		if (desiredValue > 0) {
			UpdateComboValue(desiredValue);
			timer = 0;
		}
	}

}
