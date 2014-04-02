Shader "Custom/PlanePainter" 
{
	Properties 
	{
		_PaintPoint("Paint Point", Vector) = (0,0,0,0)
		_PaintRaduis("Paint Raduis", Float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Lambert

			float4 _PaintPoint;
			float _PaintRaduis;

			struct Input 
			{
				float3 worldPos;
			};

			void surf(Input IN, inout SurfaceOutput o) 
			{
				float dist = distance(IN.worldPos, _PaintPoint.xyz);
				float3 col = float3(1);
				
				if(dist < _PaintRaduis)
				{
					col = float3(0);
				}
				
				o.Emission = col;
				o.Alpha = 1.0;
			}
		ENDCG
	} 
	FallBack "Self-Illumin/VertexLit"
}
