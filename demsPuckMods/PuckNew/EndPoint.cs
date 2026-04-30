using System;

// Token: 0x0200021C RID: 540
public class EndPoint : IEquatable<EndPoint>
{
	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x000447B4 File Offset: 0x000429B4
	// (set) Token: 0x06000EF9 RID: 3833 RVA: 0x000447BC File Offset: 0x000429BC
	public string ipAddress { get; set; }

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000EFA RID: 3834 RVA: 0x000447C5 File Offset: 0x000429C5
	// (set) Token: 0x06000EFB RID: 3835 RVA: 0x000447CD File Offset: 0x000429CD
	public ushort port { get; set; }

	// Token: 0x06000EFC RID: 3836 RVA: 0x000447D6 File Offset: 0x000429D6
	public EndPoint(string ipAddress, ushort port)
	{
		this.ipAddress = ipAddress;
		this.port = port;
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x000447EC File Offset: 0x000429EC
	public bool Equals(EndPoint other)
	{
		return other != null && this.ipAddress == other.ipAddress && this.port == other.port;
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x00044818 File Offset: 0x00042A18
	public override bool Equals(object obj)
	{
		EndPoint endPoint = obj as EndPoint;
		return endPoint != null && this.Equals(endPoint);
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x00044838 File Offset: 0x00042A38
	public static bool operator ==(EndPoint a, EndPoint b)
	{
		if (a == null)
		{
			return b == null;
		}
		return a.Equals(b);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x00044849 File Offset: 0x00042A49
	public static bool operator !=(EndPoint a, EndPoint b)
	{
		return !(a == b);
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00044855 File Offset: 0x00042A55
	public override int GetHashCode()
	{
		return HashCode.Combine<string, ushort>(this.ipAddress, this.port);
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x00044868 File Offset: 0x00042A68
	public override string ToString()
	{
		return string.Format("{0}:{1}", this.ipAddress, this.port);
	}
}
