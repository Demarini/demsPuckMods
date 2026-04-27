using System;
using TMPro;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class Scoreboard : MonoBehaviour
{
	// Token: 0x0600034B RID: 843 RVA: 0x00013934 File Offset: 0x00011B34
	public void TurnOn()
	{
		this.minutesText.gameObject.SetActive(true);
		this.secondsText.gameObject.SetActive(true);
		this.periodText.gameObject.SetActive(true);
		this.blueScoreText.gameObject.SetActive(true);
		this.redScoreText.gameObject.SetActive(true);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00013998 File Offset: 0x00011B98
	public void TurnOff()
	{
		this.minutesText.gameObject.SetActive(false);
		this.secondsText.gameObject.SetActive(false);
		this.periodText.gameObject.SetActive(false);
		this.blueScoreText.gameObject.SetActive(false);
		this.redScoreText.gameObject.SetActive(false);
	}

	// Token: 0x0600034D RID: 845 RVA: 0x000139FC File Offset: 0x00011BFC
	public void SetTick(int tick)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)tick);
		this.minutesText.text = timeSpan.Minutes.ToString("D2");
		this.secondsText.text = timeSpan.Seconds.ToString("D2");
	}

	// Token: 0x0600034E RID: 846 RVA: 0x00013A4F File Offset: 0x00011C4F
	public void SetPeriod(int period)
	{
		this.periodText.text = period.ToString();
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00013A63 File Offset: 0x00011C63
	public void SetBlueScore(int score)
	{
		this.blueScoreText.text = score.ToString();
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00013A77 File Offset: 0x00011C77
	public void SetRedScore(int score)
	{
		this.redScoreText.text = score.ToString();
	}

	// Token: 0x04000246 RID: 582
	[Header("References")]
	[SerializeField]
	private TMP_Text minutesText;

	// Token: 0x04000247 RID: 583
	[SerializeField]
	private TMP_Text secondsText;

	// Token: 0x04000248 RID: 584
	[SerializeField]
	private TMP_Text periodText;

	// Token: 0x04000249 RID: 585
	[SerializeField]
	private TMP_Text blueScoreText;

	// Token: 0x0400024A RID: 586
	[SerializeField]
	private TMP_Text redScoreText;
}
