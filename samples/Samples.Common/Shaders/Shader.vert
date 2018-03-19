#version 400

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 vpos;
layout (set = 0, binding = 0) uniform AreaUB
{
	float width;
	float height;
} area;

void main()
{
	vec3 centered = vpos - vec3(0.015, -0.013, 0);
	gl_Position = vec4(centered.x*50.0, centered.y*50*area.width/area.height, 0.0, 1.0);
}
