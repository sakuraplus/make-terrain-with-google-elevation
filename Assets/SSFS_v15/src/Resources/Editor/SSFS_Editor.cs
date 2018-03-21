/*
	This is part of the Sinuous Sci-Fi Signs v1.5 package
	Copyright (c) 2014-2017 Thomas Rasor
	E-mail : thomas.ir.rasor@gmail.com

	NOTE : 
	This editor does more than simply set the values on the material it is inspecting, please do not delete this file.
	If you do delete this editor, your materials will not behave correctly through various property changes.
	This editor cannot be within the SSFS namespace due to the way Unity currently handles custom Material Editors.
*/

using SSFS;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using gui = UnityEngine.GUILayout;
using egui = UnityEditor.EditorGUILayout;
using sed = SSFS.SSFS_Editor_Drawers;
using style = SSFS.SSFS_Styles;
using blend = UnityEngine.Rendering.BlendMode;
using comp = UnityEngine.Rendering.CompareFunction;
using cull = UnityEngine.Rendering.CullMode;

public class SSFS_Editor : MaterialEditor
{
	Material _m;
	public Material m
	{
		get
		{
			if ( _m == null )
			{
				_m = ( Material )target;
				_keywords = new List<string>( m.shaderKeywords );
				CheckFeatures();
			}
			return _m;
		}
	}

	List<string> _keywords = new List<string>();
	Hashtable _features = new Hashtable();

	public enum BlendType
	{
		Additive,
		AlphaBlended,
		SoftAdditive,
		Multiply,
		Invert,
		Solid
	}

	public BlendType blendType = BlendType.Additive;

	#region shellvars

	public Color mainColor = Color.white;
	public Color effectColor = Color.white;
	public Vector2 mainTextureOffset=Vector2.zero, mainTextureScale = Vector2.one;
	public Vector2 mainTexture2Offset = Vector2.zero, mainTexture2Scale = Vector2.one;
	public Texture mainTexture, mainTexture2, noiseTexture;
	public Vector2 tileCount = new Vector2(42f,42f), scaling = Vector2.zero, scalingCenter = Vector2.one * 0.5f;
	public Vector2 RotationRadial = new Vector2(0.5f,0f);
	public float scattering = 0.25f, scanlineIntensity = 0.2f, scanlineScale = 1f, scanlineShift = 0f, scanlineSpeed = 0f;
	public float scaleAroundTile = 0f,backfaceVisibility = 0.5f;

	public float phase = 1f, sharpness=0.6f, overbright=0f, aberration=0.2f, effectAberration=0.5f, flash =0.1f, flicker=0.1f;
	public float idleAmount=0.2f, idleSpeed=0.2f, idleRand=0f;
	public bool squareTiles=false;
	public bool invertPhase=false, invertIdle = false, clipTiles = false, roundClipping = false;
	public bool twoSided = true;

	#endregion

	public bool isChangingScalingCenter = false;
	public bool isChangingPhaseRotation = false;
	public bool showImages = false, showHelp = false;
	public static int currentTab = 0;

	#region Feature Methods

	bool FeatureToggle( string label , string feature )
	{
		if ( _features == null ) return false;
		if ( !_features.ContainsKey( feature ) ) return false;
		_features[ feature ] = egui.ToggleLeft( label , ( bool )_features[ feature ] );
		return ( ( bool )_features[ feature ] );
	}

	void CheckFeatures()
	{
		_features = new Hashtable();
		CheckFeature( "ABERRATION" );
		CheckFeature( "COMPLEX" );
		CheckFeature( "CLIPPING" );
		CheckFeature( "IDLE" );
		CheckFeature( "POST" );
		CheckFeature( "RADIAL" );
		CheckFeature( "SCALE_AROUND_TILE" );
		CheckFeature( "SCAN_LINES" );
		CheckFeature( "TEXTURE_SWAP" );
		CheckFeature( "WORLD_SPACE_SCANLINES" );
	}

	void CheckFeature( string s ) { if ( !_features.ContainsKey( s ) ) _features.Add( s , _keywords.Contains( s ) ); }

	void SetFeature( string feature , bool value )
	{
		if ( _features == null ) return;
		if ( !_features.ContainsKey( feature ) ) return;
		_features[ feature ] = value;
	}

	bool world_space_scanlines { get { return ( bool )_features[ "WORLD_SPACE_SCANLINES" ]; } }
	bool scanlines { get { return ( bool )_features[ "SCAN_LINES" ]; } }
	bool post_effect { get { return ( bool )_features[ "POST" ]; } }

	void SetFeatures()
	{
		if ( _features == null ) return;
		if ( _features.ContainsKey( "POST" ) ) _features[ "POST" ] = false;
		foreach ( string s in _features.Keys )
			if ( !( bool )_features[ s ] ) m.DisableKeyword( s ); else m.EnableKeyword( s );
	}

	#endregion

	#region PropertyBleedingAndPatching

	void ReadProperties()
	{
		if ( !m.shader.isSupported ) return;

		m.BleedProperty( "_Color" , ref mainColor );
		m.BleedProperty( "_Color2" , ref effectColor );

		m.BleedProperty( "_MainTex" , ref mainTexture , ref mainTextureScale , ref mainTextureOffset );
		m.BleedProperty( "_MainTex2" , ref mainTexture2 , ref mainTexture2Scale , ref mainTexture2Offset );
		m.BleedProperty( "_Noise" , ref noiseTexture );

		m.BleedProperty( "_Phase" , ref phase );
		m.BleedProperty( "_PhaseSharpness" , ref sharpness );
		m.BleedProperty( "_InvertPhase" , ref invertPhase );

		m.BleedProperty( "_PhaseDirection" , out RotationRadial.x , out RotationRadial.y );

		m.BleedProperty( "_Overbright" , ref overbright );
		m.BleedProperty( "_Scattering" , ref scattering );
		m.BleedProperty( "_Aberration" , ref aberration );
		m.BleedProperty( "_EffectAberration" , ref effectAberration );
		m.BleedProperty( "_FlashAmount" , ref flash );
		m.BleedProperty( "_Flicker" , ref flicker );
		m.BleedProperty( "_BackfaceVisibility" , ref backfaceVisibility );

		float invidletmp;
		m.BleedProperty( "_IdleData" , out idleAmount , out idleSpeed , out idleRand , out invidletmp );
		invertIdle = invidletmp > 0.5f;

		m.BleedProperty( "_ScaleAroundTile" , ref scaleAroundTile );
		m.BleedProperty( "_ClippedTiles" , ref clipTiles );
		m.BleedProperty( "_RoundClipping" , ref roundClipping );

		m.BleedProperty( "_ScanlineData" , out scanlineIntensity , out scanlineScale , out scanlineShift , out scanlineSpeed );

		m.BleedProperty( "_TileCount" , ref tileCount );
		m.BleedProperty( "_SquareTiles" , ref squareTiles );
		m.BleedProperty( "_Scaling" , out scaling , out scalingCenter );

		ReadBlendMode();
	}

	void SetProperties()
	{
		if ( !m.shader.isSupported ) return;

		m.PatchProperty( "_Color" , mainColor );
		m.PatchProperty( "_Color2" , effectColor );

		m.PatchProperty( "_MainTex" , mainTexture , mainTextureScale , mainTextureOffset );
		m.PatchProperty( "_MainTex2" , mainTexture2 , mainTexture2Scale , mainTexture2Offset );
		m.PatchProperty( "_Noise" , noiseTexture );

		m.PatchProperty( "_Phase" , phase );
		m.PatchProperty( "_PhaseSharpness" , sharpness );
		m.PatchProperty( "_InvertPhase" , invertPhase );

		m.PatchProperty( "_PhaseDirection" , RotationRadial );

		m.PatchProperty( "_Overbright" , overbright );
		m.PatchProperty( "_Scattering" , scattering );
		m.PatchProperty( "_Aberration" , aberration );
		m.PatchProperty( "_EffectAberration" , effectAberration );
		m.PatchProperty( "_FlashAmount" , flash );
		m.PatchProperty( "_Flicker" , flicker );
		m.PatchProperty( "_BackfaceVisibility" , backfaceVisibility );

		m.PatchProperty( "_IdleData" , idleAmount , idleSpeed , idleRand , invertIdle ? 1f : 0f );

		m.PatchProperty( "_ScaleAroundTile" , scaleAroundTile );
		m.PatchProperty( "_ClippedTiles" , clipTiles );
		m.PatchProperty( "_RoundClipping" , roundClipping );

		m.PatchProperty( "_ScanlineData" , scanlineIntensity , scanlineScale , scanlineShift , scanlineSpeed );

		m.PatchProperty( "_TileCount" , tileCount );
		m.PatchProperty( "_SquareTiles" , squareTiles );
		m.PatchProperty( "_Scaling" , scaling , scalingCenter );

		SetBlendMode();
	}

	void ReadBlendMode()
	{
		int cm = 2;
		if ( m.HasProperty( "_Cull" ) ) cm = m.GetInt( "_Cull" );
		twoSided = ( cm == 0 );

		int bs = 99, bd = 99;
		if ( m.HasProperty( "_BlendSrc" ) && m.HasProperty( "_BlendDst" ) )
		{
			bs = m.GetInt( "_BlendSrc" );
			bd = m.GetInt( "_BlendDst" );

			if ( bs == ( int )blend.SrcAlpha && bd == ( int )blend.OneMinusSrcAlpha )
				blendType = BlendType.AlphaBlended;
			else if ( bs == ( int )blend.One && bd == ( int )blend.One )
				blendType = BlendType.Additive;
			else if ( bs == ( int )blend.OneMinusDstColor && bd == ( int )blend.One )
				blendType = BlendType.SoftAdditive;
			else if ( bs == ( int )blend.DstColor && bd == ( int )blend.Zero )
				blendType = BlendType.Multiply;
			else if ( bs == ( int )blend.OneMinusDstColor && bd == ( int )blend.OneMinusSrcAlpha )
				blendType = BlendType.Invert;
			else if ( bs == ( int )blend.One && bd == ( int )blend.Zero )
				blendType = BlendType.Solid;
			else
				blendType = BlendType.Additive;
		}
	}

	void SetBlendMode()
	{
		if ( m.HasProperty( "_Cull" ) ) m.SetInt( "_Cull" , ( twoSided ) ? ( int )cull.Off : ( int )cull.Back );
		if ( m.HasProperty( "_ZTest" ) ) m.SetInt( "_ZTest" , ( int )comp.LessEqual );

		switch ( blendType )
		{
			case BlendType.AlphaBlended:
				m.renderQueue = 3000;
				m.SetOverrideTag( "RenderType" , "Transparent" );
				m.SetOverrideTag( "IgnoreProjectors" , "true" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 0 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.SrcAlpha );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.OneMinusSrcAlpha );
				break;
			case BlendType.Additive:
				m.renderQueue = 3000;
				m.SetOverrideTag( "RenderType" , "Transparent" );
				m.SetOverrideTag( "IgnoreProjectors" , "true" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 0 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.One );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.One );
				break;
			case BlendType.SoftAdditive:
				m.renderQueue = 3000;
				m.SetOverrideTag( "RenderType" , "Transparent" );
				m.SetOverrideTag( "IgnoreProjectors" , "true" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 0 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.OneMinusDstColor );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.One );
				break;
			case BlendType.Multiply:
				m.renderQueue = 3000;
				m.SetOverrideTag( "RenderType" , "Transparent" );
				m.SetOverrideTag( "IgnoreProjectors" , "true" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 0 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.DstColor );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.Zero );
				break;
			case BlendType.Invert:
				m.renderQueue = 3000;
				m.SetOverrideTag( "RenderType" , "Transparent" );
				m.SetOverrideTag( "IgnoreProjectors" , "true" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 0 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.OneMinusDstColor );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.OneMinusSrcAlpha );
				break;
			case BlendType.Solid:
				m.renderQueue = 2000;
				m.SetOverrideTag( "RenderType" , "Opaque" );
				m.SetOverrideTag( "IgnoreProjectors" , "false" );
				if ( m.HasProperty( "_ZWrite" ) ) m.SetInt( "_ZWrite" , 1 );
				if ( m.HasProperty( "_BlendSrc" ) ) m.SetInt( "_BlendSrc" , ( int )blend.One );
				if ( m.HasProperty( "_BlendDst" ) ) m.SetInt( "_BlendDst" , ( int )blend.Zero );
				break;
		}
	}

	#endregion

	override public void OnInspectorGUI()
	{
		if ( !isVisible ) return;

		Undo.RecordObject( target , "Edit SSFS Material" );
		EditorGUI.BeginChangeCheck();

		ReadProperties();

		sed.HelpToggle();
		m.DrawField( "_Phase" , ref phase , "Phase" );

		egui.Space();
		egui.BeginHorizontal();
		sed.Tab( "General" , 0 , ref currentTab );
		sed.Tab( "Transition" , 1 , ref currentTab );
		sed.Tab( "Idle" , 2 , ref currentTab );
		sed.Tab( "Effects" , 3 , ref currentTab );
		sed.Tab( "Scanlines" , 4 , ref currentTab );
		egui.EndHorizontal();
		egui.Space();

		egui.BeginVertical( style.params_box );
		switch ( currentTab )
		{
			case 0: DrawGeneralTab(); break;
			case 1: DrawTransitionTab(); break;
			case 2: DrawIdleTab(); break;
			case 3: DrawEffectsTab(); break;
			case 4: DrawScanlinesTab(); break;
		}
		egui.EndVertical();
		egui.Space();
		egui.Space();

		SetProperties();
		SetFeatures();

		if ( isChangingPhaseRotation || isChangingScalingCenter || EditorGUI.EndChangeCheck() )
			EditorUtility.SetDirty( m );
	}

	#region OptionTabs

	public void DrawGeneralTab()
	{
		m.DrawField( "_Cull" , ref twoSided , "Two Sided" );

		if ( m.HasProperty( "_BlendSrc" ) && m.HasProperty( "_BlendDst" ) )
		{
			sed.DrawHelp( "_BlendSrc" );
			blendType = ( BlendType )egui.EnumPopup( "Blending Mode" , blendType );
		}

		m.DrawField( "_Color" , ref mainColor , "Global Tint" );

		showImages = EditorGUI.Foldout( egui.GetControlRect() , showImages , "Textures" , true );
		if ( showImages )
		{
			m.DrawField( "_MainTex" , ref mainTexture , ref mainTextureScale , ref mainTextureOffset , "Image Texture" );

			if ( FeatureToggle( "Texture Swap" , "TEXTURE_SWAP" ) )
				m.DrawField( "_MainTex2" , ref mainTexture2 , ref mainTexture2Scale , ref mainTexture2Offset , "Image Texture 2" );
			m.DrawField( "_Noise" , ref noiseTexture , "Scatter Texture" );
		}

		m.DrawField( "_TileCount" , ref tileCount , "Tile Count" );
		m.DrawField( "_SquareTiles" , ref squareTiles , "Square Tiles" );
		m.DrawField( "_Scaling" , ref scaling , "Scaling" );

		if ( FeatureToggle( "Scale Around Tile" , "SCALE_AROUND_TILE" ) )
			m.DrawField( "_ScaleAroundTile" , ref scaleAroundTile , "Scale Around Tiles" );

		sed.DrawHelp( "COMPLEX" );
		if ( FeatureToggle( "Allow Complexities" , "COMPLEX" ) )
		{
			sed.DrawField( m , "_Flicker" , ref flicker , "Flicker" );
			sed.DrawField( m , "_BackfaceVisibility" , ref backfaceVisibility , "Backface Visibility" );
		}
	}

	public void DrawTransitionTab()
	{
		m.DrawField( "_Color2" , ref effectColor , "Transition Tint" );
		m.DrawField( "_PhaseSharpness" , ref sharpness , "Phase Sharpness" );
		m.DrawField( "_InvertPhase" , ref invertPhase , "Reverse Direction" );
		m.DrawField( "_Scattering" , ref scattering , "Scatter Distance" );
		m.DrawField( "_FlashAmount" , ref flash , "Flash Amount" );

		egui.Space();
		sed.DrawHelp( "_Radial" );
		egui.BeginHorizontal();
		gui.FlexibleSpace();
		gui.Label( "Hold Shift To Ignore Snapping On Grids." , EditorStyles.boldLabel );
		gui.FlexibleSpace();
		egui.EndHorizontal();

		egui.BeginHorizontal();
		gui.FlexibleSpace();
		if ( m.HasProperty( "_PhaseDirection" ) )
			RotationRadial = sed.RotationField( RotationRadial , true , ref isChangingPhaseRotation , "Start Location" , 100f , 16f , Color.cyan );
		if ( m.HasProperty( "_Scaling" ) )
		{
			gui.FlexibleSpace();
			scalingCenter = sed.GridVector2Field( scalingCenter , ref isChangingScalingCenter , "Scaling Center" , 100f , 16f , Color.cyan );
		}
		gui.FlexibleSpace();
		if ( isChangingPhaseRotation || isChangingScalingCenter )
			Repaint();
		egui.EndHorizontal();

		SetFeature( "RADIAL" , ( RotationRadial.y > 0f ) );
	}

	public void DrawIdleTab()
	{
		if ( FeatureToggle( "Idle Animation" , "IDLE" ) )
		{
			m.DrawField( "_IdleAmount" , ref idleAmount , "Idle Amount" );
			m.DrawField( "_IdleSpeed" , ref idleSpeed , "Idle Speed" );
			m.DrawField( "_InvertIdle" , ref invertIdle , "Reverse Idle Direction" );
			m.DrawField( "_IdleRand" , ref idleRand , "Idle Noise" );
		}
	}

	public void DrawEffectsTab()
	{
		m.DrawField( "_Overbright" , ref overbright , "Overbright" );
		if ( FeatureToggle( "Color Aberration" , "ABERRATION" ) )
		{
			m.DrawField( "_Aberration" , ref aberration , "Fresnel Color Separation" );
			m.DrawField( "_EffectAberration" , ref effectAberration , "Flash Color Separation" );
		}
		if ( FeatureToggle( "Tile Clipping" , "CLIPPING" ) )
			m.DrawField( "_RoundClipping" , ref roundClipping , "Round Clipping" );
	}

	public void DrawScanlinesTab()
	{
		if ( FeatureToggle( "Scanlines" , "SCAN_LINES" ) )
		{
			m.DrawField( "_ScanlineIntensity" , ref scanlineIntensity , "Scanline Intensity" );
			m.DrawField( "_ScanlineScale" , ref scanlineScale , "Scanline Scale" );
			m.DrawField( "_ScanlineSpeed" , ref scanlineSpeed , "Scanline Movement Speed" );
			m.DrawField( "_ScanlineShift" , ref scanlineShift , "Scanline UV Shift" );
			FeatureToggle( "Use World Space for Scanlines" , "WORLD_SPACE_SCANLINES" );
		}
	}

	#endregion
}