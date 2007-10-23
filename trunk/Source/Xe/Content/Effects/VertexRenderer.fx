struct VS_OUTPUT
{
    float4 Pos  : POSITION;
    float2 Tex  : TEXCOORD0;
};

texture colorTexture : DIFFUSE;

VS_OUTPUT VS(
    float3 InPos  : POSITION,
    float2 InTex  : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    // transform the position to the screen
    Out.Pos = float4(InPos,1);
    Out.Tex = InTex;

    return Out;
}


float4 PS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
      return  float4(TexCoord.x, TexCoord.y, 0 , 1); 
}


technique T0
{
    pass P0
    {          
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}