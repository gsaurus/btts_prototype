using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public sealed class AudioKvp : UnityNameValuePair<AudioClip> {
	public AudioClip value = null;
	
	override public AudioClip Value {
		get { return this.value; }
		set { this.value = value; }
	}
	
	public AudioKvp(string key, AudioClip value) : base(key, value) {
	}
}


[System.Serializable]
public class AudioDictionary : UnityDictionary<AudioClip> {
	public List<AudioKvp> values;
	
	override protected List<UnityKeyValuePair<string, AudioClip>> KeyValuePairs {
		get {
			return values.ConvertAll<UnityKeyValuePair<string,AudioClip>>(new System.Converter<AudioKvp, UnityKeyValuePair<string,AudioClip>>(
				x => {
				return x as UnityKeyValuePair<string,AudioClip>;
			}));
		}
		set {
			if (value == null) {
				values = new List<AudioKvp>();
				return;
			}
			
			values = value.ConvertAll<AudioKvp>(new System.Converter<UnityKeyValuePair<string,AudioClip>, AudioKvp>(
				x => {
				return new AudioKvp(x.Key, x.Value as AudioClip);
			}));
		}
	}
	
	override protected void SetKeyValuePair(string k, AudioClip v) {
		var index = values.FindIndex(x => {
			return x.Key == k;});
		
		if (index != -1) {
			if (v == null) {
				values.RemoveAt(index);
				return;
			}
			
			values[index] = new AudioKvp(k, v);
			return;
		}
		
		values.Add(new AudioKvp(k, v));
	}
}