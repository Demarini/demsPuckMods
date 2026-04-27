using System;

// Token: 0x02000204 RID: 516
public class Response<TSuccessData, TErrorData>
{
	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000ECE RID: 3790 RVA: 0x000446E3 File Offset: 0x000428E3
	// (set) Token: 0x06000ECF RID: 3791 RVA: 0x000446EB File Offset: 0x000428EB
	public bool success { get; set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000ED0 RID: 3792 RVA: 0x000446F4 File Offset: 0x000428F4
	// (set) Token: 0x06000ED1 RID: 3793 RVA: 0x000446FC File Offset: 0x000428FC
	public TSuccessData data { get; set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000ED2 RID: 3794 RVA: 0x00044705 File Offset: 0x00042905
	// (set) Token: 0x06000ED3 RID: 3795 RVA: 0x0004470D File Offset: 0x0004290D
	public TErrorData errorData { get; set; }
}
