/*********************************************************************NVMH3****
File:  $Id: //sw/devtools/FXComposer/1.8/SDK/MEDIA/HLSL/MrWiggle.fx#1 $

Copyright NVIDIA Corporation 2004
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY LOSS)
ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF NVIDIA HAS
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

Comments:
    Simple sinusoidal vertex animation on a phong-shaded plastic surface.
		The highlight is done in VERTEX shading -- not as a texture.
		Textured/Untextured versions are supplied
	Note that the normal is also easily distorted, based on the fact that
		dsin(x)/dx is just cos(x)
    Do not let your kids play with this shader, you will not get your
		computer back for a while.

******************************************************************************/

float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "object";
    string ScriptOrder = "standard";
    string ScriptOutput = "color";
    string Script = "Technique=Technique?Untextured:Textured;";
> = 0.8;

/************* TWEAKABLES **************/

float4x4 WorldIT : WorldInverseTranspose < string UIWidget="None"; >;
float4x4 WorldViewProj : WorldViewProjection < string UIWidget="None"; >;
float4x4 World : World < string UIWidget="None"; >;
float4x4 ViewI : ViewInverse < string UIWidget="None"; >;

float Timer : Time < string UIWidget="None"; >;

float TunnelOffset < > = 100.0f;
	

float TimeScale <
    string UIWidget = "slider";
    string UIName = "Speed";
    float UIMin = 0.1;
    float UIMax = 10;
    float UIStep = .1;
> = 4.0f;

float Horizontal <
    string UIWidget = "slider";
    float UIMin = 0.001;
    float UIMax = 10;
    float UIStep = 0.01;
> = 0.5f;

float Vertical <
    string UIWidget = "slider";
    float UIMin = 0.001;
    float UIMax = 10.0;
    float UIStep = 0.1;
> = 0.5;

float currPos ;

float3 LightPos : Position <
    string Object = "PointLight";
    string Space = "World";
> = {-10.0f, 10.0f, -10.0f};

float3 LightColor <
    string UIName =  "Lamp";
    string UIWidget = "Color";
> = {1.0f, 1.0f, 1.0f};

float3 AmbiColor : Ambient <
    string UIName =  "Ambient Light";
    string UIWidget = "Color";
> = {0.2f, 0.2f, 0.2f};

float3 SurfColor : DIFFUSE <
    string UIName =  "Surface";
    string UIWidget = "Color";
> = {0.9f, 0.9f, 0.9f};

float SpecExpon : SpecularPower <
    string UIWidget = "slider";
    float UIMin = 1.0;
    float UIMax = 128.0;
    float UIStep = 1.0;
    string UIName =  "Specular Power";
> = 5.0;

texture colorTexture : DIFFUSE <
    string ResourceName = "default_color.dds";
    string UIName =  "Color Texture (if used)";
    string ResourceType = "2D";
>;

sampler2D colorSampler = sampler_state {
	Texture = <colorTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = None;
};

/************* DATA STRUCTS **************/

/* data from application vertex buffer */
struct appdata {
    float4 Position	: POSITION;
    float4 UV		: TEXCOORD0;
    float4 Normal	: NORMAL;
};

/* data passed from vertex shader to pixel shader */
struct vertexOutput {
    float4 HPosition	: POSITION;
    float4 TexCoord0	: TEXCOORD0;
    float4 diffCol	: COLOR0;
    float4 specCol	: COLOR1;
};

/*********** vertex shader ******/

vertexOutput MrWiggleVS(appdata IN) {
    vertexOutput OUT;
    float3 Nn = normalize(mul(IN.Normal, WorldIT).xyz);
    float timeNow = Timer*TimeScale;
    float4 Po = IN.Position;
    /*float iny = Po.y * Vertical + timeNow;
    float wiggleX = sin(iny) * Horizontal;
    float wiggleY = cos(iny) * Horizontal; // deriv
    //Nn.y = Nn.y + wiggleY;*/
    Nn = normalize(Nn);
    
    
    float bourrelet=0;
    float currPos=1000-(timeNow*100)%1000;
    float diff=(currPos-Po.x)/10;
    
    if (abs(diff)<=1.57)
    {
    bourrelet=cos(diff)/5;
    }
    
    //Po.x = Po.x;
    Po.y = Po.y*(1+bourrelet);
    Po.z = Po.z*(1+bourrelet);
   
    
    OUT.HPosition = mul(Po, WorldViewProj);
    float3 Pw = mul(Po, World).xyz;
    float3 Ln = normalize(LightPos - Pw);
    float ldn = dot(Ln,Nn);
    float diffComp = max(0,ldn);
    float3 diffContrib = SurfColor * ( diffComp * LightColor + AmbiColor);
    OUT.diffCol = float4(diffContrib,1);
    
    OUT.TexCoord0 = IN.UV;
    TunnelOffset = TunnelOffset + 1;
    //OUT.TexCoord0.x = IN.UV.x + TunnelOffset;
    //OUT.TexCoord0.y = IN.UV.y + TunneOffset;
    //OUT.TexCoord0.z = IN.UV.z + 1;
    
    float3 Vn = normalize(ViewI[3].xyz - Pw);
    float3 Hn = normalize(Vn + Ln);
    float hdn = pow(max(0,dot(Hn,Nn)),SpecExpon);
    OUT.specCol = float4(hdn * LightColor,1);
    return OUT;
}

/********* pixel shader ********/

float4 MrWigglePS_t(vertexOutput IN) : COLOR {
    float4 result = IN.diffCol * tex2D(colorSampler, IN.TexCoord0);// + IN.specCol;
    return result;
    //return float4( 1, 1, 1, 1 );
    //return tex2D( colorSampler, IN.TexCoord0 );
}

/*************/

technique Untextured <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
		VertexShader = compile vs_1_1 MrWiggleVS();
		ZEnable = true;
		ZWriteEnable = true;
		//CullMode = None;
		// Don't need a pixel shader for a super-simple surface
		//SpecularEnable = true;
		//ColorArg1[ 0 ] = Diffuse;
		//ColorOp[ 0 ]   = SelectArg1;
		//ColorArg2[ 0 ] = Specular;
		//AlphaArg1[ 0 ] = Diffuse;
		//AlphaOp[ 0 ]   = SelectArg1;
    }
}

technique Textured <
	string Script = "Pass=p0;";
> {
    pass p0 <
	string Script = "Draw=geometry;";
> {		
		VertexShader = compile vs_1_1 MrWiggleVS();
		ZEnable = true;
		ZWriteEnable = true;
		//CullMode = None;
		PixelShader = compile ps_1_1 MrWigglePS_t();
    }
}

/***************************** eof ***/
