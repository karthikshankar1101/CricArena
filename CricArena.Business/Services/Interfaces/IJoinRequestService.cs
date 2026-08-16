using CricArena.Business.DTOs.JoinRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Services.Interfaces
{
    public interface IJoinRequestService
    {
        Task<JoinRequestResponse> CreateAsync(
        CreateJoinRequestRequest request);

        Task<List<JoinRequestResponse>> GetClubRequestsAsync(
            Guid clubId);

        Task ApproveAsync(Guid requestId);

        Task RejectAsync(Guid requestId);

        Task CancelAsync(Guid requestId);
    }
}
