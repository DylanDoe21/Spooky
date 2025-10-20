sampler2D uImage0 : register(s0); // Main noise
sampler2D uImage1 : register(s1); // Secondary noise
sampler2D uImage2 : register(s2); // Gaps

float uTime; // Multiply time by whatever for speed
float uScale = 1; // Noise scale, don't set too high
float uIntensity = 1;
float uExponent = 3;
float uOpacityTotal;
float4 uColor = float4(1, 1, 1, 1);

float4 main(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage1, uScale * (uv + float2(0, uTime * -.1)));
    float cA = max(c.r, max(c.g, c.b));

    float4 c2 = tex2D(
            uImage0, uScale * (uv + float2(uTime, sin(uv.x * 3.14 * 2) * 0.05 - uTime * 0.2) -
                float2(0, cA * 0.1)))
        * tex2D(uImage0,
                uScale * (1.2 * uv + float2(uTime * 0.9, -uTime * 0.02) - float2(0, cA * 0.05)));

    float4 c3 = tex2D(uImage1, uScale * (uv + float2(uTime * 0.7, 0) - float2(0, cA * 0.025))) * 2;

    float c2A = max(c2.r, max(c2.g, c2.b));
    float c3A = max(c3.r, max(c3.g, c3.b));

    float4 c4 = tex2D(
        uImage1, uScale * (
            float2(uTime * 0.3, sin(uv.x * 3.14 * 4 + c3A) * (0.02 + c3A * 0.02)) + c2A * 0.02 + uv *
            float2(.5, 1))) * 0.5;

    float4 c5 = tex2D(uImage2, uScale * (float2(1, 2) * uv + float2(uTime * 0.15, 0))) * 0.15;

    float4 col = c2 * c3 + c4 * 0.1;
    float4 final = col * uIntensity - c5 * uIntensity;
    return final * pow(1 - length(0.5 - uv), uExponent) * uOpacityTotal * uColor;
}

technique Technique1
{
    pass StripShader
    {
        PixelShader = compile ps_3_0 main();   
    
    
    }
}
