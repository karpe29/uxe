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
float3 rotation < > ;

float Timer : Time < string UIWidget="None"; >;

float TunnelOffset < > = 100.0f;
	
float longueur : longueur < > =100.0f;


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
    float currPos=longueur-(timeNow*100)%longueur;
    float diff=(currPos-Po.x)/10;
    
    if (abs(diff)<=3.14)
    {
    bourrelet=(cos(diff)+1)/5;
    }

    //Po.x = Po.x;
    Po.y = Po.y*(1+bourrelet);
    Po.z = Po.z*(1+bourrelet);
   
   float4 quat;
    float4x4 orient;
    
    float yaw=Po.x/longueur*rotation.y;
   float pitch=Po.x/longueur*rotation.x;
   float roll=Po.x/longueur*rotation.z;
    
    float num9 = roll * 0.5f;
    float num6 = sin(num9);
    float num5 = cos(num9);
    float num8 = pitch * 0.5f;
    float num4 = sin( num8);
    float num3 = cos( num8);
    float num7 = yaw * 0.5f;
    float num2 = sin( num7);
    float num = cos( num7);
    
    quat.x = ((num * num4) * num5) + ((num2 * num3) * num6);
    quat.y = ((num2 * num3) * num5) - ((num * num4) * num6);
    quat.z = ((num * num3) * num6) - ((num2 * num4) * num5);
    quat.w = ((num * num3) * num5) + ((num2 * num4) * num6);

    
    num9 = quat.x * quat.x;
    num8 = quat.y * quat.y;
    num7 = quat.z * quat.z;
    num6 = quat.x * quat.y;
    num5 = quat.z * quat.w;
    num4 = quat.z * quat.x;
    num3 = quat.y * quat.w;
    num2 = quat.y * quat.z;
    num = quat.x * quat.w;
    
    orient._11 = 1 - (2 * (num8 + num7));
    orient._12 = 2 * (num6 + num5);
    orient._13 = 2 * (num4 - num3);
    orient._14 = 0;
    orient._21 = 2 * (num6 - num5);
    orient._22 = 1 - (2 * (num7 + num9));
    orient._23 = 2 * (num2 + num);
    orient._24 = 0;
    orient._31 = 2 * (num4 + num3);
    orient._32 = 2 * (num2 - num);
    orient._33 = 1 - (2 * (num8 + num9));
    orient._34 = 0;
    orient._41 = 0;
    orient._42 = 0;
    orient._43 = 0;
    orient._44 = 1;

    
    

    Po=mul(Po,orient);
    
    
    
    OUT.HPosition = mul(Po, WorldViewProj);
    float3 Pw = mul(Po, World).xyz;
    float3 Ln = normalize(LightPos - Pw);
    float ldn = dot(Ln,Nn);
    float diffComp = max(0,ldn);
    float3 diffContrib = SurfColor * ( diffComp * LightColor + AmbiColor);
    OUT.diffCol = float4(diffContrib,1);
    OUT.TexCoord0=IN.UV;
    OUT.TexCoord0.y = (IN.UV.y+timeNow/50)%longueur;
 
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
		VertexShader = compile vs_2_0 MrWiggleVS();
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
		VertexShader = compile vs_2_0 MrWiggleVS();
		ZEnable = true;
		ZWriteEnable = true;
		//CullMode = None;
		PixelShader = compile ps_1_1 MrWigglePS_t();
    }
}

/***************************** eof ***/
