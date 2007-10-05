sampler TextureSampler;


struct PixelInput
{
 float2 TexCoord : TEXCOORD0;
};


float4 GreyScale(PixelInput input) : COLOR
{
 float4 color = tex2D( TextureSampler, input.TexCoord);
 float4 gs = dot(color.rgb, float3(0.3, 0.59, 0.11));
 if (input.TexCoord.x > 0.5f)
  color = lerp(gs, color, (1 - input.TexCoord.x) * 2);
 else
  color = lerp(gs, color, input.TexCoord.x * 2);
 return( color );
}


technique GreyScale
{
 pass P0
 {
  PixelShader = compile ps_2_0 GreyScale();
 }
}