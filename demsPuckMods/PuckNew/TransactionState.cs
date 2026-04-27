using System;

// Token: 0x0200008A RID: 138
public struct TransactionState
{
	// Token: 0x0600048E RID: 1166 RVA: 0x00018D25 File Offset: 0x00016F25
	public bool Equals(TransactionState other)
	{
		return this.Phase == other.Phase;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00018D38 File Offset: 0x00016F38
	public override bool Equals(object obj)
	{
		if (obj is TransactionState)
		{
			TransactionState other = (TransactionState)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00018D5D File Offset: 0x00016F5D
	public override int GetHashCode()
	{
		return HashCode.Combine<TransactionPhase>(this.Phase);
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00018D6A File Offset: 0x00016F6A
	public override string ToString()
	{
		return string.Format("Phase: {0}", this.Phase);
	}

	// Token: 0x040002D5 RID: 725
	public TransactionPhase Phase;
}
