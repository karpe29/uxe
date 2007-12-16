// must be an advanced shader
//shockEffect.Parameters["xcenter"].SetValue((float)Math.Cos(deltaFrame) * 0.2f + 0.4f);
//shockEffect.Parameters["ycenter"].SetValue((float)Math.Sin(deltaFrame) * 0.2f + 0.3f);
//
//shockEffect.Parameters["width"].SetValue((float)alphaFrame * 0.75f);
//shockEffect.Parameters["mag"].SetValue((1.0f - (float)alphaFrame));
// alpha frame being the totalsconds gametime
// delta frame would be elapsed seconds, or something similar.

// shockwave.fx
sampler samplerState; 



float2 center = 0.2; // 0.5 = middle screen

float mag = 0.5;
float width = 0.5;

#define INNER		0.2                                       
                                                          
#define OUTER		0.1                                       
                                                          
#define IREL		0.5 //1-(INNER/OUTER)                     
                                                          
#define INTENSITY	15.0                                    
                                                          
                                                          
                                                          
float4 Shockwave(float2 tex : TEXCOORD0) : COLOR0      
                                                          
{                                                         
                                                          
	float4 col = 0.0;                                       
                                                          
                                                          
                                                          
	float2 dif = tex - center;                              
                                                          
	float d = length(dif) - width;                          
                                                          
	                                                        
                                                          
	if (d < OUTER && d > -INNER)                            
                                                          
	{		                                                    
                                                          
		float t = OUTER - abs(d) * (1.0 - ((d < 0.0) * IREL));
                                                          
		col = tex2D(samplerState, tex - dif * t * mag);       
                                                          
		col.a = t * INTENSITY;                                
                                                          
	}                                                       
                                                          
	                                                        
                                                          
	return col;                                             
                                                          
}                                                         
technique Shockwave { 
    pass P0{ 
        PixelShader = compile ps_2_0 Shockwave(); 
    } 
    
}