
//////////////////////////////////////////////////////////////////////////////
//                                                                            //
//    NemoKradBumpTerrain.fx Terrain shader by C.Humphrey 02/05/2007          //
//                                                                            //
//    This shader is based on terrain shaders by Riemer and Frank Luna.       //
//  Riemer: http://www.riemers.net                                            //
//  Frank Luna: http://www.moon-labs.com                                    //
//                                                                            //
//    http://randomchaos.co.uk                                                //
//    http://randomchaosuk.blogspot.com                                        //
//                                                                            //
// 25/06/2007 - Adde bumpmap functionlaity                                    //
//                                                                            //
//////////////////////////////////////////////////////////////////////////////

float4x4 wvp : WorldViewProjection;
float4x4 world : World;
float3 LightPosition : LightDirection;
float3 EyePosition : CAMERAPOSITION;

// Do we want to use bump map feature?
bool bumpOn = true;

// Texture size moifier 1 = no change.
float tileSizeMod = 1;

// Terrain Textures.
texture LayerMap0;

// Terrain Normals for above texture.
texture BumpMap0;

// Normal samplers
sampler BumpMap0Sampler = sampler_state
{
   Texture = <BumpMap0>;
   MinFilter = Linear;
   MagFilter = Linear;
   MipFilter = Linear;
   AddressU = mirror;
   AddressV = mirror;
};

// Texture Samplers
sampler LayerMap0Sampler = sampler_state
{
   Texture   = <LayerMap0>;
   MinFilter = LINEAR;
   MagFilter = LINEAR;
   MipFilter = LINEAR;
   AddressU  = mirror;
   AddressV  = mirror;
};


// Vertex Shader input structure.
struct VS_INPUT
{
   float4 posL         : POSITION0;
   float3 normalL      : NORMAL0;
   float4 tiledTexC    : TEXCOORD0;
   float4 TextureWeights : TEXCOORD1;
   float3 Tangent : TANGENT;
};

// Vertex Shader output structure/Pixel Shaer input structure
struct VS_OUTPUT
{
   float4 posH         : POSITION0;
   float  shade        : TEXCOORD0;
   float4 tiledTexC    : TEXCOORD1;  
   float4 Light : TEXCOORD3;
   float3 lView : TEXCOORD4;
};

// Vetex Shader
VS_OUTPUT BumpVS(VS_INPUT input)
{
   // Clean the output structure.
   VS_OUTPUT Out = (VS_OUTPUT)0;
       
   // Calculate tangent space.
   float3x3 worldToTangentSpace;
   worldToTangentSpace[0] = mul(input.Tangent,world);
   worldToTangentSpace[1] = mul(cross(input.Tangent,input.normalL),world);
   worldToTangentSpace[2] = mul(input.normalL,world);    
   
   // Get world pos for texture and normal.
   float4 PosWorld = mul(input.posL,world);    
   
   // Get light position.
   Out.Light.xyz = LightPosition;
   Out.Light.w = 1;
   
   // Set position for pixel shader
   Out.posH  = mul(input.posL, wvp);        
   
   // Set lighting.
   Out.shade = saturate(saturate(dot(input.normalL, normalize(LightPosition))));    
   
   // Set view direction for normals.
   Out.lView = mul(worldToTangentSpace,EyePosition - Out.posH);    
   
   // Set tile TexCoord.
   Out.tiledTexC = input.tiledTexC * tileSizeMod;
       
   
   return Out;
}
// Output to screen.
struct PixelToFrame
{
   float4 Color : COLOR0;
};

// Pixel shader.
PixelToFrame BumpPS(VS_OUTPUT input) : COLOR
{
   // Clean output structure.
   PixelToFrame Out = (PixelToFrame)0;    
   
   // Get pixel color.
   float4 Col = tex2D(LayerMap0Sampler, input.tiledTexC);
   
   // Set light directon amd view direction.
   float4 LightDir = normalize(input.Light);
   float3 ViewDir = normalize(input.lView);
   
   // Get prominent normal.    
   float2 nearTextureCoords = input.tiledTexC*3;
   float3 Normal;
   Normal = tex2D(BumpMap0Sampler,nearTextureCoords);

   //Normal = 2 * Normal - 1.0;    // ?
       
   // Set diffuse, reflection and specular effect for Normal.
   float Diffuse = saturate(dot(Normal,LightDir));    
   float Reflect = normalize(2 * Diffuse * Normal - LightDir);
   float Specular = min(pow(saturate(dot(Reflect,ViewDir)),3),Col.w);    
   
   float4 final;
   
   // Do color calculation depending if bump feature is on or off.
   if(bumpOn)
       final = (0.2 * Col * (Diffuse + Specular)) * (input.shade * 12);
   else
       final = Col * input.shade;

   Out.Color = final;
   
   return Out;
}

technique SimpleBumpMapping
{
   pass P0
   {
       vertexShader = compile vs_2_0 BumpVS();
       pixelShader  = compile ps_2_0 BumpPS();
   }    
}