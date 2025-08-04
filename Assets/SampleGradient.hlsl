#ifndef VECTOR4_TO_TEXTURE_INCLUDED
#define VECTOR4_TO_TEXTURE_INCLUDED



void SampleGradient_float(float inputValue,
                   float4 color1,
                   float4 color2,
                   float4 color3,

                   float location1,
                   float location2,
                   float location3,

                   out float4 outFloat)
{
    if(inputValue<location1)
    {
        outFloat = color1;
    }
    else if(inputValue<location2)
    {
        float pos = (inputValue-location1)/(location2-location1);
        outFloat = lerp(color1, color2, pos);
    }
    else if (inputValue < location3)
    {
        float pos = (inputValue - location2) / (location3 - location2);
        outFloat = lerp(color2, color3, pos);
    }
    else
    {
        outFloat = color3;
    }
}

#endif // VECTOR4_TO_TEXTURE_INCLUDED
