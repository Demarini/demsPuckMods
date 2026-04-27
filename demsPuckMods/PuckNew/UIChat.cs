using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UIElements;

// Token: 0x02000186 RID: 390
public class UIChat : UIView
{
	// Token: 0x17000114 RID: 276
	// (get) Token: 0x06000B12 RID: 2834 RVA: 0x00034D36 File Offset: 0x00032F36
	// (set) Token: 0x06000B13 RID: 2835 RVA: 0x00034D3E File Offset: 0x00032F3E
	public bool IsTeamChat { get; private set; }

	// Token: 0x06000B14 RID: 2836 RVA: 0x00034D48 File Offset: 0x00032F48
	public void Initialize(VisualElement rootVisualElement)
	{
		base.View = rootVisualElement.Query("ChatView", null);
		this.chat = base.View.Query("Chat", null);
		this.quickChat = base.View.Query("QuickChat", null);
		this.scrollView = this.chat.Query(null, null);
		this.messages = this.scrollView.Query("Messages", null);
		this.padding = this.scrollView.Query("Padding", null);
		this.textField = this.chat.Query(null, null);
		this.quickChatCategoryLabel = this.quickChat.Query(null, null);
		this.quickChatMessages = this.quickChat.Query("Messages", null);
		this.textField.RegisterCallback<NavigationSubmitEvent>(delegate(NavigationSubmitEvent e)
		{
			this.SubmitMessage();
		}, TrickleDown.TrickleDown);
		this.textField.RegisterCallback<NavigationCancelEvent>(delegate(NavigationCancelEvent e)
		{
			this.StopInput();
		}, TrickleDown.TrickleDown);
		this.chat.RegisterCallback<FocusOutEvent>(delegate(FocusOutEvent e)
		{
			if (UIUtils.GetVisualElementChildren(this.chat, true).Contains(e.relatedTarget))
			{
				return;
			}
			this.StopInput();
		}, TrickleDown.TrickleDown);
		this.messages.RegisterCallback<ChildAddedEvent>(delegate(ChildAddedEvent childAddedEvent)
		{
			UIChat.<>c__DisplayClass18_0 CS$<>8__locals1 = new UIChat.<>c__DisplayClass18_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.child = childAddedEvent.child;
			CS$<>8__locals1.child.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(CS$<>8__locals1.<Initialize>g__OnChildGeometryChanged|8), TrickleDown.NoTrickleDown);
		}, TrickleDown.NoTrickleDown);
		this.messages.RegisterCallback<BeforeChildRemovedEvent>(delegate(BeforeChildRemovedEvent e)
		{
			VisualElement child = e.child;
			this.padding.style.height = new StyleLength(this.padding.resolvedStyle.height + child.resolvedStyle.height);
		}, TrickleDown.NoTrickleDown);
		this.messages.RegisterCallback<HierarchyChangedEvent>(delegate(HierarchyChangedEvent e)
		{
			if (this.messages.childCount == 0)
			{
				this.padding.style.height = StyleKeyword.Initial;
			}
		}, TrickleDown.NoTrickleDown);
		this.scrollView.verticalScroller.valueChanged += delegate(float value)
		{
			this.LimitScrollToPaddingHeight();
			if (!this.isScrolling)
			{
				int num = Mathf.RoundToInt(this.scrollView.verticalScroller.highValue - this.scrollView.verticalScroller.value);
				this.autoScroll = (num <= 0);
			}
		};
		UIUtils.GetVisualElementChildren(this.chat, true).ForEach(delegate(VisualElement visualElement)
		{
			visualElement.focusable = true;
		});
		this.ClearChatMessages();
		this.StopInput();
		this.HideQuickChat();
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0003233E File Offset: 0x0003053E
	public override bool Show()
	{
		return SettingsManager.ShowGameUserInterface && base.Show();
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00034F2E File Offset: 0x0003312E
	public void StartInput(bool isTeamChat = false)
	{
		base.IsFocused = true;
		this.IsTeamChat = isTeamChat;
		this.ShowTextField();
		this.uiChatMessages.ForEach(delegate(UIChatMessage uiChatMessage)
		{
			uiChatMessage.Focus();
		});
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x00034F6E File Offset: 0x0003316E
	public void StopInput()
	{
		base.IsFocused = false;
		this.IsTeamChat = false;
		this.HideTextField();
		this.uiChatMessages.ForEach(delegate(UIChatMessage uiChatMessage)
		{
			uiChatMessage.Blur();
		});
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x00034FB0 File Offset: 0x000331B0
	private void ShowTextField()
	{
		this.textField.style.opacity = 1f;
		this.textField.pickingMode = PickingMode.Position;
		this.textField.value = string.Empty;
		this.textField.Focus();
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x00035000 File Offset: 0x00033200
	private void HideTextField()
	{
		this.textField.style.opacity = 0f;
		this.textField.pickingMode = PickingMode.Ignore;
		this.textField.value = string.Empty;
		this.textField.Blur();
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00035050 File Offset: 0x00033250
	public void ShowQuickChat(QuickChatCategory category, QuickChat[] quickChats)
	{
		this.quickChatCategoryLabel.text = category.ToString().ToUpper();
		for (int i = 0; i < quickChats.Length; i++)
		{
			VisualElement visualElement = this.quickChatMessageAsset.Instantiate();
			TextElement textElement = visualElement.Query(null, null);
			string content = quickChats[i].Content;
			textElement.text = string.Format("{0}. {1}", i + 1, content);
			this.quickChatMessages.Add(visualElement);
		}
		this.quickChat.style.display = DisplayStyle.Flex;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x000350E8 File Offset: 0x000332E8
	public void HideQuickChat()
	{
		this.quickChat.style.display = DisplayStyle.None;
		this.quickChatMessages.Clear();
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0003510C File Offset: 0x0003330C
	private void SmoothScrollToVerticalPosition(float position, bool isBottomPosition = false)
	{
		Vector2 vector = new Vector2(0f, position - (isBottomPosition ? this.scrollView.contentViewport.resolvedStyle.height : 0f));
		vector = Utils.Vector2Clamp(vector, Vector2.zero, Vector2.positiveInfinity);
		Tween tween = this.smoothScrollTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.smoothScrollTween = DOTween.To(() => this.scrollView.scrollOffset, delegate(Vector2 x)
		{
			this.scrollView.scrollOffset = x;
		}, vector, 0.2f).OnStart(delegate
		{
			this.isScrolling = true;
		}).OnComplete(delegate
		{
			this.isScrolling = false;
		}).SetEase(Ease.Linear);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000351C0 File Offset: 0x000333C0
	private void LimitScrollToPaddingHeight()
	{
		if (this.scrollView.scrollOffset.y < this.padding.resolvedStyle.height)
		{
			this.scrollView.scrollOffset = new Vector2(this.scrollView.scrollOffset.x, this.padding.resolvedStyle.height);
		}
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x00035220 File Offset: 0x00033420
	private void SubmitMessage()
	{
		EventManager.TriggerEvent("Event_OnChatSubmitMessage", new Dictionary<string, object>
		{
			{
				"content",
				this.textField.value
			},
			{
				"isTeamChat",
				this.IsTeamChat
			}
		});
		this.StopInput();
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x00035270 File Offset: 0x00033470
	public void AddChatMessage(ChatMessage chatMessage, Units units, bool filterProfanity)
	{
		VisualElement visualElement = this.messageAsset.Instantiate();
		visualElement.focusable = true;
		Label label = visualElement.Query(null, null);
		label.focusable = true;
		string chatMessagePrefix = this.GetChatMessagePrefix(chatMessage);
		string text = this.ParseChatContent(chatMessage.Content.ToString(), chatMessage.IsSystem, units, filterProfanity);
		label.text = chatMessagePrefix + text;
		label.style.display = ((text.Length > 0) ? DisplayStyle.Flex : DisplayStyle.None);
		UIChatMessage uichatMessage = new UIChatMessage(chatMessage, visualElement, 5f);
		this.messages.Add(uichatMessage.VisualElement);
		this.uiChatMessages.Add(uichatMessage);
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x00035320 File Offset: 0x00033520
	public void RemoveChatMessage(ChatMessage chatMessage)
	{
		UIChatMessage uichatMessage = this.uiChatMessages.FirstOrDefault((UIChatMessage m) => m.ChatMessage == chatMessage);
		if (uichatMessage == null)
		{
			return;
		}
		uichatMessage.Dispose();
		this.messages.Remove(uichatMessage.VisualElement);
		this.uiChatMessages.Remove(uichatMessage);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0003537C File Offset: 0x0003357C
	public void ClearChatMessages()
	{
		this.uiChatMessages.ForEach(delegate(UIChatMessage uiChatMessage)
		{
			uiChatMessage.Dispose();
		});
		this.messages.Clear();
		this.uiChatMessages.Clear();
		Tween tween = this.smoothScrollTween;
		if (tween != null)
		{
			tween.Kill(false);
		}
		this.autoScroll = true;
		this.isScrolling = false;
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x000353E9 File Offset: 0x000335E9
	public void SetOpacity(float opacity)
	{
		this.chat.style.opacity = new StyleFloat(opacity);
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00035401 File Offset: 0x00033601
	public void SetScale(float scale)
	{
		this.chat.style.scale = new StyleScale(new Scale(new Vector2(scale, scale)));
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x00035424 File Offset: 0x00033624
	private string GetChatMessagePrefix(ChatMessage chatMessage)
	{
		string text = string.Empty;
		if (chatMessage.IsTeamChat)
		{
			text += "[TEAM] ";
		}
		if (!chatMessage.IsSystem)
		{
			string str = StringUtils.WrapInTeamColor(chatMessage.Username.ToString(), chatMessage.Team.Value);
			text = text + str + ": ";
		}
		return text;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00035484 File Offset: 0x00033684
	private string ParseChatContent(string content, bool isSystem, Units units, bool filterProfanity)
	{
		if (isSystem)
		{
			content = Regex.Replace(content, "<united>([^<]+)</united>", delegate(Match match)
			{
				string value = match.Groups[1].Value;
				float value2;
				if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out value2))
				{
					return ((units == Units.Metric) ? Utils.GameUnitsToMetric(value2) : Utils.GameUnitsToImperial(value2)).ToString("F1", CultureInfo.InvariantCulture);
				}
				return value;
			});
			content = Regex.Replace(content, "&units", (units == Units.Metric) ? "KPH" : "MPH");
		}
		else
		{
			content = StringUtils.FilterStringRichText(content);
			content = StringUtils.FilterStringSpecialCharacters(content, Constants.CHAT_WHITELIST, filterProfanity ? Constants.CHAT_BLACKLIST : null);
			if (filterProfanity)
			{
				content = StringUtils.FilterStringProfanity(content, true);
			}
		}
		return content;
	}

	// Token: 0x04000697 RID: 1687
	[Header("References")]
	[SerializeField]
	private VisualTreeAsset messageAsset;

	// Token: 0x04000698 RID: 1688
	[SerializeField]
	private VisualTreeAsset quickChatMessageAsset;

	// Token: 0x0400069A RID: 1690
	private VisualElement chat;

	// Token: 0x0400069B RID: 1691
	private VisualElement quickChat;

	// Token: 0x0400069C RID: 1692
	private ScrollView scrollView;

	// Token: 0x0400069D RID: 1693
	private VisualElement padding;

	// Token: 0x0400069E RID: 1694
	private VisualElement messages;

	// Token: 0x0400069F RID: 1695
	private TextField textField;

	// Token: 0x040006A0 RID: 1696
	private Label quickChatCategoryLabel;

	// Token: 0x040006A1 RID: 1697
	private VisualElement quickChatMessages;

	// Token: 0x040006A2 RID: 1698
	private List<UIChatMessage> uiChatMessages = new List<UIChatMessage>();

	// Token: 0x040006A3 RID: 1699
	private bool autoScroll = true;

	// Token: 0x040006A4 RID: 1700
	private bool isScrolling;

	// Token: 0x040006A5 RID: 1701
	private Tween smoothScrollTween;
}
