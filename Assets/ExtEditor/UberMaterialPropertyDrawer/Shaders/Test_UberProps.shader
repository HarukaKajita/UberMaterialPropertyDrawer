Shader "Test/UberProps"
{
    Properties
    {
//        _AAA ("_AAA", 2D) = "white" {}
//        [Uber(Test, ARG0)]                  _UberTest ("_UberTest", int) = 0
//        [Uber(TestGroup,  BeginToggleGroup)]_UseSomeFeature ("_UseSomeFeature", int) = 1
//        [Uber(TestGroup,  GroupName)]       _SomeFeatureMap ("_SomeFeatureMap", 2D) = "white" {}
//        [Uber(TestGroup,  GroupName)]       _SomeFeatureWidth ("_SomeFeatureWidth", float) = 0
//        [Uber(NestGroup,  BeginGroup)]      _UseNestFeature ("_UseNestFeature", int) = 1
//        [Uber(NestGroup)]                   _NestFeatureColor ("_NestFeatureColor", color) = (0.2,0.7,0.8)
//        [Uber(GrandChild, BeginGroup)]      _BeginGroundChild ("_BeginGroundChild", int) = 1
//        [Uber(GrandChild, Vector2)]         _GrandChildVector ("_GrandChildVector", vector) = (0,0,0,0)
//        [Uber(GrandChild, EndGroup)]        _EndGroundChild ("_EndGroundChild", int) = 0
//        [Uber(NestGroup,  EndGroup)]        _EndNestGroup ("_EndNestGroup", int) = 0
//        [Uber(TestGroup,  EndGroup)]        _EndSomGroup ("_EndSomGroup", int) = 0
//        [Uber(None, ARG0)]                  _UberTest0 ("_UberTestNone", range(0,1)) = 0
        
        [Uber(Face, BeginGroup)]	_BeginFaceDummy		("_", int) = 0
        [Uber(Face)]	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
    	[Uber(Face)]	_FaceTex			("Face Texture", 2D) = "white" {}
		[Uber(Face)]	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
		[Uber(Face)]	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
		[Uber(Face)]    _OutlineSoftness	("Outline Softness", Range(0,1)) = 0
    	[Uber(Face)]	_FaceDilate			("Face Dilate", Range(-1,1)) = 0
    	[Uber(Face, EndGroup)]		_EndFaceDummy		("_", int) = 0

		[Uber(Outline, BeginGroup)] _BeginOutlineDummy	("_", int) = 0
    	[Uber(Outline)] [HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
		[Uber(Outline)] _OutlineTex			("Outline Texture", 2D) = "white" {}
		[Uber(Outline)] _OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
		[Uber(Outline)] _OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
		[Uber(Outline)] _OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
		
		[Uber(Outline, EndGroup  )] _EndOutlineDummy    ("_", int) = 0
    	
    	[Uber(Unverlay, BeginToggleGroup)] _BeginUnderlayDummy ("_", int) = 0
		[Uber(Unverlay)]	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0, 0.5)
		[Uber(Unverlay)]	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
		[Uber(Unverlay)]	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
		[Uber(Unverlay)]	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
		[Uber(Unverlay)]	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0
    	[Uber(Unverlay, EndGroup)] _EndUnderlayDummy ("_", int) = 0
    	
    	[Uber(Lighting, BeginToggleGroup)] _BeginLightingDummy ("_", int) = 0
    	[Uber(Bevel, BeginGroup)] _BeginBevelDummy	("", int) = 0
    	[Uber(Bevel)] _ShaderFlags					("Flags", float) = 0
		[Uber(Bevel)] _Bevel						("Bevel", Range(0,1)) = 0.5
		[Uber(Bevel)] _BevelOffset					("Bevel Offset", Range(-0.5,0.5)) = 0
		[Uber(Bevel)] _BevelWidth					("Bevel Width", Range(-.5,0.5)) = 0
		[Uber(Bevel)] _BevelClamp					("Bevel Clamp", Range(0,1)) = 0
		[Uber(Bevel)] _BevelRoundness				("Bevel Roundness", Range(0,1)) = 0
    	[Uber(Bevel, EndGroup)] _EndBevelDummy		("_", int) = 0

    	[Uber(LocalLighting, BeginGroup)] _BeginLocalLightingDummy ("_", int) = 0
		[Uber(LocalLighting)] _LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
		[Uber(LocalLighting)] [HDR]_SpecularColor	("Specular", Color) = (1,1,1,1)
		[Uber(LocalLighting)] _SpecularPower		("Specular", Range(0,4)) = 2.0
		[Uber(LocalLighting)] _Reflectivity			("Reflectivity", Range(5.0,15.0)) = 10
		[Uber(LocalLighting)] _Diffuse				("Diffuse", Range(0,1)) = 0.5
		[Uber(LocalLighting)] _Ambient				("Ambient", Range(1,0)) = 0.5
    	[Uber(LocalLighting, EndGroup)] _EndLocalLightingDummy		("_", int) = 0

    	[Uber(Bump Map, BeginGroup)] _BeginBumpMapDummy ("_", int) = 0
		[Uber(Bump Map)] [NoScaleOffset]_BumpMap 		("Normal map", 2D) = "bump" {}
		[Uber(Bump Map)] _BumpOutline					("Bump Outline", Range(0,1)) = 0
		[Uber(Bump Map)] _BumpFace						("Bump Face", Range(0,1)) = 0
    	[Uber(Bump Map, EndGroup)] _EndBumpMapDummy		("_", int) = 0
		
    	[Uber(EnvironmentReflection, BeginGroup)] _BeginEnvironmentReflectionDummy ("_", int) = 0
		[Uber(EnvironmentReflection)] _ReflectFaceColor				("Reflection Color", Color) = (0,0,0,1)
		[Uber(EnvironmentReflection)] _ReflectOutlineColor			("Reflection Color", Color) = (0,0,0,1)
		[Uber(EnvironmentReflection)] [NoScaleOffset]_Cube			("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
		[Uber(EnvironmentReflection, Vector3)] _EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)
    	[Uber(EnvironmentReflection, EndGroup)] _EndEnvironmentReflectionDummy ("_", int) = 0
		
    	[Uber(Lighting, EndGroup)] _EndLightingDummy ("_", int) = 0

    	[Uber(Glow, BeginToggleGroup)] _BeginGlowDummy ("_", int) = 0
		[Uber(Glow)]	[HDR]_GlowColor		("Color", Color) = (0, 1, 0, 0.5)
		[Uber(Glow)]	_GlowOffset			("Offset", Range(-1,1)) = 0
		[Uber(Glow)]	_GlowInner			("Inner", Range(0,1)) = 0.05
		[Uber(Glow)]	_GlowOuter			("Outer", Range(0,1)) = 0.05
		[Uber(Glow)]	_GlowPower			("Falloff", Range(1, 0)) = 0.75
    	[Uber(Glow, EndGroup)] _EndGlowDummy ("", int) = 0

    	[Uber(Debug Setting, BeginGroup)] _BeginDebugSettingDummy ("_", int) = 0
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

		[Uber(Debug Setting)]	_StencilComp		("Stencil Comparison", Float) = 8
		[Uber(Debug Setting)]	_Stencil			("Stencil ID", Float) = 0
		[Uber(Debug Setting)]	_StencilOp			("Stencil Operation", Float) = 0
		[Uber(Debug Setting)]	_StencilWriteMask	("Stencil Write Mask", Float) = 255
		[Uber(Debug Setting)]	_StencilReadMask	("Stencil Read Mask", Float) = 255

		[Uber(Debug Setting, Enum, CullMode)]	
    							_CullMode			("Cull Mode", Float) = 0
		[Uber(Debug Setting)]	_ColorMask			("Color Mask", Float) = 15
    	[Uber(Debug Setting)]	_ScaleRatioA		("Scale RatioA", float) = 1
		[Uber(Debug Setting)]	_ScaleRatioB		("Scale RatioB", float) = 1
		[Uber(Debug Setting)]	_ScaleRatioC		("Scale RatioC", float) = 1
    	[Uber(Debug Setting, EndGroup)] _EndDebugSettingDummy ("_", int) = 0
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
