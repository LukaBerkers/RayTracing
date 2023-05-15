// Only used in Modern OpenGL

#version 330

// shader input
in vec2 uv;			        // interpolated texture coordinates
uniform sampler2D pixels;   // texture sampler

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    outputColor = texture(pixels, uv);
}