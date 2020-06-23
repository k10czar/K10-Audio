using UnityEngine;
using System.Collections;
using UnityEditor;
using K10.EditorGUIExtention;

[CustomPropertyDrawer( typeof( UniqueAudioLoop ) )]
public class UniqueAudioLoopDrawer : PropertyDrawer 
{
	public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) { return EditorGUIUtility.singleLineHeight; }
	public override void OnGUI( Rect area, SerializedProperty property, GUIContent label ) 
	{
		var labelSize = EditorGUIUtility.labelWidth;
		GUI.Label( area.CutRight( area.width - labelSize ), label );
		ScriptableObjectField.Draw<UniqueAudioLoop>( area.CutLeft( labelSize ), property, "Audio/Loops/" + property.serializedObject.targetObject.name + "AudioLoop" );
	}
}
