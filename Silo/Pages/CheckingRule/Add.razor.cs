using Silo.Application.Features;

namespace Silo.Pages.CheckingRule;

public partial class Add
{
    public bool IsLoading = true;
    public string MessageText = string.Empty;
    public string CurrentUserId = string.Empty;
    public List<GetAllDataMiningElementVm> Elements = new();
    public GetAllRuleVm Request = new();
    public List<GetAllRuleVm> Rules;
    public List<TelerikDropDownItem> CheckingRuleTypes = new()
    {
        new() { Name = "ثبت بازرسی", Value = "1" },
        new() { Name = "ورود به انبار", Value = "2" },
        new() { Name = "خروج از انبار", Value = "3" },
        new() { Name = "صدور بارنامه", Value = "4" }
    };
    public List<TelerikDropDownItem> CheckingRuleResultTypes = new()
    {
        new() { Name = "هشدار", Value = "1" },
        new() { Name = "توقف", Value = "2" }
    };

    [Inject] public RfidConnectApi Api { get; set; }
    [Inject] public SiloAuthenticationStateProvider SiloAuth { get; set; }

    public Modal ModalDetails { get; set; }
    public Modal ModalMessage { get; set; }
    public Modal ModalDelete { get; set; }
    public Modal ModalElements { get; set; }

    protected override async Task SiloInitializer()
    {
        SiloContext.NavbarFilterClicked = null;

        Elements = (await Api.PostAsync<List<GetAllDataMiningElementVm>>("SGetDataMiningElements"
                , new("DataMiningElementsId", "-1")
                , new("DataMiningElementsTitle", "-1"))).Value;

        Rules = (await Api.PostAsync<List<GetAllRuleVm>>("SGetcheckingrules",
                        new("CheckingRulesId", "-1"),
                        new("CheckingRulesTitle", "-1"),
                        new("CheckingRulesType", "-1"),
                        new("CheckingRulesStationCode", "-1"))).Value;

        CurrentUserId = (await SiloAuth.GetAuthenticationStateAsync()).User.GetUserId();

        IsLoading = false;
    }

    public async Task OnSubmitClick(MouseEventArgs e)
    {
        if (CheckEmptiness())
        {
            MessageText = TextResources.APP_StringKeys_Validation_EmptinessCheck;

            await ModalMessage.Open(e);

            return;
        }

        IsLoading = true;

        GetAllRuleVm rule = FixEmptiness();

        rule.Title = Request.Title;

        rule.Command = Request.Command;

        rule.ReturnResultTrue = Request.ReturnResultTrue;

        rule.ReturnResultFalse = Request.ReturnResultFalse;

        int result = (await Api.PostAsync<int>("SSavecheckingrules",
                    new KeyValuePair<string, object>("rule", rule))).Value;

        if (result != -1)
        {
            Request.Id = result.ToString();

            MessageText = TextResources.APP_StringKeys_Alert_Success;
        }
        else
        {
            MessageText = TextResources.APP_StringKeys_Alert_Fail;
        }

        Rules = (await Api.PostAsync<List<GetAllRuleVm>>("SGetcheckingrules",
                        new("CheckingRulesId", "-1"),
                        new("CheckingRulesTitle", "-1"),
                        new("CheckingRulesType", "-1"),
                        new("CheckingRulesStationCode", "-1"))).Value;

        await ModalMessage.Open(e);

        IsLoading = false;
    }

    public async Task OnOperationClick(CheckingRuleOperator op)
    {
        switch (op)
        {
            case CheckingRuleOperator.OpenParentheses:
                Request.Command += $" (";
                break;
            case CheckingRuleOperator.CloseParentheses:
                Request.Command += $" )";
                break;
            case CheckingRuleOperator.Equals:
                Request.Command += $" ==";
                break;
            case CheckingRuleOperator.BiggerThan:
                Request.Command += $" >";
                break;
            case CheckingRuleOperator.BiggerEqualsThan:
                Request.Command += $" >=";
                break;
            case CheckingRuleOperator.SmallerThan:
                Request.Command += $" <";
                break;
            case CheckingRuleOperator.SmallerEqualsThan:
                Request.Command += $" <=";
                break;
            case CheckingRuleOperator.And:
                Request.Command += $" &&";
                break;
            case CheckingRuleOperator.Or:
                Request.Command += $" ||";
                break;
            case CheckingRuleOperator.Delete:
                if (Request.Command.Any())
                {
                    Request.Command = Request.Command
                                             .Remove(Request.Command.Length - 1, 1);
                }
                break;
            default:
                break;
        }
    }

    public async Task OnChooseElement(string element)
    {
        Request.Command += $" [{element}]";
    }

    public async Task OnChooseRule(GetAllRuleVm rule)
    {
        Request = rule;

        await ModalDetails.Close(new());
    }

    public async Task OnRemoveClick(MouseEventArgs e)
    {
        if (Request.Id == "0")
        {
            MessageText = TextResources.APP_StringKeys_Validation_Choose;

            await ModalMessage.Open(e);

            return;
        }

        IsLoading = true;

        bool result = (await Api.PostAsync<bool>("SRemoveCheckingRule",
                     new KeyValuePair<string, object>("id", Request.Id))).Value;

        if (result)
        {
            MessageText = TextResources.APP_StringKeys_Alert_Success;
        }
        else
        {
            MessageText = TextResources.APP_StringKeys_Alert_Fail;
        }

        await OnClearClick(e);

        Rules = (await Api.PostAsync<List<GetAllRuleVm>>("SGetcheckingrules",
                         new("CheckingRulesId", "-1"),
                         new("CheckingRulesTitle", "-1"),
                         new("CheckingRulesType", "-1"),
                         new("CheckingRulesStationCode", "-1"))).Value;

        IsLoading = false;

        await ModalMessage.Open(e);
    }

    public async Task OnClearClick(MouseEventArgs e)
    {
        Request = new();
    }

    private GetAllRuleVm FixEmptiness()
    {
        GetAllRuleVm rule = new();

        if (string.IsNullOrEmpty(Request.StationCode))
            rule.StationCode = "-1";
        else
            rule.StationCode = Request.StationCode;

        if (string.IsNullOrEmpty(Request.Type))
            rule.Type = "-1";
        else
        {
            rule.Type = Request.Type;
        }

        if (string.IsNullOrEmpty(Request.ResultType))
            rule.ResultType = "-1";
        else
        {
            rule.ResultType = Request.ResultType;
        }

        if (string.IsNullOrEmpty(Request.Id))
            rule.Id = "-1";
        else
        {
            rule.Id = Request.Id;
        }

        rule.RegDate = DateTime.Now.ToString();

        rule.Status = "1";

        rule.RegUser = CurrentUserId;

        return rule;
    }

    private bool CheckEmptiness()
    {
        if (Request.Title.HasNoValue())
        {
            return true;
        }

        if (Request.StationCode.HasNoValue())
        {
            return true;
        }

        if (Request.Type.HasNoValue())
        {
            return true;
        }

        if (Request.ResultType.HasNoValue())
        {
            return true;
        }

        if (Request.ReturnResultTrue.HasNoValue())
        {
            return true;
        }

        if (Request.ReturnResultFalse.HasNoValue())
        {
            return true;
        }

        if (Request.Command.HasNoValue())
        {
            return true;
        }

        return false;
    }
}
