sampler NormalSampler : register(s0);
float2 PixelKernel[4] =
{
    { 0,  1},
    { 1,  0},
    { 0, -1},
    {-1,  0}
};

float2 TexelKernel[4] =
{
    { 0,  1},
    { 1,  0},
    { 0, -1},
    {-1,  0}
};

float3 LuminanceConv = { 0.2125f, 0.7154f, 0.0721f };   

float4 _PixelShader( float2 Tex : TEXCOORD0) : COLOR0
{
    float4 Orig = tex2D(NormalSampler, Tex );
    float4 Sum = 0;
    for( int i = 0; i < 4; i++ )
    {
        Sum += saturate( 1 - dot( Orig.xyz, tex2D(NormalSampler, Tex + TexelKernel[i] ).xyz ) );
    }
    return Sum;
}
technique NormalEdgeDetect
{
    pass p0
    {
        VertexShader = null;
        PixelShader = compile ps_2_0 _PixelShader();
        ZEnable = false;
    }
}