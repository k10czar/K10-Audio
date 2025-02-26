using UnityEngine;
using UnityEditor;
using K10.EditorGUIExtention;

[CustomPropertyDrawer( typeof( AudioSourceConfiguration ) )]
public class AudioSourceConfigurationDrawer : PropertyDrawer
{
	static float SPACING = EditorGUIUtility.standardVerticalSpacing;

	PersistentValue<bool> Fold( SerializedProperty property ) { return PersistentValue<bool>.At( "Temp/K10/Audio/AudioSourceConfiguration/" + property.serializedObject.targetObject.name + property.propertyPath + ".bool" ); }

	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		var lh = EditorGUIUtility.singleLineHeight;

		if( Fold( property ).Get )
			return lh;

		// var sh = SeparationLine.HEIGHT;
		var props = 9;
		//var seps = 2;
		return props * lh + /*seps * sh +*/ ( props + 3 ) * SPACING;
	}

	public override void OnGUI( Rect area, SerializedProperty property, GUIContent label )
	{
		var groupProp = property.FindPropertyRelative( "_group" );
		var volumeProp = property.FindPropertyRelative( "_volume" );
		var minPitchProp = property.FindPropertyRelative( "_minPitch" );
		var maxPitchProp = property.FindPropertyRelative( "_maxPitch" );
		var panProp = property.FindPropertyRelative( "_pan" );
		var spatialBlendProp = property.FindPropertyRelative( "_spatialBlend" );
		var dopplerLevelProp = property.FindPropertyRelative( "_dopplerLevel" );
		var spreadProp = property.FindPropertyRelative( "_spread" );
		var rolloffProp = property.FindPropertyRelative( "_rolloff" );
		var minDistanceProp = property.FindPropertyRelative( "_minDistance" );
		var maxDistanceProp = property.FindPropertyRelative( "_maxDistance" );

		EditorGuiIndentManager.New( 0 );

		var fold = Fold( property );

		var open = !fold.Get;
		GUI.Box( area, default( Texture2D ) );

		var lh = EditorGUIUtility.singleLineHeight;
		var lhs = lh + SPACING;
		
		var line = area.CutBottom( area.height - lh );

		var labelSize = label.text.Length * 6;
		fold.Set = !EditorGUI.Foldout( line.CutRight( area.width - labelSize ), open, open ? "" : label.text );

		if( !open )
		{
			var icon = IconCache.Get( "Gear" ).Texture;

			if( icon != null && GUI.Button( line.RequestHeight( icon.height ).CutLeft( line.width - icon.width ).MoveLeft( 2 ).MoveDown( 1 ), icon, K10GuiStyles.basicStyle ) )
				fold.Set = false;

			EditorGuiIndentManager.Revert();
			return;
		}

		area = area.CutTop( 2 * SPACING );
		area = area.CutLeft( SPACING * 4 );
		area = area.CutRight( SPACING * 4 );

		//line = line.CutLeft( labelSize );
		line = area.CutBottom( area.height - lh );
		//line = SeparationLine.Vertical( line );
		//GUI.Box( line, default( Texture2D ) );
		//EditorGUI.PropertyField( area.CutBottom( area.height - lh ), groupProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.New( 42 );
		EditorGUI.PropertyField( line, groupProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.Revert();
		GuiLabelWidthManager.New( 50 );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), volumeProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.Revert();
//		GuiLabelWidthManager.New( 33 );
		var minPitch = minPitchProp.floatValue;
		var maxPitch = maxPitchProp.floatValue;
		GUI.Label( area.CutBottom( area.height - lh ).CutRight( area.width - 33 ), "Pitch" );
		minPitch = EditorGUI.FloatField( area.CutBottom( area.height - lh ).CutLeft( 33 ).CutRight( area.width - 83 ), minPitch );
		minPitch = Mathf.Clamp( minPitch, .011f, 3f );
		EditorGUI.MinMaxSlider( area.CutBottom( area.height - lh ).CutLeft( 88 ).CutRight( 55 ), ref minPitch, ref maxPitch, 0.01f, 3f ); 
		maxPitch = EditorGUI.FloatField( area.CutBottom( area.height - lh ).CutLeft( area.width - 50 ), maxPitch );
		maxPitch = Mathf.Clamp( maxPitch, .011f, 3f );
		area = area.CutTop( lhs );
		minPitchProp.floatValue = minPitch;
		maxPitchProp.floatValue = maxPitch;
//		GuiLabelWidthManager.Revert();
		GuiLabelWidthManager.New( 27 );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), panProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.Revert();

		GuiLabelWidthManager.New( 85 );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), spatialBlendProp ); area = area.CutTop( lhs );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), dopplerLevelProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.Revert();
		GuiLabelWidthManager.New( 47 );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), spreadProp ); area = area.CutTop( lhs );
		EditorGUI.PropertyField( area.CutBottom( area.height - lh ), rolloffProp ); area = area.CutTop( lhs );
		GuiLabelWidthManager.Revert();

		line = area.CutBottom( area.height - lh );
		GuiLabelWidthManager.New( 28 );
		var distSize = 55;
		GUI.Label( line.CutRight( line.width - distSize ), "Distance" );
		
		line = line.CutLeft( distSize );

		var iconSize = 16;
		if( IconButton.Draw( line.CutLeft( line.width - iconSize ).RequestHeight( iconSize ), "RefreshButton", 'R', "Reset configuration to default value", Color.white ) )
		{
			volumeProp.floatValue = 1;
			minPitchProp.floatValue = 1;
			maxPitchProp.floatValue = 1;
			panProp.floatValue = 0;
			spatialBlendProp.floatValue = 0;
			dopplerLevelProp.floatValue = 1;
			spreadProp.floatValue = 1;
			rolloffProp.enumValueIndex = (int)AudioRolloffMode.Custom;
			minDistanceProp.floatValue = 1;
			maxDistanceProp.floatValue = 25;
		}

		line = line.CutRight( iconSize );
		maxDistanceProp.floatValue = EditorGUI.FloatField( line.CutLeft( line.width / 2 ), "Max", maxDistanceProp.floatValue );
		minDistanceProp.floatValue = EditorGUI.FloatField( line.CutRight( line.width / 2 ), "Min", minDistanceProp.floatValue );
		EditorGuiIndentManager.Revert();
		GuiLabelWidthManager.Revert();
		area = area.CutTop( lhs );
		EditorGuiIndentManager.Revert();
	}
}
