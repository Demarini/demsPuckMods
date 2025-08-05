using System;
using TMPro;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class Scoreboard : MonoBehaviour
{
	// Token: 0x06000127 RID: 295 RVA: 0x00011D18 File Offset: 0x0000FF18
	public void TurnOn()
	{
		this.minutesText.gameObject.SetActive(true);
		this.secondsText.gameObject.SetActive(true);
		this.periodText.gameObject.SetActive(true);
		this.blueScoreText.gameObject.SetActive(true);
		this.redScoreText.gameObject.SetActive(true);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00011D7C File Offset: 0x0000FF7C
	public void TurnOff()
	{
		this.minutesText.gameObject.SetActive(false);
		this.secondsText.gameObject.SetActive(false);
		this.periodText.gameObject.SetActive(false);
		this.blueScoreText.gameObject.SetActive(false);
		this.redScoreText.gameObject.SetActive(false);
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void SetTime(int time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		this.minutesText.text = timeSpan.ToString("mm");
		this.secondsText.text = timeSpan.ToString("ss");
	}

	// Token: 0x0600012A RID: 298 RVA: 0x0000799B File Offset: 0x00005B9B
	public void SetPeriod(int period)
	{
		this.periodText.text = period.ToString();
	}

	// Token: 0x0600012B RID: 299 RVA: 0x000079AF File Offset: 0x00005BAF
	public void SetBlueScore(int score)
	{
		this.blueScoreText.text = score.ToString();
	}

	// Token: 0x0600012C RID: 300 RVA: 0x000079C3 File Offset: 0x00005BC3
	public void SetRedScore(int score)
	{
		this.redScoreText.text = score.ToString();
	}

	// Token: 0x04000096 RID: 150
	[Header("References")]
	[SerializeField]
	private TMP_Text minutesText;

	// Token: 0x04000097 RID: 151
	[SerializeField]
	private TMP_Text secondsText;

	// Token: 0x04000098 RID: 152
	[SerializeField]
	private TMP_Text periodText;

	// Token: 0x04000099 RID: 153
	[SerializeField]
	private TMP_Text blueScoreText;

	// Token: 0x0400009A RID: 154
	[SerializeField]
	private TMP_Text redScoreText;
}
