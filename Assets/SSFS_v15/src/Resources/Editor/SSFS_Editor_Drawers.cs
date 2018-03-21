/*
	This is part of the Sinuous Sci-Fi Signs v1.5 package
	Copyright (c) 2014-2017 Thomas Rasor
	E-mail : thomas.ir.rasor@gmail.com

	NOTE : 
	These classes are used in handling various Editor capabilities for SSFS inspectors.
*/

namespace SSFS
{
	using System.IO;
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using adb = UnityEditor.AssetDatabase;

	//this class caches loaded resources for editor inspectors
	public static class SSFS_Resources
	{
		static void LoadGUITexture( string name , ref Texture2D tex )
		{
			tex = Resources.Load( name , typeof( Texture2D ) ) as Texture2D;
			if ( tex == null ) Debug.LogWarning( "Texture2D " + name + " not found!" );
		}

		static Texture2D _box = null;
		static Texture2D _box_active = null;
		static Texture2D _box_selected = null;
		public static Texture2D box { get { if ( _box == null ) LoadGUITexture( "GUI/Editor_Box" , ref _box ); return _box; } }
		public static Texture2D box_active { get { if ( _box_active == null ) LoadGUITexture( "GUI/Editor_Box_Active" , ref _box_active ); return _box_active; } }
		public static Texture2D box_selected { get { if ( _box_selected == null ) LoadGUITexture( "GUI/Editor_Box_Thick" , ref _box_selected ); return _box_selected; } }

		static Texture2D _knob = null;
		static Texture2D _grid = null;
		static Texture2D _grid_radial = null;
		public static Texture2D knob { get { if ( _knob == null ) LoadGUITexture( "GUI/Editor_Knob" , ref _knob ); return _knob; } }
		public static Texture2D grid { get { if ( _grid == null ) LoadGUITexture( "GUI/Editor_Grid" , ref _grid ); return _grid; } }
		public static Texture2D grid_radial { get { if ( _grid_radial == null ) LoadGUITexture( "GUI/Editor_RadialGrid" , ref _grid_radial ); return _grid_radial; } }
	}

	public static class SSFS_Editor_Drawers
	{
		public static bool showHelp = false;

		public static void MaterialWithST( string title , ref Texture tex , ref Vector2 tiling , ref Vector2 offset )
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField( title );

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical( GUILayout.MaxHeight( 72f ) );
			GUILayout.FlexibleSpace();
			EditorGUIUtility.labelWidth = 50f;
			tiling = EditorGUILayout.Vector2Field( "Tiling:" , tiling );
			offset = EditorGUILayout.Vector2Field( "Offset:" , offset );
			EditorGUIUtility.labelWidth = 0f;
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			tex = ( Texture )EditorGUILayout.ObjectField( "" , tex , typeof( Texture ) , true , GUILayout.MaxWidth( 64f ) );

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		public static Vector2 GridVector2Field( Vector2 value , ref bool inUse , string title , float size , float knobSize = 16f , Color knobUseColor = default( Color ) )
		{
			GUILayout.BeginVertical();
			GUILayout.Space( 20f );

			Event e = Event.current;
			Vector2 msp = GUIUtility.GUIToScreenPoint( e.mousePosition );

			GUILayout.Box( title , SSFS_Styles.grid , GUILayout.Width( size ) , GUILayout.Height( size ) );//make the grid

			Rect gr = GUILayoutUtility.GetLastRect();//calculate the screen rect of the grid
			Vector2 gtls = GUIUtility.GUIToScreenPoint( gr.position );
			Vector2 gbrs = GUIUtility.GUIToScreenPoint( gr.position + gr.size );
			Rect gsr = new Rect( gtls , size * Vector2.one );

			if ( gsr.Contains( msp ) && e.rawType == EventType.mouseDown )//if the mouse clicked on the grid, start using this widget
				inUse = true;
			if ( e.rawType == EventType.mouseUp )//if the mouse is released anywhere, stop using this widget
				inUse = false;

			GUI.color = inUse ? knobUseColor : Color.white;//color the knob if we're using this widget

			Vector2 mgp = V2Inverp( gtls , gbrs , msp );//percent across the grid that the mouse is

			if ( inUse && e.rawType == EventType.Repaint )
			{
				if ( !e.shift )
					mgp = snapVector2ToGrid( mgp , 4 , 0.35f );
				value = mgp;
			}

			Vector2 ksp = V2Lerp( gtls , gbrs , value );//knob screen pixel position
			Vector2 kgp = GUIUtility.ScreenToGUIPoint( ksp );//the gui pixel position of the knob

			Rect kr = new Rect( kgp.x - knobSize * 0.5f , kgp.y - knobSize * 0.5f , knobSize , knobSize );
			GUI.DrawTexture( kr , SSFS_Resources.knob );
			GUI.color = Color.white;

			GUILayout.EndVertical();
			value.y = 1f - value.y;
			return value;
		}

		public static Vector2 RotationField( Vector2 value , bool useDistance , ref bool inUse , string title , float size , float knobSize = 16f , Color knobUseColor = default( Color ) )
		{
			GUILayout.BeginVertical();
			GUILayout.Space( 20f );

			Event e = Event.current;
			Vector2 msp = GUIUtility.GUIToScreenPoint( e.mousePosition );

			GUILayout.Box( title , SSFS_Styles.grid_radial , GUILayout.Width( size ) , GUILayout.Height( size ) );//make the grid

			Rect gr = GUILayoutUtility.GetLastRect();//calculate the screen rect of the grid
			Vector2 gtls = GUIUtility.GUIToScreenPoint( gr.position );
			Rect gsr = new Rect( gtls , size * Vector2.one );

			if ( gsr.Contains( msp ) && e.rawType == EventType.mouseDown )//if the mouse clicked on the grid, start using this widget
				inUse = true;
			if ( e.rawType == EventType.mouseUp )//if the mouse is released anywhere, stop using this widget
				inUse = false;


			Vector2 diff = gsr.center - msp;
			float middist = diff.magnitude / ( size * 0.5f );

			float angle = Mathf.Atan2( diff.y , diff.x );

			if ( inUse && e.rawType == EventType.Repaint )
			{
				value.x = angle * 0.15915494309f + 0.5f;//convert from radians to percentage
				if ( useDistance )
					value.y = Mathf.Clamp01( 1f - middist );

				if ( !e.shift )
				{
					value.x = snapValueToGrid( value.x , 8 , 0.35f );
					if ( useDistance )
						value.y = snapValueToGrid( value.y , 2 , 0.35f );
				}
			}

			float sinr = Mathf.Sin( value.x * Mathf.PI * 2f );
			float cosr = Mathf.Cos( value.x * Mathf.PI * 2f );
			Vector2 dir = new Vector2( cosr , sinr );
			Vector2 ksp = gsr.center + ( useDistance ? 1f - value.y : 1f ) * dir * ( size * 0.5f - knobSize * 0.25f );//knob screen pixel position
			Vector2 kgp = GUIUtility.ScreenToGUIPoint( ksp );//the gui pixel position of the knob

			Rect kr = new Rect( kgp.x - knobSize * 0.5f , kgp.y - knobSize * 0.5f , knobSize , knobSize );
			GUI.color = inUse ? knobUseColor : Color.white;//color the knob if we're using this widget

			GUI.DrawTexture( kr , SSFS_Resources.knob );
			GUI.color = Color.white;

			GUILayout.EndVertical();

			return value;
		}

		public static Vector2 V2Lerp( Vector2 a , Vector2 b , Vector2 t ) //separate component lerping for a Vector2 based on a Vector2 interpolant
		{
			Vector2 v = Vector2.zero;
			v.x = Mathf.Lerp( a.x , b.x , t.x );
			v.y = Mathf.Lerp( a.y , b.y , t.y );
			return v;
		}

		public static Vector2 V2Inverp( Vector2 a , Vector2 b , Vector2 t ) //separate component inverse lerping for a Vector2 based on a Vector2 interpolant
		{
			Vector2 v = Vector2.zero;
			v.x = Mathf.InverseLerp( a.x , b.x , t.x );
			v.y = Mathf.InverseLerp( a.y , b.y , t.y );
			return v;
		}

		public static Vector2 snapVector2ToGrid( Vector2 input , int sections = 4 , float weight = 0.25f )
		{
			input.x = snapValueToGrid( input.x , sections , weight );
			input.y = snapValueToGrid( input.y , sections , weight );
			return input;
		}

		public static float snapValueToGrid( float input , int sections = 4 , float weight = 0.25f )
		{
			float sectionWidth = 1f / sections;
			float w = sectionWidth * weight;

			for ( float i = 0f ; i <= 1f ; i += sectionWidth )
			{
				if ( Mathf.Abs( input - i ) < w )
					input = i;
			}

			return Mathf.Clamp01( input );
		}

		public static void DrawHelp( string propertyName , MessageType type = MessageType.Info )
		{
			if ( showHelp && propertyName.Length > 0 && HelpText.ContainsKey( propertyName ) && HelpText[ propertyName ].Length > 0 )
			{
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox( HelpText[ propertyName ] , type );
			}
		}

		public static void HelpToggle()
		{
			showHelp = EditorGUILayout.ToggleLeft( "Show Contextual Help" , showHelp );
		}

		public static void Tab( string name , int tabId , ref int selectedId )
		{
			if ( GUILayout.Button( name , selectedId == tabId ? SSFS_Styles.tab_on : SSFS_Styles.tab_off ) )
				selectedId = tabId;
		}

		public static void SliderWithHelp( string title , string property , ref float value , float min = 0f , float max = 1f )
		{
			DrawHelp( property );
			value = EditorGUILayout.Slider( title , value , min , max );
		}

		public static void ToggleWithHelp( string title , string property , ref bool value )
		{
			DrawHelp( property );
			value = EditorGUILayout.Toggle( title , value );
		}

		#region Property Bleeding

		public static void BleedProperty( this Material m , string n , ref float v )
		{
			if ( m.HasProperty( n ) )
				v = m.GetFloat( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].floatValue;
			else
				v = 0.5f;
		}

		public static void BleedProperty( this Material m , string n , ref bool v )
		{
			if ( m.HasProperty( n ) )
				v = m.GetFloat( n ) > 0.5f;
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].boolValue;
			else
				v = false;
		}

		public static void BleedProperty( this Material m , string n , ref Texture t )
		{
			if ( m.HasProperty( n ) )
				t = m.GetTexture( n );
			else
				t = null;
		}

		public static void BleedProperty( this Material m , string n , ref Texture t , ref Vector2 s , ref Vector2 o )
		{
			if ( m.HasProperty( n ) )
			{
				t = m.GetTexture( n );
				s = m.GetTextureScale( n );
				o = m.GetTextureOffset( n );
			}
			else
			{
				t = null;
				s = Vector2.one;
				o = Vector2.zero;
			}
		}

		public static void BleedProperty( this Material m , string n , ref Color v )
		{
			if ( m.HasProperty( n ) )
				v = m.GetColor( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].colorValue;
			else
				v = Color.white;
		}

		public static void BleedProperty( this Material m , string n , ref Vector2 v )
		{
			if ( m.HasProperty( n ) )
				v = m.GetVector( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].vectorValue;
			else
				v = Vector2.zero;
		}

		//Finds a Vector4 and splits it between two Vector2 values
		public static void BleedProperty( this Material m , string n , out Vector2 v1 , out Vector2 v2 )
		{
			Vector4 v;
			if ( m.HasProperty( n ) )
				v = m.GetVector( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].vectorValue;
			else
				v = Vector4.zero;

			v1 = new Vector2( v.x , v.y );
			v2 = new Vector2( v.z , v.w );
		}

		//Finds a Vector4 and splits it between two float values
		public static void BleedProperty( this Material m , string n , out float f1 , out float f2 )
		{
			Vector4 v;
			if ( m.HasProperty( n ) )
				v = m.GetVector( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].vectorValue;
			else
				v = Vector4.zero;

			f1 = v.x;
			f2 = v.y;
		}

		//Finds a Vector4 and splits it between three float values
		public static void BleedProperty( this Material m , string n , out float f1 , out float f2 , out float f3 )
		{
			Vector4 v;
			if ( m.HasProperty( n ) )
				v = m.GetVector( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].vectorValue;
			else
				v = Vector4.zero;

			f1 = v.x;
			f2 = v.y;
			f3 = v.z;
		}

		//Finds a Vector4 and splits it between four float values
		public static void BleedProperty( this Material m , string n , out float f1 , out float f2 , out float f3 , out float f4 )
		{
			Vector4 v;
			if ( m.HasProperty( n ) )
				v = m.GetVector( n );
			else if ( defaultProperties.ContainsKey( n ) )
				v = defaultProperties[ n ].vectorValue;
			else
				v = Vector4.zero;

			f1 = v.x;
			f2 = v.y;
			f3 = v.z;
			f4 = v.w;
		}

		#endregion

		#region Property Patching

		public static void PatchProperty( this Material m , string n , float v )
		{
			m.SetFloat( n , v );
		}

		public static void PatchProperty( this Material m , string n , bool v )
		{
			m.SetFloat( n , v ? 1f : 0f );
		}

		public static void PatchProperty( this Material m , string n , Texture t )
		{
			m.SetTexture( n , t );
		}

		public static void PatchProperty( this Material m , string n , Texture t , Vector2 s , Vector2 o )
		{
			m.SetTexture( n , t );
			m.SetTextureScale( n , s );
			m.SetTextureOffset( n , o );
		}

		public static void PatchProperty( this Material m , string n , Color v )
		{
			m.SetColor( n , v );
		}

		public static void PatchProperty( this Material m , string n , Vector2 v )
		{
			m.SetVector( n , ( Vector4 )v );
		}

		//Combines two Vector2 values for a material's Vector4 value
		public static void PatchProperty( this Material m , string n , Vector2 v1 , Vector2 v2 )
		{
			m.SetVector( n , new Vector4( v1.x , v1.y , v2.x , v2.y ) );
		}

		//Combines four float values into one Vector4 value
		public static void PatchProperty( this Material m , string n , float f1 , float f2 , float f3 , float f4 )
		{
			m.SetVector( n , new Vector4( f1 , f2 , f3 , f4 ) );
		}


		#endregion

		#region Property Drawers

		public static void DrawField( this Material m , string n , ref bool v , string title )
		{
			DrawHelp( n );
			v = EditorGUILayout.Toggle( title , v );
		}

		public static void DrawField( this Material m , string n , ref float v , string title )
		{
			DrawHelp( n );
			v = EditorGUILayout.Slider( title , v , 0f , 1f );
		}

		public static void DrawField( this Material m , string n , ref Color v , string title )
		{
			DrawHelp( n );
			v = EditorGUILayout.ColorField( title , v );
		}

		public static void DrawField( this Material m , string n , ref Vector2 v , string title )
		{
			DrawHelp( n );
			v = EditorGUILayout.Vector2Field( title , v );
		}

		public static void DrawField( this Material m , string n , ref Texture t , ref Vector2 s , ref Vector2 o , string title )
		{
			DrawHelp( n );
			MaterialWithST( title , ref t , ref s , ref o );
		}

		public static void DrawField( this Material m , string n , ref Texture v , string title )
		{
			DrawHelp( n );
			v = ( Texture )EditorGUILayout.ObjectField( title , v , typeof( Texture ) , true );
		}

		#endregion

		#region Property Help Dialogues

		public static Dictionary<string , string> HelpText = new Dictionary<string , string>()
		{
			{ "_Phase","" },

			{ "_CullMode","Whether or not to cull object back faces." },
			{ "_BlendSrc","The way this material blends with things drawn behind it." },
			{ "_Color","The overall color tint of this material." },
			{ "_MainTex","" },
			{ "_Noise","The texture that affects tile scattering. Experiment with this, as it can have a wide range of different effects on how your material looks." },
			{ "_TileCount","How many tiles there are on each axis. Generally these values should be integers." },
			{ "_Scaling","How the tiles individually scale during the effect. " +
				"Positive values shrink the tile, and negative values ( down to -1.0 ) will increase the tile image size. " +
				"Values beyond -1.0 scale the tile smaller, and flip it along that axis." },
			{ "_ScaleAroundTile","Whether the scaling should be done around tiles locally or around the entire UV." },
			{ "_IdleAmount","How much effect the idle animation has proportional to the transition effect." },
			{ "_IdleSpeed","How quickly the idle animation plays and repeats." },
			{ "_IdleRand","Random offset added to the idle animation based on the scatter texture." },

			{ "_Color2","The color tint of tiles during their effect." },
			{ "_PhaseSharpness","How quickly individual tiles complete their effect animation." },
			{ "_InvertPhase","" },
			{ "_InvertIdle","" },
			{ "_Scattering","How much tiles' individual phases are offset by the scatter texture." },
			{ "_FlashAmount","Extra overbrightening added when tiles undergo the transition or idle effect. Works with the transition tint." },

			{ "_Overbright","Extra brightness added to the base image. This will vary heavily depending on the image used. Overbrightening works very well with HDR sensitive bloom effects." },
			{ "_Aberration","Color separation seen at sheer viewing angles. Gives the effect some extra depth." },
			{ "_EffectAberration","Distance of color separation specific to tiles undergoing the animation effect." },
			{ "_ClippedTiles","Whether or not to maintain tile content despite image scaling." },
			{ "_RoundClipping","Option to clip tiles using circles instead of the normal square tile." },

			{ "_Radial","Start Location : Where the phase animation starts. Scaling Center : Where tiles scale towards or away from. When tiles scale around tiles ( General Options ), this is local to the tile's individual space." },

			{ "_MainTex2","The image shown behind the camera's render. Can be a render texture from another camera to create camera transitions." },
			{ "_SquareTiles","Whether or not to force the usage of square tiles on the screen." },
			{ "_ScanlineIntensity","The strength of the scanline effect." },
			{ "_ScanlineShift","Pixel and aberration offset created by scanlines." },

			{ "COMPLEX","This toggle enables or disables certain subtle features in SSFS shaders. These include using vertex colors and using all RGB channels of the scatter texture." },
		};

		#endregion

		public class PropertyDefault
		{
			public enum Type { Float, Bool, Vector, Color }
			public Type type = Type.Float;
			public float floatValue = 0f;
			public bool boolValue = false;
			public Vector4 vectorValue = Vector4.zero;
			public Color colorValue = Color.white;

			public PropertyDefault( float f ) { type = Type.Float; floatValue = f; }
			public PropertyDefault( bool b ) { type = Type.Bool; boolValue = b; }
			public PropertyDefault( Vector2 v ) { type = Type.Vector; vectorValue = v; }
			public PropertyDefault( Vector3 v ) { type = Type.Vector; vectorValue = v; }
			public PropertyDefault( Vector4 v ) { type = Type.Vector; vectorValue = v; }
			public PropertyDefault( Color c ) { type = Type.Color; colorValue = c; }
		}

		public static Dictionary<string , PropertyDefault> defaultProperties = new Dictionary<string , PropertyDefault>()
		{
			{"_BlendSrc" ,  new PropertyDefault(1f) },
			{"_BlendDst" ,  new PropertyDefault(0f) },
			{"_Cull" ,  new PropertyDefault(2f) },
			{"_ZWrite" ,  new PropertyDefault(0f) },
			{"_ZTest" ,  new PropertyDefault(8f) },

			{"_Phase" ,  new PropertyDefault(1f) },
			{"_Color" ,  new PropertyDefault(Color.white) },
			{"_Color2" ,  new PropertyDefault(Color.white) },
			{"_Overbright" ,  new PropertyDefault(0f) },
			{"_TileCount" ,  new PropertyDefault(new Vector4(25,25,0,0)) },
			{"_SquareTiles" ,  new PropertyDefault(false) },
			{"_InvertPhase" ,  new PropertyDefault(false) },
			{"_InvertIdle" ,  new PropertyDefault(false) },
			{"_IdleAmount" ,  new PropertyDefault(0.1f) },
			{"_IdleSpeed" ,  new PropertyDefault(0.2f) },
			{"_IdleRand" ,  new PropertyDefault(0f) },
			{"_PhaseRotation" ,  new PropertyDefault(0f) },
			{"_Radial" ,  new PropertyDefault(0f) },
			{"_PhaseSharpness" ,  new PropertyDefault(0.5f) },
			{"_Scattering" ,  new PropertyDefault(0.25f) },
			{"_Scaling" ,  new PropertyDefault(Vector4.zero) },
			{"_ScaleCenter" ,  new PropertyDefault(Vector4.one * 0.5f) },
			{"_Aberration" ,  new PropertyDefault(0f) },
			{"_EffectAberration" ,  new PropertyDefault(0f) },
			{"_FlashAmount" ,  new PropertyDefault(0f) },
			{"_Flicker" ,  new PropertyDefault(0f) },
			{"_ScanlineIntensity" ,  new PropertyDefault(0f) },
			{"_ScaleAroundTile" ,  new PropertyDefault(true) },
			{"_ClippedTiles" ,  new PropertyDefault(true) },
			{"_RoundClipping" ,  new PropertyDefault(false) }
		};
	}

	public static class SSFS_Styles
	{
		static GUIStyle _tab_off = null;
		public static GUIStyle tab_off
		{
			get
			{
				if ( _tab_off == null )
				{
					_tab_off = new GUIStyle( GUI.skin.button );
					_tab_off.padding = new RectOffset( 4 , 4 , 8 , 8 );
					_tab_off.margin = new RectOffset( 0 , 0 , 0 , 0 );
					_tab_off.normal.background = SSFS_Resources.box;
					_tab_off.active.background = SSFS_Resources.box_active;
				}
				return _tab_off;
			}
		}

		static GUIStyle _tab_on = null;
		public static GUIStyle tab_on
		{
			get
			{
				if ( _tab_on == null )
				{
					_tab_on = new GUIStyle( GUI.skin.button );
					_tab_on.padding = new RectOffset( 4 , 4 , 8 , 8 );
					_tab_on.margin = new RectOffset( 0 , 0 , 0 , 0 );
					_tab_on.normal.background = SSFS_Resources.box_selected;
					_tab_on.active.background = SSFS_Resources.box_active;
				}
				return _tab_on;
			}
		}

		static GUIStyle _params_box = null;
		public static GUIStyle params_box
		{
			get
			{
				if ( _params_box == null )
				{
					_params_box = new GUIStyle( GUI.skin.box );
					_params_box.margin = new RectOffset( 0 , 0 , 0 , 0 );
					_params_box.padding = new RectOffset( 16 , 16 , 8 , 8 );
					_params_box.normal.background = SSFS_Resources.box;
				}
				return _params_box;
			}
		}

		static GUIStyle _grid = null;
		public static GUIStyle grid
		{
			get
			{
				if ( _grid == null )
				{
					_grid = new GUIStyle( GUI.skin.box );
					_grid.normal.background = SSFS_Resources.grid;
					_grid.contentOffset = new Vector2( 0f , -20f );
					_grid.fontStyle = FontStyle.Bold;
					_grid.clipping = TextClipping.Overflow;
					_grid.wordWrap = false;
				}
				return _grid;
			}
		}

		static GUIStyle _grid_radial = null;
		public static GUIStyle grid_radial
		{
			get
			{
				if ( _grid_radial == null )
				{
					_grid_radial = new GUIStyle( GUI.skin.box );
					_grid_radial.normal.background = SSFS_Resources.grid_radial;
					_grid_radial.contentOffset = new Vector2( 0f , -20f );
					_grid_radial.fontStyle = FontStyle.Bold;
					_grid_radial.clipping = TextClipping.Overflow;
					_grid_radial.wordWrap = false;
				}
				return _grid_radial;
			}
		}
	}

	public static class SSFS_Editor_Helpers
	{
		[MenuItem( "Assets/Create/SSFS Material" , false , 301 )]
		public static void CreateNewSSFSMaterial()
		{
			string path = adb.GetAssetPath( Selection.activeObject );
			if ( path == "" ) path = "Assets";
			else if ( Path.GetExtension( path ) != "" )
				path = path.Replace( Path.GetFileName( adb.GetAssetPath( Selection.activeObject ) ) , "" );

			Material m = new Material( Shader.Find( "Sci-Fi/SSFS" ) );
			adb.CreateAsset( m , adb.GenerateUniqueAssetPath( path + "/New SSFS Material.mat" ) );
		}
	}
}