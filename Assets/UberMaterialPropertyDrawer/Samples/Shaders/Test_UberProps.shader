Shader "Test/UberProps"
{
    Properties
    {
    	[InitGroupDecorator]
    	[BeginGroup(Misc)] _BeginMiscDummy ("_BeginMiscDummy", int) = 0
    	[Vector2(Misc)] _Vec2Test("Vec2Test", Vector) = (0,0,0,0)
    	[EndGroup(Misc)] _EndMiscDummy ("_EndMiscDummy", int) = 0
        
    	_AAA ("_AAA", 2D) = "white" {}
    	[BeginToggleGroup(A)]	_UseA ("_UseA", int) = 1
        [BeginToggleGroup(B)]   _UseB ("_UseB", int) = 1
        [BeginToggleGroup(C)]   _UseC ("_UseC", int) = 1
        [BeginToggleGroup(D)]   _UseD ("_UseD", int) = 1
        [EndGroup(D)]			_EndD ("_EndD", int) = 0
        [EndGroup(C)]			_EndC ("_EndC", int) = 0
        [EndGroup(B)]			_EndB ("_EndB", int) = 0
        [EndGroup(A)]			_EndA ("_EndA", int) = 0
        [BeginToggleGroup(TestGroup)]	_UseSomeFeature		("_UseSomeFeature", int) = 1
        [Uber(TestGroup)]				_UberTest			("_UberTest", int) = 0
        [Uber(TestGroup)]				_SomeFeatureMap		("_SomeFeatureMap", 2D) = "white" {}
        [Uber(TestGroup)]				_SomeFeatureWidth	("_SomeFeatureWidth", float) = 0
    	[UberEnum(TestGroup,R,0,G,1,B,2,A,3)]
    									_AlphaChannel				("_AlphaChannel",int)=3
        [Uber(TestGroup)]				_UberTest0					("_UberTestNone", range(0,1)) = 0
        [BeginGroup(NestGroup)]			_UseNestFeature				("_UseNestFeature", int) = 1
        [Uber(NestGroup)]               _NestFeatureColor 			("_NestFeatureColor", color) = (0.2,0.7,0.8)
        [BeginGroup(GrandChild)]		_BeginGroundChild 			("_BeginGroundChild", int) = 1
        [Vector2(GrandChild)]			_GrandChildVector 			("_GrandChildVector", vector) = (0,0,0,0)
    	[UberToggle(GrandChild)]		_UseGrandChildSomeFeature	("_UseGrandChildSomeFeature", int) = 1
    	[CurveTexture(GrandChild, res256, ch4, bit8)]
										_GrandChildCurve			("_GrandChildCurve", 2D) = "black" {}
    	[GradientTexture(GrandChild, res256, ch4, bit8)]
    									_GrandChildGradient 		("_GrandChildGradient", 2D) = "black" {}
        [EndGroup(GrandChild)]			_EndGroundChild 			("_EndGroundChild", int) = 0
        [EndGroup(NestGroup)]			_EndNestGroup				("_EndNestGroup", int) = 0
        [EndGroup(TestGroup)]			_EndSomGroup				("_EndSomGroup", int) = 0
    	
        [BeginGroup(Face)]	_BeginFaceDummy		("_", int) = 0
        [Uber(Face)][HDR]	_FaceColor			("Face Color", Color) = (1,1,1,1)
    	[Uber(Face)]		_FaceTex			("Face Texture", 2D) = "white" {}
		[Uber(Face)]		_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
		[Uber(Face)]		_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
		[Uber(Face)]    	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0
    	[Uber(Face)]		_FaceDilate			("Face Dilate", Range(-1,1)) = 0
    	[EndGroup(Face)]	_EndFaceDummy		("_", int) = 0
		
		[BeginGroup(Outline)] _BeginOutlineDummy	("_", int) = 0
    	[Uber(Outline)] [HDR]_OutlineColor			("Outline Color", Color) = (0,0,0,1)
		[Uber(Outline)] _OutlineTex					("Outline Texture", 2D) = "white" {}
		[Uber(Outline)] _OutlineUVSpeedX			("Outline UV Speed X", Range(-5, 5)) = 0.0
		[Uber(Outline)] _OutlineUVSpeedY			("Outline UV Speed Y", Range(-5, 5)) = 0.0
		[Uber(Outline)] _OutlineWidth				("Outline Thickness", Range(0, 1)) = 0
		[EndGroup(Outline)] _EndOutlineDummy		("_", int) = 0
    	
    	[BeginToggleGroup(Unverlay)]	_BeginUnderlayDummy ("_", int) = 0
		[Uber(Unverlay)]				[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0, 0.5)
		[Uber(Unverlay)]				_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
		[Uber(Unverlay)]				_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
		[Uber(Unverlay)]				_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
		[Uber(Unverlay)]				_UnderlaySoftness	("Border Softness", Range(0,1)) = 0
    	[EndGroup(Unverlay)]			_EndUnderlayDummy	("_", int) = 0
    	
    	[BeginToggleGroup(Lighting)]	_BeginLightingDummy	("_", int) = 0
    	[BeginGroup(Bevel)]				_BeginBevelDummy	("", int) = 0
    	[Uber(Bevel)] 					_ShaderFlags		("Flags", float) = 0
		[Uber(Bevel)] 					_Bevel				("Bevel", Range(0,1)) = 0.5
		[Uber(Bevel)] 					_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
		[Uber(Bevel)] 					_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
		[Uber(Bevel)] 					_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
		[Uber(Bevel)] 					_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0
    	[EndGroup(Bevel)]				_EndBevelDummy		("_", int) = 0

    	[BeginGroup(LocalLighting)] _BeginLocalLightingDummy ("_", int) = 0
		[Uber(LocalLighting)] _LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
		[Uber(LocalLighting)] [HDR]_SpecularColor	("Specular", Color) = (1,1,1,1)
		[Uber(LocalLighting)] _SpecularPower		("Specular", Range(0,4)) = 2.0
		[Uber(LocalLighting)] _Reflectivity			("Reflectivity", Range(5.0,15.0)) = 10
		[Uber(LocalLighting)] _Diffuse				("Diffuse", Range(0,1)) = 0.5
		[Uber(LocalLighting)] _Ambient				("Ambient", Range(1,0)) = 0.5
    	[EndGroup(LocalLighting)] _EndLocalLightingDummy		("_", int) = 0

    	[BeginGroup(Bump Map)] _BeginBumpMapDummy	("_", int) = 0
		[Uber(Bump Map)] [NoScaleOffset]_BumpMap 	("Normal map", 2D) = "bump" {}
		[Uber(Bump Map)] _BumpOutline				("Bump Outline", Range(0,1)) = 0
		[Uber(Bump Map)] _BumpFace					("Bump Face", Range(0,1)) = 0
    	[EndGroup(Bump Map)] _EndBumpMapDummy		("_", int) = 0
		
    	[BeginGroup(EnvironmentReflection)] 		_BeginEnvironmentReflectionDummy("_", int) = 0
		[Uber(EnvironmentReflection)]				_ReflectFaceColor				("Reflection Color", Color) = (0,0,0,1)
		[Uber(EnvironmentReflection)]				_ReflectOutlineColor			("Reflection Color", Color) = (0,0,0,1)
		[Uber(EnvironmentReflection)][NoScaleOffset]_Cube							("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
		[Vector3(EnvironmentReflection)]			_EnvMatrixRotation				("Texture Rotation", vector) = (0, 0, 0, 0)
    	[EndGroup(EnvironmentReflection)]			_EndEnvironmentReflectionDummy	("_", int) = 0
		
    	[EndGroup(Lighting)] _EndLightingDummy ("_", int) = 0

    	[BeginToggleGroup(Glow)] _BeginGlowDummy ("_", int) = 0
		[Uber(Glow)]	[HDR]_GlowColor		("Color", Color) = (0, 1, 0, 0.5)
		[Uber(Glow)]	_GlowOffset			("Offset", Range(-1,1)) = 0
		[Uber(Glow)]	_GlowInner			("Inner", Range(0,1)) = 0.05
		[Uber(Glow)]	_GlowOuter			("Outer", Range(0,1)) = 0.05
		[Uber(Glow)]	_GlowPower			("Falloff", Range(1, 0)) = 0.75
    	[EndGroup(Glow)] _EndGlowDummy ("", int) = 0

    	[BeginGroup(Debug Setting)] _BeginDebugSettingDummy ("_", int) = 0
		[Uber(Debug Setting)]	_WeightNormal		("Weight Normal", float) = 0
		[Uber(Debug Setting)]	_WeightBold			("Weight Bold", float) = 0.5

		[Uber(Debug Setting)]	_MainTex			("Font Atlas", 2D) = "white" {}
		[Uber(Debug Setting)]	_TextureWidth		("Texture Width", float) = 512
		[Uber(Debug Setting)]	_TextureHeight		("Texture Height", float) = 512
		[Uber(Debug Setting)]	_GradientScale		("Gradient Scale", float) = 5.0
		[Uber(Debug Setting)]	_ScaleX				("Scale X", float) = 1.0
		[Uber(Debug Setting)]	_ScaleY				("Scale Y", float) = 1.0
		
		[Uber(Debug Setting)]	_Sharpness			("Sharpness", Range(-1,1)) = 0
    	[Uber(Debug Setting)]	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875

		[Uber(Debug Setting)]	_VertexOffsetX		("Vertex OffsetX", float) = 0
		[Uber(Debug Setting)]	_VertexOffsetY		("Vertex OffsetY", float) = 0

		[Uber(Debug Setting)]	_MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
		[Uber(Debug Setting)]	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
		[Uber(Debug Setting)]	_MaskSoftnessX		("Mask SoftnessX", float) = 0
		[Uber(Debug Setting)]	_MaskSoftnessY		("Mask SoftnessY", float) = 0

		[UberEnum(Debug Setting, CompareFunction)]
    							_StencilComp		("Stencil Comparison", Float) = 8
		[Uber(Debug Setting)]	_Stencil			("Stencil ID", Float) = 0
		[UberEnum(Debug Setting, StencilOp)]
    							_StencilOp			("Stencil Operation", Float) = 0
		[Uber(Debug Setting)]	_StencilWriteMask	("Stencil Write Mask", Float) = 255
		[Uber(Debug Setting)]	_StencilReadMask	("Stencil Read Mask", Float) = 255

		[UberEnum(Debug Setting, CullMode)]	
    							_CullMode			("Cull Mode", Float) = 0
		[Uber(Debug Setting)]	_ColorMask			("Color Mask", Float) = 15
    	[Uber(Debug Setting)]	_ScaleRatioA		("Scale RatioA", float) = 1
		[Uber(Debug Setting)]	_ScaleRatioB		("Scale RatioB", float) = 1
		[Uber(Debug Setting)]	_ScaleRatioC		("Scale RatioC", float) = 1
    	[EndGroup(Debug Setting)] _EndDebugSettingDummy ("_", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
