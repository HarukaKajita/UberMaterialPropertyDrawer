Shader "Test/UberProps"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Uber(Test, ARG0)]
        _UberTest ("_UberTest", int) = 0
        
        [Uber(TestGroup, BeginToggleGroup)]
        _UseSomeFeature ("_UseSomeFeature", int) = 1
        [Uber(TestGroup, GroupName)]
        _SomeFeatureMap ("_SomeFeatureMap", 2D) = "white" {}
        [Uber(TestGroup, GroupName)]
        _SomeFeatureWidth ("_SomeFeatureWidth", float) = 0
        [Uber(NestGroup, BeginGroup)]
        _UseNestFeature ("_UseNestFeature", int) = 0
        [Uber(NestGroup)]
        _NestFeatureColor ("_NestFeatureColor", color) = (0.2,0.7,0.8)
        [Uber(GrandChild, BeginGroup)]
        _BeginGroundChild ("_BeginGroundChild", int) = 0
        [Uber(GrandChild)]
        _GrandChildVector ("_GrandChildVector", vector) = (0,0,0,0)
        [Uber(GrandChild, EndToggleGroup)]
        _EndGroundChild ("_EndGroundChild", int) = 0
        [Uber(NestGroup, EndToggleGroup)]
        _EndNestGroup ("_EndNestGroup", int) = 0
        [Uber(TestGroup, EndToggleGroup)]
        _EndSomGroup ("_EndSomGroup", int) = 0
        
        [Uber(None, ARG0)]
        _UberTest0 ("_UberTestNone", range(0,1)) = 0
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
