using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor( typeof(MonoBehaviour), true )]
public class MonoBehaviourWithPropertiesEditor : Editor {
	
	
	MonoBehaviour m_Instance;
	PropertyField[] m_fields;
	
	
	public void OnEnable()
	{
		m_Instance = target as MonoBehaviour;
		m_fields = ExposeProperties.GetProperties( m_Instance );
	}
	
	public override void OnInspectorGUI () {
		
		if ( m_Instance == null )
			return;
		
		this.DrawDefaultInspector();
		
		ExposeProperties.Expose( m_fields );
		
	}
}