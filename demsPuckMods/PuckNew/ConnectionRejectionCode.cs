using System;

// Token: 0x020000CF RID: 207
public enum ConnectionRejectionCode
{
	// Token: 0x040003E6 RID: 998
	Unreachable,
	// Token: 0x040003E7 RID: 999
	ServerFull,
	// Token: 0x040003E8 RID: 1000
	TimedOut,
	// Token: 0x040003E9 RID: 1001
	Banned,
	// Token: 0x040003EA RID: 1002
	NotWhitelisted,
	// Token: 0x040003EB RID: 1003
	MissingPassword,
	// Token: 0x040003EC RID: 1004
	InvalidPassword,
	// Token: 0x040003ED RID: 1005
	MissingMods,
	// Token: 0x040003EE RID: 1006
	Unknown
}
