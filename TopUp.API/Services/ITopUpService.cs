using TopUp.API.Models;

namespace TopUp.API.Services
{
    public interface ITopUpService
    {
        Task<TopUpBeneficiary> AddTopUpBeneficiary(TopUpBeneficiary beneficiary);
        Task<TopUpBeneficiary> DeleteTopUpBeneficiary(Guid id);
        Task<TopUpBeneficiary> GetTopUpBeneficiary(Guid id);
        Task<List<TopUpBeneficiary>> GetUserTopUpBeneficiaries(Guid userId);
        Task TopUp(decimal amount, Guid beneficiaryId, Guid userId);
        Task<TopUpBeneficiary> UpdateTopUpBeneficiary(TopUpBeneficiary beneficiary);

        Task<List<TopUpOption>> GetTopUpOptions();
    }
}