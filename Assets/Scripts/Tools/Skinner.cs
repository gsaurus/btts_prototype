using UnityEngine;

[ExecuteInEditMode]
public class Skinner : MonoBehaviour {

	public Texture2D texture;
	
	public void SetupSkin(){
		if (texture != null){
			
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			block.SetTexture("_MainTex",texture);
			
			// Change the texture of the sprites
			Renderer[] renderers = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderers) {
				renderer.SetPropertyBlock(block);
			}
		}
	}

//	void Start() {
//		SetupSkin();
//	}

	void Update() {
		// This is not pretty, but will do for the prototype
		SetupSkin();
	}

}
