using System.Text.Json;
using AutoMapper;
using Markdig;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Silo.Identity.Client;

namespace Silo.Modules.Ai.Pages.Agent;

public partial class ChatBot : SiloBasePage
{
    private List<CopilotMessageRequest> chatMessages = new();
    private string currentMessage = string.Empty;
    private bool IsLoading = true;
    private bool IsSending = false;
    private bool IsInitialLoading = true;
    private string UserId = string.Empty;
    private ElementReference chatMessagesElement;
    private readonly Markdig.MarkdownPipeline markdownPipeline;
    private bool ShowModeSelectionDialog = false;
    private Application.Features.RagDocType selectedMode = Application.Features.RagDocType.Agent;
    private bool isSidebarExpanded = false;
    private List<ChatHistory> chatHistories = new();
    private int currentChatId = 0;
    private List<string> currentPromptKeys = new();

    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public IMapper Mapper { get; set; }
    [Inject] public ILogger<ChatBot> Logger { get; set; }
    [Inject] public IExcelExport ExcelExporter { get; set; }

    [Parameter] public Application.Features.RagDocType? Mode { get; set; }

    [CascadingParameter] public DialogFactory Dialog { get; set; }


    public ChatBot()
    {
        markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    protected override async Task SiloInitializer()
    {
        if (Mode is null)
        {
            ShowModeSelectionDialog = true;

            IsLoading = false;

            IsInitialLoading = false;

            StateHasChanged();

            return;
        }

        await InitializeChatWithMode(Mode.Value);
    }

    private async Task<bool> LoadUserChatHistoriesFromDb()
    {
        chatHistories = new List<ChatHistory>();

        UserId = (await AuthState.GetAuthenticationStateAsync()).User.GetUserId();

        var response = await Api.SendAsyncObjectByUri<GetChatHistoriesVm>(
            HttpMethod.Get,
            $"ChatSessions/GetChatHistories?userId={UserId}&mode={Mode}"
        );

        if (response.Value?.Histories is null || response.Value.Histories.Count == 0)
        {
            return false;
        }

        chatHistories = response.Value.Histories;

        return true;
    }


    private async Task InitializeChatWithMode(Application.Features.RagDocType mode)
    {
        Mode = mode;

        ShowModeSelectionDialog = false;

        List<string> keys = mode switch
        {
            Application.Features.RagDocType.Report => new List<string> { "report", "report-query" },
            //ChatPageMode.Help => new List<string> { "help", "support", "troubleshooting" },
            Application.Features.RagDocType.Agent => new()
            {
                "agent-general","add-product","report-builder","exit-report-builder","product-report-builder","truckcross","location",
                "inventory-conflict","reports-truckcross"
            },
            _ => new()
        };

        currentPromptKeys = keys;

        await LoadUserChatHistoriesFromDb();

        currentChatId = 0;

        chatMessages = new List<CopilotMessageRequest>
        {
            new()
            {
                Text = "به دستیار هوشمند سیلو خوش آمدید. چطور میتوانم کمکتان کنم؟",
                IsUser = false,
                Datetime = DateTime.Now
            }
        };

        chatHistories.Insert(0, new ChatHistory
        {
            Id = 0,
            Title = "گفتگوی جدید",
            Messages = new List<CopilotMessageRequest>(chatMessages),
            CreatedDate = DateTime.Now,
            LastUpdated = DateTime.Now
        });

        IsLoading = false;

        IsInitialLoading = false;

        StateHasChanged();
    }

    private async Task OnModeSelected()
    {
        await InitializeChatWithMode(selectedMode);
    }

    private string GetModeFriendlyName(Application.Features.RagDocType mode)
    {
        return mode switch
        {
            Application.Features.RagDocType.Report => "گزارش‌گیری",
            Application.Features.RagDocType.Agent => "دستیار هوشمند",
            _ => mode.ToString()
        };
    }

    private string GetModeIcon(Application.Features.RagDocType mode)
    {
        return mode switch
        {
            Application.Features.RagDocType.Report => MaterialIconsHelper.InsertChart2,
            //ChatPageMode.Help => MaterialIconsHelper.Help,
            Application.Features.RagDocType.Agent => MaterialIconsHelper.SmartToy,
            _ => MaterialIconsHelper.Info
        };
    }

    private void ToggleSidebar()
    {
        isSidebarExpanded = !isSidebarExpanded;
        StateHasChanged();
    }

    private void LoadChatHistory(ChatHistory history)
    {
        currentChatId = history.Id;

        chatMessages = (history.Messages);

        StateHasChanged();
    }

    private async Task StartNewChat()
    {
        if (chatMessages.Count > 1)
        {
            await LoadUserChatHistoriesFromDb();
        }

        currentChatId = 0;

        var result = await Api.SendAsyncObjectByUri<NewChatSessionVm>(
            HttpMethod.Post,
            "ChatSessions/NewSession",
            new NewChatSessionCommand
            {
                UserId = UserId,
                Mode = Mode.Value,
                PromptKeys = currentPromptKeys
            }
        );

        if (result?.Value?.SessionId > 0)
        {
            currentChatId = result.Value.SessionId;
        }

        chatMessages = new List<CopilotMessageRequest>
        {
            new()
            {
                Text = "به دستیار هوشمند سیلو خوش آمدید. چطور میتوانم کمکتان کنم؟",
                IsUser = false,
                Datetime = DateTime.Now
            }
        };

        chatHistories.Insert(0, new ChatHistory
        {
            Id = 0,
            Title = "گفتگوی جدید",
            Messages = new List<CopilotMessageRequest>(chatMessages),
            CreatedDate = DateTime.Now,
            LastUpdated = DateTime.Now
        });

        StateHasChanged();
    }


    private void SaveCurrentChatToHistory()
    {
        if (chatMessages.Count <= 1)
        {
            return;
        }

        var firstUserMessage = chatMessages.FirstOrDefault(m => m.IsUser);

        if (firstUserMessage is null)
        {
            return;
        }

        if (currentChatId != 0)
        {
            var existingHistory = chatHistories.FirstOrDefault(h => h.Id == currentChatId);

            if (existingHistory is not null)
            {
                existingHistory.Messages =(chatMessages);

                existingHistory.LastUpdated = DateTime.Now;
            }
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await OnSendClick();
        }
    }

    private void UpdateTemporaryChatTitle()
    {
        if (currentChatId == 0 && chatHistories.Any() && chatHistories.First().Id == 0)
        {
            var tempHistory = chatHistories.First();

            var firstUserMessage = chatMessages.FirstOrDefault(m => m.IsUser);

            if (firstUserMessage is not null)
            {
                var title = firstUserMessage.Text.Length > 50
                    ? firstUserMessage.Text.Substring(0, 50) + "..."
                    : firstUserMessage.Text;

                tempHistory.Title = title;

                tempHistory.LastUpdated = DateTime.Now;

                StateHasChanged();
            }
        }
    }

    private async Task OnSendClick()
    {
        if (currentMessage.HasNoValue() || IsLoading || IsSending)
        {
            return;
        }

        IsSending = true;

        var userMessage = currentMessage.Trim();

        currentMessage = string.Empty;

        chatMessages.Add(new()
        {
            Text = userMessage,
            IsUser = true,
            Datetime = DateTime.Now
        });

        UpdateTemporaryChatTitle();

        IsLoading = true;

        StateHasChanged();

        await ScrollToBottom();

        try
        {
            var sendResult = await Api.SendAsyncObjectByUri<SendChatMessageVm>(
                HttpMethod.Post,
                "ChatSessions/SendMessage",
                new SendChatMessageCommand
                {
                    UserId = UserId,
                    SessionId = currentChatId,
                    Message = userMessage,
                    Mode = Mode.Value,
                    PromptKeys = currentPromptKeys
                }
            );

            CopilotMessageRequest response = new()
            {
                Text = sendResult?.Value?.ResponseText ?? string.Empty,
                IsUser = false,
                Datetime = DateTime.Now
            };

            if (currentChatId == 0 && sendResult?.Value?.SessionId > 0)
            {
                currentChatId = sendResult.Value.SessionId;
            }

            if (sendResult?.Value?.SqlCommandsResults is { Count: > 0 })
            {
                response.SqlCommandsResults = sendResult.Value.SqlCommandsResults;
            }

            chatMessages.Add(response);

            SaveCurrentChatToHistory();

            if (chatHistories.Any(h => h.Id == 0))
            {
                chatHistories.RemoveAll(h => h.Id == 0);

                await LoadUserChatHistoriesFromDb();

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing chat message for user {UserId}, sessionId {SessionId}", UserId, currentChatId);

            chatMessages.Add(new()
            {
                Text = "متأسفانه در پردازش درخواست شما خطایی رخ داده است. لطفاً دوباره تلاش کنید.",
                IsUser = false,
                Datetime = DateTime.Now
            });
        }

        finally
        {
            IsSending = false;

            IsLoading = false;

            StateHasChanged();

            await ScrollToBottom();
        }
    }

    private async Task ScrollToBottom()
    {
        try
        {
            await Task.Delay(100);

            await JSRuntime.InvokeVoidAsync("scrollToBottom", chatMessagesElement);
        }
        catch
        {
        }
    }

    private string ConvertMarkdownToHtml(string markdown)
    {
        if (markdown.HasNoValue())
        {
            return string.Empty;
        }

        return Markdown.ToHtml(markdown, markdownPipeline);
    }

    private async Task CopyMessageToClipboard(string text)
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("copyToClipboard", text);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to copy message to clipboard");
        }
    }

    private async Task DeleteHistory(ChatHistory history)
    {
        var resultDialog = await Dialog.ConfirmAsync(
            TextResources.APP_StringKeys_Message_Delete,
            TextResources.APP_StringKeys_Attention,
            okButtonText: TextResources.APP_StringKeys_Approve,
            cancelButtonText: TextResources.APP_StringKeys_Return
        );

        if (!resultDialog)
        {
            return;
        }

        DeleteChatSessionsCommand cmd = new()
        {
            SessionId = history.Id,
            UserId = UserId
        };

        var res = (await Api.SendAsyncObjectByUri<DeleteChatSessionsVm>(
            HttpMethod.Delete,
            "ChatSessions/Delete",
            cmd
        ))?.Value?.Result;

        chatHistories.RemoveAll(h => h.Id == history.Id);

        currentChatId = 0;

        chatMessages = new()
        {
            new()
            {
                Text = "به دستیار هوشمند سیلو خوش آمدید. چطور میتوانم کمکتان کنم؟",
                IsUser = false,
                Datetime = DateTime.Now
            }
        };

        StateHasChanged();
    }
}
