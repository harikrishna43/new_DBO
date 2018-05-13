using DBO.Data.Models;
using System.Collections.Generic;

namespace DBO.Services.Contract
{
    public interface ISubdataService
    {
        IEnumerable<INamedEntity> GetSkills();
        IEnumerable<INamedEntity> GetIndustries();
    }
}
