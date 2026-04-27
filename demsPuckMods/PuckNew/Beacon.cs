using System;

// Token: 0x0200022D RID: 557
public class Beacon
{
	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000F6E RID: 3950 RVA: 0x00044C0A File Offset: 0x00042E0A
	// (set) Token: 0x06000F6F RID: 3951 RVA: 0x00044C12 File Offset: 0x00042E12
	public string id { get; set; }

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000F70 RID: 3952 RVA: 0x00044C1B File Offset: 0x00042E1B
	// (set) Token: 0x06000F71 RID: 3953 RVA: 0x00044C23 File Offset: 0x00042E23
	public string host { get; set; }

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000F72 RID: 3954 RVA: 0x00044C2C File Offset: 0x00042E2C
	// (set) Token: 0x06000F73 RID: 3955 RVA: 0x00044C34 File Offset: 0x00042E34
	public string fqdn { get; set; }

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000F74 RID: 3956 RVA: 0x00044C3D File Offset: 0x00042E3D
	// (set) Token: 0x06000F75 RID: 3957 RVA: 0x00044C45 File Offset: 0x00042E45
	public ushort udp_port { get; set; }

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000F76 RID: 3958 RVA: 0x00044C4E File Offset: 0x00042E4E
	// (set) Token: 0x06000F77 RID: 3959 RVA: 0x00044C56 File Offset: 0x00042E56
	public ushort tcp_port { get; set; }

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000F78 RID: 3960 RVA: 0x00044C5F File Offset: 0x00042E5F
	// (set) Token: 0x06000F79 RID: 3961 RVA: 0x00044C67 File Offset: 0x00042E67
	public Location location { get; set; }
}
