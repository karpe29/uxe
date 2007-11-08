struct VS_OUTPUT
{
    float4 Pos  : POSITION;
    float2 Tex  : TEXCOORD0;
};

texture colorTexture;
sampler colorSampler = sampler_state
{
	Texture = <colorTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = None;
};

VS_OUTPUT VS(
    float3 InPos  : POSITION,
    float2 InTex  : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    // transform the position to the screen
    Out.Pos = float4(InPos.xyz,1);
    Out.Tex = InTex;

    return Out;
}


//return the tex coordinates x and y as the red and green values 
//to add some color to the output

float4 PS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
      //return  float4(0, TexCoord.x, 0 , TexCoord.y); 
      //return  float4(0, 0, 0 , 1); 
      return tex2D(colorSampler, TexCoord);
}


technique T0
{
    pass P0
    {    
        VertexShader = compile vs_2_0 VS();
        ZEnable = false;
		ZWriteEnable = false;
        PixelShader  = compile ps_2_0 PS();
    }
}