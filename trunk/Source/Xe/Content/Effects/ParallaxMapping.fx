/*********************************************************************NVMH3****
File:  $Id: //sw/devtools/ShaderLibrary/1.0/HLSL/relief_mapping.fx#3 $

Copyright NVIDIA Corporation 2007
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY LOSS)
ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF NVIDIA HAS
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.

% This material shows and compares results from four popular and
% advanced schemes for emulating displaement mapping.  They are:
% Relief Mapping, Parallax Mapping, Normal Mapping, and Relief
% Mapping with Shadows.  Original File by Fabio Policarpo.

keywords: material bumpmap
date: 070305

******************************************************************************/

float TileCount <
	string UIName = "Tile Repeat";
	string UIWidget = "slider";
	float UIMin = 1.0;
	float UIStep = 1.0;
	float UIMax = 32.0;
> = 1;

float Depth <
	string UIName = "Depth";
	string UIWidget = "slider";
	float UIMin = 0.0f;
	float UIStep = 0.001f;
	float UIMax = 0.25f;
> = 0.1;

float3 AmbiColor <
	string UIName = "Ambient";
	string UIWidget = "color";
> = {0.2,0.2,0.2};

float3 DiffColor <
	string UIName = "Diffuse";
	string UIWidget = "color";
> = {1,1,1};

float3 SpecColor <
	string UIName = "Specular";
	string UIWidget = "color";
> = {0.75,0.75,0.75};

float PhongExp <
    string UIName = "Phong Exponent";
	string UIWidget = "slider";
	float UIMin = 8.0f;
	float UIStep = 8;
	float UIMax = 256.0f;
> = 128.0;

float4 LampPos : POSITION <
	string UIName="Light Position";
    string Object = "PointLight";
    string Space = "World";	
> = { -150.0, 200.0, -125.0, 1 };

/************* UNTWEAKABE VALUES *******/

float4x4 WvpXf : WorldViewProjection <string UIWidget="none";>;
float4x4 WorldViewXf : WorldView <string UIWidget="none";>;
float4x4 ViewXf : View <string UIWidget="none";>;
// float4x4 WorldIXf : WorldInverse <string UIWidget="none";>; // unused

/*********** TEXTURES ***************/

texture ColorTex : DIFFUSE <
	string UIName = "Color Texture";
    string ResourceName = "rockwall.jpg";
    string ResourceType = "2D";
>;

texture ReliefTex : NORMAL <
	string UIName = "Normal-Map Texture";
    string ResourceName = "rockwall.tga";
    string ResourceType = "2D";
>;

sampler2D ColorSamp = sampler_state {
	Texture = <ColorTex>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

sampler2D ReliefSamp = sampler_state {
	Texture = <ReliefTex>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

/********** CONNECTOR STRUCTURES *****************/

struct AppVertexData 
{
    float4 pos		: POSITION;
    float4 color	: COLOR0;
    float3 normal	: NORMAL;
    float2 txcoord	: TEXCOORD0;
    float3 tangent	: TANGENT0;
    float3 binormal	: BINORMAL0;
};

struct VertexOutput
{
    float4 hpos		: POSITION;
    float4 color	: COLOR0;
    float2 UV		: TEXCOORD0;
    float3 vpos		: TEXCOORD1;
    float3 tangent	: TEXCOORD2;
    float3 binormal	: TEXCOORD3;
    float3 normal	: TEXCOORD4;
    float4 lightpos	: TEXCOORD5;
};

/*** SHADER FUNCTIONS **********************************************/

VertexOutput view_space(AppVertexData IN)
{
	VertexOutput OUT = (VertexOutput)0;

	// vertex position in object space
	float4 Po=float4(IN.pos.xyz,1.0);

	// compute modelview rotation only part
	float3x3 modelviewrot;
	modelviewrot[0]=WorldViewXf[0].xyz;
	modelviewrot[1]=WorldViewXf[1].xyz;
	modelviewrot[2]=WorldViewXf[2].xyz;

	// vertex position in clip space
	OUT.hpos=mul(Po,WvpXf);

	// vertex position in view space (with model transformations)
	OUT.vpos=mul(Po,WorldViewXf).xyz;

	// light position in view space
	float4 lp=float4(LampPos.xyz,1);
	OUT.lightpos=mul(lp,ViewXf);

	// tangent space vectors in view space (with model transformations)
	OUT.tangent=mul(IN.tangent,modelviewrot);
	OUT.binormal=mul(IN.binormal,modelviewrot);
	OUT.normal=mul(IN.normal,modelviewrot);
	
	// copy color and texture coordinates
	OUT.color=IN.color;
	OUT.UV = TileCount * IN.txcoord.xy;

	return OUT;
}

/************ PIXEL SHADERS ******************/

float4 normal_map(
	VertexOutput IN,
	uniform sampler2D texmap,
	uniform sampler2D reliefmap) : COLOR
{
	float2 uv=IN.UV; // *TileCount;
	float4 normal=tex2D(reliefmap,uv);
	normal.xyz-=0.5; 
	
	// transform normal to world space
	normal.xyz=normalize(normal.x*IN.tangent-normal.y*IN.binormal+normal.z*IN.normal);
	
	// color map
	float4 color=tex2D(texmap,uv);

	// view and light directions
	float3 v = normalize(IN.vpos);
	float3 l = normalize(IN.lightpos.xyz-IN.vpos);

	// compute diffuse and specular terms
	float att=saturate(dot(l,IN.normal.xyz));
	float diff=saturate(dot(l,normal.xyz));
	float spec=saturate(dot(normalize(l-v),normal.xyz));

	// compute final color
	float4 finalcolor;
	finalcolor.xyz=AmbiColor*color.xyz+
		att*(color.xyz*DiffColor.xyz*diff+SpecColor.xyz*pow(spec,PhongExp));
	finalcolor.w=1.0;

	return finalcolor;
}

float4 parallax_map(
	VertexOutput IN,
	uniform sampler2D texmap,
	uniform sampler2D reliefmap) : COLOR
{
   	// view and light directions
	float3 v = normalize(IN.vpos);
	float3 l = normalize(IN.lightpos.xyz-IN.vpos);

	float2 uv = IN.UV; // *TileCount;

	// parallax code
	float3x3 tbn = float3x3(IN.tangent,IN.binormal,IN.normal);
	float height = tex2D(reliefmap,uv).w * 0.06 - 0.03;
	uv += height * mul(tbn,v);

	// normal map
	float4 normal=tex2D(reliefmap,uv);
	normal.xyz-=0.5; 
	
	// transform normal to world space
	normal.xyz=normalize(normal.x*IN.tangent-normal.y*IN.binormal+normal.z*IN.normal);

	// color map
	float4 color=tex2D(texmap,uv);

	// compute diffuse and specular terms
	float att=saturate(dot(l,IN.normal.xyz));
	float diff=saturate(dot(l,normal.xyz));
	float spec=saturate(dot(normalize(l-v),normal.xyz));

	// compute final color
	float4 finalcolor;
	finalcolor.xyz=AmbiColor*color.xyz+
		att*(color.xyz*DiffColor.xyz*diff+SpecColor.xyz*pow(spec,PhongExp));
	finalcolor.w=1.0;

	return finalcolor;
}

technique normal_mapping
{
    pass p0 
    {
    	CullMode = CW;
		VertexShader = compile vs_3_0 view_space();
		PixelShader  = compile ps_3_0 normal_map(ColorSamp,ReliefSamp);
    }
}

technique parallax_mapping
{
    pass p0 
    {
    	CullMode = CW;
		VertexShader = compile vs_3_0 view_space();
		PixelShader  = compile ps_3_0 parallax_map(ColorSamp,ReliefSamp);
    }
}
