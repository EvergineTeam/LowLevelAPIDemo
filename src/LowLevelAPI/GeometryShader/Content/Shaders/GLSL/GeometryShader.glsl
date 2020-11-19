  #version 330
  #extension GL_ARB_geometry_shader4 : enable

uniform Matrices
{
	mat4 worldViewProj;
};

in GS_IN
{
	vec4 vcolor;
} gsInput[3];

out PS_IN
{
	vec4 color;
} gsOutput;

layout(triangles) in;
layout(triangle_strip, max_vertices = 93) out;
void main()
  {
    int i;
    vec4 vertex;
   
    for(i = 0; i < 3; i++)
    {
      gl_Position = gl_PositionIn[i];
	  gsOutput.color = gsInput[i].vcolor;	  
      EmitVertex();
    }

    EndPrimitive();

	for (int i = 1; i <= 10; i++)
	{
		for (int j = 0; j < 3; j++)
		{
			gl_Position = worldViewProj * (gl_PositionIn[j] + vec4(0, i / 10.0, 0, 0));			
			gsOutput.color = gsInput[j].vcolor;			
			EmitVertex();
		}

		EndPrimitive();
	}
}
