using Silo.Api.External.Sharif.Features.ExitPermission;
using Silo.Api.External.Sharif.Features.ItemInformation;
using Silo.Api.External.Sharif.Features.LoanRegistration;
using Silo.Api.External.Sharif.Features.LoanRenewal;
using Silo.Api.External.Sharif.Features.LoanReturn;
using Silo.Api.External.Sharif.Features.MemberIdentification;
using Silo.Api.External.Sharif.Features.RfidRegistration;
using Silo.Api.External.Sharif.Models;

namespace Silo.Api.External.Sharif.Services;

public class SharifExternalConnect
{
    private readonly SharifHttpClientHandler _httpClientHandler;

    public SharifExternalConnect(SharifHttpClientHandler httpClientHandler)
    {
        _httpClientHandler = httpClientHandler;
    }

    public async Task<SharifApiResponse<ItemInformationResponse>> GetItemInformationAsync(
        GetItemInformationQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.LoanableItemId))
        {
            throw new ArgumentException("LoanableItemId cannot be empty", nameof(query));
        }

        var endpoint = $"/v1/kiosk/loanable-items/{query.LoanableItemId}";
        return await _httpClientHandler.GetAsync<ItemInformationResponse>(endpoint, cancellationToken);
    }

    public async Task<SharifApiResponse<RegisterRfidResponse>> RegisterRfidAsync(
        RegisterRfidCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.LoanableItemId))
        {
            throw new ArgumentException("LoanableItemId cannot be empty", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.RfidUid))
        {
            throw new ArgumentException("RfidUid cannot be empty", nameof(command));
        }

        var endpoint = $"/v1/kiosk/loanable-items/{command.LoanableItemId}/rfid-uids/";
        var request = new RegisterRfidRequest
        {
            RfidUid = command.RfidUid
        };

        return await _httpClientHandler.PostAsync<RegisterRfidResponse>(endpoint, request, cancellationToken);
    }

    public async Task<SharifApiResponse<MemberInformationResponse>> GetMemberInformationAsync(
        GetMemberInformationQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.MemberId))
        {
            throw new ArgumentException("MemberId cannot be empty", nameof(query));
        }

        var endpoint = $"/v1/kiosk/members/{query.MemberId}/";
        return await _httpClientHandler.GetAsync<MemberInformationResponse>(endpoint, cancellationToken);
    }

    public async Task<SharifApiResponse<RegisterLoanResponse>> RegisterLoanAsync(
        RegisterLoanCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.MemberBarcode))
        {
            throw new ArgumentException("MemberBarcode cannot be empty", nameof(command));
        }

        if (command.RfidUids == null || command.RfidUids.Count == 0)
        {
            throw new ArgumentException("RfidUids cannot be empty", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.OptHashValue))
        {
            throw new ArgumentException("OptHashValue cannot be empty", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.RequestId))
        {
            throw new ArgumentException("RequestId cannot be empty", nameof(command));
        }

        var endpoint = "/v1/kiosk/loans";
        var request = new RegisterLoanRequest
        {
            MemberBarcode = command.MemberBarcode,
            RfidUids = command.RfidUids,
            OptHashValue = command.OptHashValue,
            RequestId = command.RequestId
        };

        return await _httpClientHandler.PostAsync<RegisterLoanResponse>(endpoint, request, cancellationToken);
    }

    public async Task<SharifApiResponse<RenewLoanResponse>> RenewLoanAsync(
        RenewLoanCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.RfidUids == null || command.RfidUids.Count == 0)
        {
            throw new ArgumentException("RfidUids cannot be empty", nameof(command));
        }

        var endpoint = "/v1/kiosk/renewals/by-rfid-uids";
        var request = new RenewLoanRequest
        {
            RfidUids = command.RfidUids
        };

        return await _httpClientHandler.PostAsync<RenewLoanResponse>(endpoint, request, cancellationToken);
    }

    public async Task<SharifApiResponse<ReturnLoanResponse>> ReturnLoanAsync(
        ReturnLoanCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.RfidUids == null || command.RfidUids.Count == 0)
        {
            throw new ArgumentException("RfidUids cannot be empty", nameof(command));
        }

        var endpoint = "/v1/kiosk/returns/by-rfid-uids";
        var request = new ReturnLoanRequest
        {
            RfidUids = command.RfidUids
        };

        return await _httpClientHandler.PostAsync<ReturnLoanResponse>(endpoint, request, cancellationToken);
    }

    public async Task<SharifApiResponse<CheckExitPermissionResponse>> CheckExitPermissionAsync(
        CheckExitPermissionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.RfidUids == null || command.RfidUids.Count == 0)
        {
            throw new ArgumentException("RfidUids cannot be empty", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.GateId))
        {
            throw new ArgumentException("GateId cannot be empty", nameof(command));
        }

        var endpoint = "/v1/kiosk/gate/exit-checks/by-rfid-uids";
        var request = new CheckExitPermissionRequest
        {
            RfidUids = command.RfidUids,
            GateId = command.GateId
        };

        return await _httpClientHandler.PostAsync<CheckExitPermissionResponse>(endpoint, request, cancellationToken);
    }
}

