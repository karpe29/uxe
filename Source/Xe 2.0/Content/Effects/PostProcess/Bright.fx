sampler TextureSampler : register(s0);
float Luminance = 0.08f;
static const float fMiddleGray = 0.18f;
static const float fWhiteCutoff = 0.8f;
float4 BrightPassFilter( in float2 Tex : TEXCOORD0 ) : COLOR0
{
    float3 ColorOut = tex2D(TextureSampler, Tex );
    ColorOut *= fMiddleGray / ( Luminance + 0.001f );
    ColorOut *= ( 1.0f + ( ColorOut / ( fWhiteCutoff * fWhiteCutoff ) ) );
    ColorOut -= 5.0f;
    ColorOut = max( ColorOut, 0.0f );
    ColorOut /= ( 10.0f + ColorOut );
    return float4( ColorOut, 1.0f );
}

technique Bright
{
    pass p0
    {
        //VertexShader = null;
        PixelShader = compile ps_2_0 BrightPassFilter();
        ZEnable = false;
    }
}