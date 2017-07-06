using UnityEngine;
using System.Collections;


public abstract class Activable : MonoBehaviour {

	public bool isActive {get; private set;}

	public void Activate(){
		if (isActive) return;
		isActive = true;
		OnActivate();
	}


	public void DeActivate(){
		if (!isActive) return;
		isActive = false;
		OnDeactivate();
	}

	protected virtual void OnActivate(){
		// nothing by default
	}

	protected virtual void OnDeactivate() {
		// nothing by default
	}

}
