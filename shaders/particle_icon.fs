#version 140

// particle_icon.fs

#ifdef GL_ES
precision mediump float;
#endif

uniform sampler2D Texture;
uniform vec4 Time;

in vec2 v_TexCoord;
in vec4 v_ColorPrimary;
in vec4 v_ColorSecondary;
in float v_SelectedState;

out vec4 out_FragColor;

void main() 
{
    vec3 anim = v_ColorPrimary.xyz;
    vec4 texel = texture(Texture, v_TexCoord).bgra; // webkit texture is bgra

    if (v_ColorPrimary.w == 0.0)
        out_FragColor = texel;
    else
    {
		float alpha = texel.a;
		
        if (v_SelectedState > 0 && texel.b > 0.1 && texel.b < 0.15) {
			float x = v_ColorSecondary.x - 0.55;
			float y = v_ColorSecondary.y - 0.50;
			float t = abs(y / (sin(Time.x*3)*1.5));
			if( x > 0 && x > t ){
				alpha = 0.0;
			}else{
				if( x > - 0.025 && x > t - 0.025 ){
					anim = vec3(1.0, 1.0, 1.0);					
				}else{
					if( x > - 0.1 && x > t - 0.1 ){
						anim = v_ColorPrimary.xyz * 0.5;
					}
				}
			}
        }
		
        // horrible, horrible hack to work aground webkit png handling
        texel.rgb = clamp((texel.rgb - 0.27) / 0.7, 0.0, 1.0);

        vec3 mixColor = v_SelectedState > 0.0 ? vec3(1.0, 1.0, 1.0) : vec3(0.0, 0.0, 0.0);
        vec3 color = texel.r * mix(mixColor, anim, pow(texel.g, 1.0 / 2.2) / (texel.a + 0.00001));

        // check for hover
        if (abs(v_SelectedState) > 1.5)
        {
            float mask = pow(texel.r, 1.0 / 2.2);
            alpha = mix(min(1.0, 2.0 * alpha), alpha, mask);
            color = mix(vec3(1.0, 1.0, 1.0), color, mask);
        }

        out_FragColor = vec4(color, alpha * v_ColorPrimary.a);
    }
}
