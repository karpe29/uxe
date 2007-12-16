sampler TextureSampler;


struct PixelInput
{
 float2 TexCoord : TEXCOORD0;
};


float4 GreyScale(PixelInput input) : COLOR
{
float4 color = tex2D( TextureSampler, input.TexCoord);
color.rgb = dot(color.rgb, float3(0.3, 0.59, 0.11));
return( color );
}


technique GreyScale
{
 pass P0
 {
  PixelShader = compile ps_2_0 GreyScale();
 }
}