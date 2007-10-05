sampler TextureSampler : register(s0);
float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the original image color.
    return 1.0F - tex2D(TextureSampler, texCoord);
} 

technique ColorInverse
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
        ZEnable = false;
    }
}