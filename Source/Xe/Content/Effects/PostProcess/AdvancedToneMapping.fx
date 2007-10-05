sampler TextureSampler : register(s0);

float exposure = 0.0;
float defog = 0;
float gamma = 1.0f / 2.2f;
float3 fogColor = { 1.0, 1.0, 1.0 };

float4 TonemapPS(float4 texcoord  : TEXCOORD0) : COLOR
{
		float3 c = tex2D(TextureSampler, texcoord);
  	c = max(0, c - defog * fogColor);
	  c *= pow(2.0f, exposure);
    // gamma correction - could use texture lookups for this
    c = pow(c, gamma);
    return float4(c.rgb, 1.0);
}

technique AdvancedToneMapping
{
	pass p0 
	{
		cullmode = none;
		ZEnable = false;
		AlphaBlendEnable = false;
		PixelShader = compile ps_2_0 TonemapPS();
	}
}