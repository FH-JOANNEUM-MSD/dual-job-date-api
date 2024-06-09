using System.Reflection.Metadata.Ecma335;

namespace DualJobDate.BusinessLogic.Exceptions
{
    public sealed class CompanyNotFoundException : NotFoundException
    {
        public CompanyNotFoundException(int? companyId = null)
            : base(GetMessage(companyId))
        {
        }

        private static string GetMessage(int? companyId)
        {
            return companyId.HasValue
                ?  $"The company with the identifier {companyId} was not found."
                :  "Company identifier was not provided.";
        }
    }
}
