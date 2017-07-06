using UnityEngine;


public class AudioComponent : MonoBehaviour {
	// Keep a dictionary of sounds that can be plaid from animator
	
	public AudioDictionary sounds;
	// Dictionary of sounds identified by a name

	private AudioSource audioSource;
	// Audio source component


	void Awake() {
		audioSource = GetComponent<AudioSource>();

	}


	public void PlaySound(string soundID) {
		if (audioSource == null) return;

		// split argument, perhaps it has a list of random sounds to choose from
		string[] soundParams = soundID.Split(';');

		if (soundParams != null && soundParams.Length > 0) {
			soundID = soundParams[Random.Range(0,soundParams.Length)];
		}
		// Play the requested audio clip
		AudioClip clip;
		if (sounds.TryGetValue(soundID, out clip)) {
			audioSource.PlayOneShot(clip);
			//AudioSource.PlayClipAtPoint(clip, transform.position);
		}
	}

}
