using System;
using System.Text.Json.Serialization;

// Token: 0x02000220 RID: 544
public class PlayerMatchData : MatchData
{
	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000F2A RID: 3882 RVA: 0x000449B8 File Offset: 0x00042BB8
	[JsonIgnore]
	public int? JoinTimeoutRemainingSeconds
	{
		get
		{
			if (base.startedAt == null)
			{
				return null;
			}
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds((long)base.startedAt.Value).DateTime;
			int num = (int)(60.0 - utcNow.Subtract(dateTime).TotalSeconds);
			if (num <= 0)
			{
				return null;
			}
			return new int?(num);
		}
	}
}
