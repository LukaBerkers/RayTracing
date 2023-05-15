// Only used in Modern OpenGL

#version 330
 
// shader input
in vec3 vPosition;		// vertex position in normalized device coordinates
in vec2 vUV;			// vertex uv coordinate

// shader output
out vec2 uv;				
 
// vertex shader
void main()
{
	// forward vertex position; will be interpolated for each fragment
	// no transformation needed because the user already provided NDC
	gl_Position = vec4(vPosition, 1.0);

	// forward vertex uv coordinate; will be interpolated for each fragment
	uv = vUV;
}