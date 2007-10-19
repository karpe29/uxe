// Pixel shader extracts the brighter areas of an image.
sampler TextureSampler : register(s0);
float Threshold = 0.25F;

float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, texCoord);
    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - Threshold) / (1 - Threshold));
}

technique Bloom
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
        ZEnable = false;
    }
}