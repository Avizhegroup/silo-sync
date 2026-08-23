using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Silo.Application.Features;
using System.Text.RegularExpressions;

namespace Silo.Pages.Notif;
public partial class ManageNotif
{
    public bool IsLoading = true;
    public bool IsShownUsers = false;
    public bool IsElementForContact = true;
    public string Contact = string.Empty;
    public string User = string.Empty;
    public string UserId = string.Empty;
    public string Username = string.Empty;
    public string SearchElementText = string.Empty;
    public string ElementsModalTitle = string.Empty;
    public SaveNotificationOrderCommand Order = new();
    public List<GetAllNotificationOrderVm> Orders;
    public List<GetAllNotificationEventTypeVm> EventTypes;
    public List<ChoosableKeyValue> SendTypes = new()
    {
        new()
        {
            IsChoosed = true,
            Key = "EMAIL",
            Value = "0"
        },
        new()
        {
            IsChoosed = false,
            Key = "SMS",
            Value = "1"
        },
        new()
        {
            IsChoosed = false,
            Key = "PUSH",
            Value = "2"
        },
        new()
        {
            IsChoosed = false,
            Key = "WHATSAPP",
            Value = "3"
        },
        new()
        {
            IsChoosed = false,
            Key = "TELEGRAM",
            Value = "4"
        }
    };
    public List<SelectListItem> SendPeriod = new()
    {
        new()
        {
            Text = "روزانه",
            Value = "0"
        },
        new()
        {
            Text = "هفتگی",
            Value = "1"
        },
        new()
        {
            Text = "ماهیانه",
            Value = "2"
        }
    };
    public List<SelectListItem> SendDay = new()
    {
        new()
        {
            Text = "هر روز",
            Value = "0"
        },
        new()
        {
            Text = "شنبه",
            Value = "1"
        },
        new()
        {
            Text = "پنج شنبه",
            Value = "2"
        },
        new()
        {
            Text = "جمعه",
            Value = "3"
        },
        new()
        {
            Text = "روز آخر ماه",
            Value = "4"
        },
        new()
        {
            Text = "روز اول ماه",
            Value = "5"
        }
    };
    public List<SelectListItem> NotifType = new()
    {
        new()
        {
            Text = TextResources.APP_StringKeys_View_Notif_EventBase,
            Value = "0"
        },
        new()
        {
            Text = TextResources.APP_StringKeys_View_Notif_ScheduleBase,
            Value = "1"
        }
    };
    public List<GetAllDataMiningElementVm> Elements = new();
    public List<SelectListItem> SearchElements = new();
    public List<string> Contacts = new();
    public List<SelectListItem> Users = new();

    public Modal ModalElements { get; set; }
    public Modal ModalDetails { get; set; }
    public Modal ModalDelete { get; set; }
    public EditForm EditForm { get; set; }
    public ElementReference ContentTextArea { get; set; }

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        var user = (await AuthStateProvider.GetAuthenticationStateAsync()).User;

        UserId = user.GetUserId();

        Username = user.GetUsername();

        Elements = (await Api.PostAsync<List<GetAllDataMiningElementVm>>("SGetDataMiningElements"
                                        , new("DataMiningElementsId", "-1")
                                        , new("DataMiningElementsTitle", "-1"))).Value;

        EventTypes = (await Api.PostAsync<List<GetAllNotificationEventTypeVm>>("SGetAllEventTypes")).Value;

        Users = (await Api.PostAsync<List<ApplicationUser>>("GetAllUser",
            new KeyValuePair<string, object>[] { new("userToken", "Ceramic client user") }))
            .Value
            .Where(p => p.IsActive)
            .Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id
            }).ToList();

        await RefreshOrders();

        Order = new()
        {
            Type = NotifType.First().Value,
            TimePeriod = SendPeriod.First().Value,
            SendDay = SendDay.First().Value,
            SendClock = DateTime.Now.ToString("HH:mm"),
            SendType = "1"
        };

        IsLoading = false;
    }

    #region Events
    public async Task OnValidSubmit(EditContext editContext)
    {
        var emptiness = CheckEmptiness();

        if (emptiness.HasValue())
        {
            Notification.Show(string.Format(TextResources.APP_StringKeys_Validation_Required, emptiness), "error");

            return;
        }

        IsLoading = true;

        if (Order.Id == 0) Order.Id = -1;

        Order.UserId = UserId;

        Order.Username = Username;

        Order.SendContacts = string.Join(',', Contacts);

        if (Order.Type.Equals("0"))
        {
            Order.SendDay = string.Empty;

            Order.TimePeriod = "-1";

            Order.SendClock = string.Empty;
        }

        int result = (await Api.PostAsync<int>("SInsertUpdateNotification"
            , new KeyValuePair<string, object>("notif", Order))).Value;

        if (result != -1)
        {
            if (Order.Id == -1)
            {
                Order.Id = result;
            }

            await RefreshOrders();

            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        IsLoading = false;
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        Order = new();

        Contacts.Clear();

        Contact = string.Empty;

        User = string.Empty;
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Order.Id == 0 || Order.Id == -1)
        {
            Notification.Show(TextResources.APP_StringKeys_Validation_Choose, "warning");

            return;
        }

        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SRemoveNotification",
            new KeyValuePair<string, object>("id", Order.Id))).Value;

        if (result)
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Success, "success");
        }
        else
        {
            Notification.Show(TextResources.APP_StringKeys_Alert_Fail, "error");
        }

        await OnClearClick(e);

        await RefreshOrders();

        IsLoading = false;
    }
    #endregion

    #region ModalElement
    public async Task OnModalElementClick(MouseEventArgs e, bool isElementForContact)
    {
        IsElementForContact = isElementForContact;

        if (IsElementForContact)
        {
            ElementsModalTitle = TextResources.APP_StringKeys_Notif_Elements_Contacts;
        }
        else
        {
            ElementsModalTitle = TextResources.APP_StringKeys_Notif_ReportElements;
        }

        SearchElements = Elements.Where(p => p.DataMiningElementsType.Equals(Order.Type)).Select(p => new SelectListItem()
        {
            Text = p.DataMiningElementsTitle,
            Value = p.DataMiningElementsId.ToString()
        }).ToList();

        SearchElementText = string.Empty;

        await ModalElements.Open(e);
    }

    public async Task OnSelectElementClick(string element)
    {
        if (IsElementForContact)
        {
            Contacts.Add($"[{element}]");
        }
        else
        {
            Order.Content += $" [{element}]";

            await ContentTextArea.FocusAsync();
        }

        StateHasChanged();
    }

    public void OnElementRefreshClick()
    {
        SearchElements = Elements.Where(p => p.DataMiningElementsType.Equals(Order.Type)).Select(p => new SelectListItem()
        {
            Text = p.DataMiningElementsTitle,
            Value = p.DataMiningElementsId.ToString()
        }).ToList();

        SearchElementText = string.Empty;
    }

    public void OnElementSearchClick()
    {
        SearchElements = Elements.Where(p => p.DataMiningElementsType.Equals(Order.Type) && p.DataMiningElementsTitle.Contains(SearchElementText)).Select(p => new SelectListItem()
        {
            Text = p.DataMiningElementsTitle,
            Value = p.DataMiningElementsId.ToString()
        }).ToList();
    }
    #endregion

    #region Contact
    public async Task OnRemoveContact(string contact)
    {
        Contacts.Remove(contact);
    }

    public async Task OnAddContactClick(MouseEventArgs e)
    {
        string formatInvalid = "لطفا {0} را در فرمت درست وارد کنید\n {1}";

        if (Order.SendType == "0")
        {
            if (!Tools.StringTools.IsValidEmail(Contact))
            {
                Notification.Show(string.Format(formatInvalid, "ایمیل", "email@example.com"), "error");

                return;
            }
        }

        if (Order.SendType == "1")
        {
            if (!Regex.IsMatch(Contact,
                "^[0-9]{11}$"))
            {
                Notification.Show(string.Format(formatInvalid, "تلفن", "09XXXXXXXXX" + " یا " + "021XXXXXXXX"), "error");

                return;
            }
        }

        if (Order.SendType == "0" ||
            Order.SendType == "1")
        {
            if (Contact.HasNoValue())
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Contact_Required, "error");

                return;
            }

            if (Contacts.Contains(Contact))
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Contact_Duplicate, "error");

                return;
            }

            Contacts.Add(Contact);

            Contact = string.Empty;
        }

        if (Order.SendType == "2")
        {
            if (User.HasNoValue())
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Contact_Required, "error");

                return;
            }

            if (Contacts.Contains(User))
            {
                Notification.Show(TextResources.APP_StringKeys_Validation_Contact_Duplicate, "error");

                return;
            }

            Contacts.Add(User);

            User = string.Empty;
        }
    }
    #endregion

    #region OrdersModal
    public async Task OnChooseOrder(GetAllNotificationOrderVm order)
    {
        Order = Mapper.Map<SaveNotificationOrderCommand>(order);

        Contacts.Clear();

        if (Order.SendContacts.Contains(","))
        {
            string[] contacts = Order.SendContacts.Split(',');

            foreach (string contact in contacts)
            {
                Contacts.Add(contact);
            }
        }
        else
        {
            if (Order.SendContacts.HasValue())
            {
                Contacts.Add(Order.SendContacts);
            }
        }

        await ModalDetails.Close(new());
    }

    public async Task RefreshOrders()
    {
        IsLoading = true;

        Orders = (await Api.PostAsync<List<GetAllNotificationOrderVm>>("SGetNotification")).Value;

        IsLoading = false;
    }
    #endregion

    #region Private Methods
    private string CheckEmptiness()
    {
        if (Order.Type.Equals("1"))
        {
            if (Order.TimePeriod.HasNoValue())
            {
                return TextResources.APP_StringKeys_Notif_TimePeriod;
            }

            if (Order.SendDay.HasNoValue())
            {
                return TextResources.APP_StringKeys_Notif_SendDay;
            }

            if (Order.SendClock.HasNoValue())
            {
                return TextResources.APP_StringKeys_Notif_SendClock;
            }
        }
        else
        {
            if (Order.EventType.Equals(0))
            {
                return TextResources.APP_StringKeys_Event_Type;
            }
        }

        if (!Contacts.Any())
        {
            return TextResources.APP_StringKeys_Notif_Contacts;
        }

        return string.Empty;
    }
    #endregion
}
