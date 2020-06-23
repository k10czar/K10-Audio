using UnityEngine;
using UnityEditor;
using K10.EditorGUIExtention;
using UnityEditorInternal;

//[CustomPropertyDrawer(typeof( RandomClipFX ) )]
//public class RandomClipFXDrawer : PropertyDrawer
//{
//	protected AudioSourceConfigurationDrawer _config = new AudioSourceConfigurationDrawer();
//	protected ReorderableList _clipsList;
//	protected static float SPACING = EditorGUIUtility.standardVerticalSpacing;
//
//	GUIContent _configLabel = new GUIContent( " Audio Source Configuration" );
//    PersistentValue<bool> Fold( SerializedProperty property ) { return PersistentValue<bool>.At( "Temp/K10/Audio/RandomClipFX/" + property.serializedObject.targetObject.name + property.propertyPath + ".bool" ); }
//
//	protected void UpdateList( SerializedProperty property )
//	{
//		if( _clipsList != null && _clipsList.serializedProperty.serializedObject == property.serializedObject )
//			return;
//
//		var clips = property.FindPropertyRelative( "_clips" );
//		_clipsList = new ReorderableList( property.serializedObject, clips, true, true, true, true );
//
//		_clipsList.drawHeaderCallback = ( Rect rect ) =>
//		{
//			var icon = IconCache.Get( "audioClips" ).Texture;
//			if( icon != null )
//			{
//				GUI.Label( rect.RequestHeight( icon.height ).CutRight( rect.width - icon.width ), icon );
//				rect = rect.CutLeft( icon.width );
//            }
//			GUI.Label( rect, "Audio Clips" );
//		};
//
//		_clipsList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
//		{
//			var element = _clipsList.serializedProperty.GetArrayElementAtIndex( index );
//			GuiLabelWidthManager.New( 50 );
//			EditorGUI.PropertyField( rect.RequestHeight( EditorGUIUtility.singleLineHeight ), element, new GUIContent( "Clip " + index ) );
//			GuiLabelWidthManager.Revert();
//		};
//
//		_clipsList.elementHeight = EditorGUIUtility.singleLineHeight;
//    }
//
//	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
//	{
//		var fold = Fold( property );
//		if( !fold.Get )
//			return EditorGUIUtility.singleLineHeight;
//
//		UpdateList( property );
//		var sh = SeparationLine.HEIGHT;
//		var ch = _config.GetPropertyHeight( property.FindPropertyRelative( "_config" ), _configLabel );
//		return EditorGUIUtility.singleLineHeight + _clipsList.Height() + ch + 2 * sh + SPACING * 4;
//	}
//
//	public override void OnGUI( Rect area, SerializedProperty property, GUIContent label )
//	{
////		var lh = EditorGUIUtility.singleLineHeight;
////		var lhs = lh + SPACING;
//
//		var fold = Fold( property );
//		var open = fold.Get;
//
//		area = EditorGUI.IndentedRect( area );
//
//		EditorGuiIndentManager.New( 0 );
//
//		if( open ) area = SeparationLine.Horizontal( area );
//		area = DrawFold( area, fold, label );
//		if( !open )
//		{
//			EditorGuiIndentManager.Revert();
//			return;
//		}
//
//		area = DrawConfigurations( area, property );
//		area = area.CutTop( 2 * SPACING );
//		area = DrawClips( area, property );
//
//		area = area.CutTop( area.height - SeparationLine.HEIGHT );
//		area = SeparationLine.Horizontal( area );
//
//		EditorGuiIndentManager.Revert();
//	}
//
//	protected Rect DrawFold( Rect area, PersistentValue<bool> fold, GUIContent label )
//	{
//		var lh = EditorGUIUtility.singleLineHeight;
//		var line = area.CutBottom( area.height - lh );
//		fold.Set = EditorGUI.Foldout( line, fold.Get, label );
//		return area.CutTop( lh );
//	}
//
//	protected Rect DrawClips( Rect area, SerializedProperty property )
//	{
//		UpdateList( property );
//		var line = area.CutBottom( area.height - _clipsList.Height() );
//		_clipsList.DoList( line );
//
////		if( GUI.Button( area.CutTop( line.height - 20 ).CutRight( line.width - 50 ), "Test add" ) )
////		{
////			property.arraySize++;
////		}
//
//		return area.CutTop( line.height );
//	}
//
//	protected Rect DrawConfigurations( Rect area, SerializedProperty property )
//	{
//		var configProp = property.FindPropertyRelative( "_config" );
//		var line = area.CutBottom( area.height - _config.GetPropertyHeight( configProp, _configLabel ) );
//		_config.OnGUI( line.CutLeft( 16 ), configProp, _configLabel );
//		return area.CutTop( line.height );
//	}
//}