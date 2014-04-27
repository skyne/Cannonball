sampler TextureSampler : register(s0);
sampler SecondTextureSampler : register(s1);

float4 desaturate(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    float4 sTex = tex2D(SecondTextureSampler, texCoord);

    tex.rgb *= sTex.rgb;

    return tex;
}

technique Desaturate
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 desaturate();
    }
}