sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float Intensity1 = 1;
float Intensity2 = 1;

float Saturation1 = 1;
float Saturation2 = 1;

float3 SaturationColor = float3(0.3, 0.59, 0.11);


// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, SaturationColor);

    return lerp(grey, color, saturation);
}


float4 PixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, Saturation1) * Intensity1;
    base = AdjustSaturation(base, Saturation2) * Intensity2;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
    return base + bloom;
}

technique AdvancedCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShader();
        ZEnable = false;
    }
}