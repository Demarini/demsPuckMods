using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public static class StringUtils
{
	// Token: 0x06000E6B RID: 3691 RVA: 0x00043256 File Offset: 0x00041456
	static StringUtils()
	{
		StringUtils.LoadProfanityWords();
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00043260 File Offset: 0x00041460
	private static void LoadProfanityWords()
	{
		try
		{
			string[] array = JsonSerializer.Deserialize<string[]>(Resources.Load<TextAsset>("profanity_words").text, null);
			string[] value = (from w in array
			where !string.IsNullOrWhiteSpace(w)
			orderby w.Length descending
			select Regex.Escape(w)).ToArray<string>();
			StringUtils.profanityRegex = new Regex("(?<![a-zA-Z])(" + string.Join("|", value) + ")(?![a-zA-Z])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Debug.Log(string.Format("[StringUtils] Loaded {0} profanity words and compiled regex", array.Length));
		}
		catch (Exception ex)
		{
			Debug.LogError("[StringUtils] Error loading profanity words asset: " + ex.Message);
		}
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00043360 File Offset: 0x00041560
	public static string FilterStringNotLetters(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		foreach (char c in text)
		{
			if (char.IsLetter(c))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x000433B4 File Offset: 0x000415B4
	public static string FilterStringSpecialCharacters(string text, string[] characterWhitelist = null, string[] characterBlacklist = null)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(text);
		while (textElementEnumerator.MoveNext())
		{
			string textElement = textElementEnumerator.GetTextElement();
			if (characterBlacklist == null || !characterBlacklist.Contains(textElement))
			{
				UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(textElement, 0);
				if (textElement.All(new Func<char, bool>(char.IsLetterOrDigit)) || textElement.All(new Func<char, bool>(char.IsWhiteSpace)) || unicodeCategory == UnicodeCategory.ConnectorPunctuation || unicodeCategory == UnicodeCategory.DashPunctuation || unicodeCategory == UnicodeCategory.OpenPunctuation || unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.FinalQuotePunctuation || unicodeCategory == UnicodeCategory.OtherPunctuation || unicodeCategory == UnicodeCategory.MathSymbol || unicodeCategory == UnicodeCategory.CurrencySymbol || (characterWhitelist != null && characterWhitelist.Contains(textElement)))
				{
					stringBuilder.Append(textElement);
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00043478 File Offset: 0x00041678
	public static string FilterStringProfanity(string text, bool replaceWithStars = false)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (StringUtils.profanityRegex == null)
		{
			return text;
		}
		string text2;
		if (replaceWithStars)
		{
			text2 = StringUtils.profanityRegex.Replace(text, (Match match) => new string('*', match.Length));
		}
		else
		{
			text2 = StringUtils.profanityRegex.Replace(text, string.Empty);
		}
		if (!replaceWithStars)
		{
			text2 = Regex.Replace(text2, "\\s+", " ").Trim();
		}
		return text2;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x000434F4 File Offset: 0x000416F4
	public static string FilterStringRichText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		string[] tagsToRemove = new string[]
		{
			"align",
			"allcaps",
			"alpha",
			"b",
			"br",
			"color",
			"cspace",
			"font",
			"font-weight",
			"gradient",
			"i",
			"indent",
			"line-height",
			"line-indent",
			"link",
			"lowercase",
			"margin",
			"mark",
			"mspace",
			"nobr",
			"noparse",
			"page",
			"pos",
			"rotate",
			"s",
			"size",
			"smallcaps",
			"space",
			"sprite",
			"strikethrough",
			"style",
			"sub",
			"sup",
			"u",
			"uppercase",
			"voffset",
			"width"
		};
		string pattern = "</?(\\w+(?:-\\w+)?)(?:\\s+[^>]*)?>";
		return Regex.Replace(text, pattern, delegate(Match match)
		{
			string value = match.Groups[1].Value.ToLower();
			if (tagsToRemove.Contains(value))
			{
				return string.Empty;
			}
			return match.Value;
		}, RegexOptions.IgnoreCase);
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x0004367C File Offset: 0x0004187C
	public static string WrapInTeamColor(string username, PlayerTeam team)
	{
		string text;
		if (team != PlayerTeam.Blue)
		{
			if (team != PlayerTeam.Red)
			{
				text = "#404040";
			}
			else
			{
				text = "#d13333";
			}
		}
		else
		{
			text = "#3b82f6";
		}
		string text2 = text;
		if (text2 == null)
		{
			return username;
		}
		return string.Concat(new string[]
		{
			"<color=",
			text2,
			">",
			username,
			"</color>"
		});
	}

	// Token: 0x040008BE RID: 2238
	private static Regex profanityRegex;
}
