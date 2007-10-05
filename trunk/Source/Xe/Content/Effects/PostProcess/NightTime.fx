sampler TextureSampler;


struct PixelInput
{
 float2 TexCoord : TEXCOORD0;
};


float4 NightTime(PixelInput input) : COLOR
{
 float4 color = tex2D(TextureSampler, input.TexCoord);
 color.b = color.b + color.b * .25;
 color.rg *= .15;
 return( color );
}


technique NightTime
{
 pass P0
 {
  PixelShader = compile ps_2_0 NightTime();
 }
}