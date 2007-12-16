sampler TextureSampler;


struct PixelInput
{
 float2 TexCoord : TEXCOORD0;
};


float4 Embossed(PixelInput input) : COLOR
{
 float sharpAmount = 15.0f;
 float4 color;
 color.rgb = 0.5f;
 color.a = 1.0f;
 color -= tex2D( TextureSampler, input.TexCoord - 0.0001) * sharpAmount;
 color += tex2D( TextureSampler, input.TexCoord + 0.0001) * sharpAmount;
 color = (color.r+color.g+color.b) / 3.0f;
 return( color );
}


technique Embossed
{
 pass P0
 {
  PixelShader = compile ps_2_0 Embossed();
 }
}