using DBO.Data.Models;
using System;
using System.Collections.Generic;

namespace DBO.Services.Contract
{
    public interface IAdsService
    {
        List<Advertisement> GetAllForUser(Guid userId);
        List<Advertisement> GetAll();
        Advertisement GetById(int id, Guid userId);
        Advertisement GetById(int id);
        Advertisement Create(Advertisement model);
        void Remove(int id, Guid userId);
        Advertisement Update(Advertisement model);
        Advertisement OpenAd(int id, string currentUserId, string ipAddress);
        IEnumerable<Advertisement> DisplayAds(int count, int companyId = -1);
        string CheckAdsVisibilityForCurrentUser(Advertisement ad);
        void TransferBudget(int currentId, int transferAdId, decimal amount, bool transferToAnother, out string error);
        decimal StopAdvertisement(int id);
    }
}