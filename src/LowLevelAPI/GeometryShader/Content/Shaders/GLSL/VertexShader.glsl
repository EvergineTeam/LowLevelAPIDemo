#version 330

layout (location = 0) in vec4 Position0;
layout (location = 1) in vec4 Color0;

out GS_IN
{
	vec4 vcolor;
} vsOutput;

void main()
{    
    gl_Position = Position0;
	vsOutput.vcolor = Color0;	
}