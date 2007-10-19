sampler TextureSampler : register(s0);

float2 PixelCoordsDownFilter[16] =
{
    { 1.5,  -1.5 },
    { 1.5,  -0.5 },
    { 1.5,   0.5 },
    { 1.5,   1.5 },
    { 0.5,  -1.5 },
    { 0.5,  -0.5 },
    { 0.5,   0.5 },
    { 0.5,   1.5 },
    {-0.5,  -1.5 },
    {-0.5,  -0.5 },
    {-0.5,   0.5 },
    {-0.5,   1.5 },
    {-1.5,  -1.5 },
    {-1.5,  -0.5 },
    {-1.5,   0.5 },
    {-1.5,   1.5 },
};  

float2 TexelCoordsDownFilter[16]
<
    string ConvertPixelsToTexels = "PixelCoordsDownFilter";
>;

float4 DownFilter( in float2 Tex : TEXCOORD0 ) : COLOR0
{
    float4 Color = 0;
    for (int i = 0; i < 16; i++)
    {
        Color += tex2D(TextureSampler, Tex + PixelCoordsDownFilter[i].xy );
    }
    return Color /16;
}

technique DownSample
{
    pass p0
    {
        PixelShader = compile ps_2_0 DownFilter();
        ZEnable = false;
    }
}