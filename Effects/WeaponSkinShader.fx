sampler uImage0 : register(s0);
float uTime : register(C0);
float2 textureSize;
float2 uWorldPosition;
bool TileHorizontal1;
bool TileHorizontal2;
bool TileHorizontal3;

//Thanks a lot for the help, Oli!!
texture materialMap;
sampler2D materialMapSampler = sampler_state
{
    Texture = (materialMap);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};

texture materialTexture;
float2 materialTextureSize;
sampler2D materialTextureSampler = sampler_state
{
    Texture = (materialTexture);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};
texture materialTexture2;
float2 materialTexture2Size;
sampler2D materialTextureSampler2 = sampler_state
{
    Texture = (materialTexture2);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};
texture materialTexture3;
float2 materialTexture3Size;
sampler2D materialTextureSampler3 = sampler_state
{
    Texture = (materialTexture3);
    AddressU = WRAP;
    AddressV = WRAP;
    MagFilter = POINT;
    MinFilter = POINT;
    MipFilter = POINT;
};

float4 FilterMyShader(float2 coords : TEXCOORD0) : COLOR0
{  
    float4 tex = tex2D(uImage0, coords);
    
    float2 tex2material1 = materialTextureSize / textureSize;
    float4 materialtex1 = tex2D(materialTextureSampler, coords / tex2material1 + (TileHorizontal1 ? tex2material1 : float2(0, 0)));
    
    float2 tex2material2 = materialTexture2Size / textureSize;
    float4 materialtex2 = tex2D(materialTextureSampler2, coords / tex2material2 + (TileHorizontal2 ? tex2material2 : float2(0, 0)));
   
    float2 tex2material3 = materialTexture3Size / textureSize;
    float4 materialtex3 = tex2D(materialTextureSampler3, coords / tex2material3 + (TileHorizontal3 ? tex2material3 : float2(0, 0)));
    
    float4 materialmap = tex2D(materialMapSampler, coords);
    
    if (materialmap.b == 1)
    {
        return float4(tex.rgb * materialtex1.rgb * 2, tex.a);
    }
    else if (materialmap.r == 1)
    {
        return float4(tex.rgb * materialtex2.rgb * 2, tex.a);
    }
    else if (materialmap.g == 1)
    {
        return float4(tex.rgb * materialtex3.rgb * 2, tex.a);
    }
    else
    {
        return float4(tex.rgb, tex.a);
    }
}

technique Technique1
{
    pass FilterMyShader
    {
        PixelShader = compile ps_2_0 FilterMyShader();
    }
}