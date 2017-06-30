RWStructuredBuffer<uint> _AppearCounts;

//================================================================================
// Calc
//================================================================================
[numthreads(1,1,1)]
void Calc (uint3 id : SV_DispatchThreadID)
{	
	float result = rand();

	uint num, stride;
	_AppearCounts.GetDimensions(num, stride);

	_AppearCounts[(uint)(result * num)]++;
}


//================================================================================
// Draw
//================================================================================
RWTexture2D<float4> _OutputTexture;

[numthreads(8,8,1)]
void Draw (uint2 id : SV_DispatchThreadID)
{	
	uint num, stride;
	_AppearCounts.GetDimensions(num, stride);

	uint appear_count = _AppearCounts[id.x];

	uint current = (uint)(_RandData[0].current * num);
	float4 color = ((id.x == current) || (appear_count > id.y)) ? (1).xxxx : float4(0,0,0,1);
	_OutputTexture[id] = color;
}
