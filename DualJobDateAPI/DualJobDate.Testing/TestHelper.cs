using System.Diagnostics;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using Microsoft.AspNetCore.Identity;

namespace DualJobDate.Testing;

public class TestHelper
{
    public static Company GetTestCompany()
    {
        return new Company() { Name = "TestCompany", IsActive = true, Id = 1, AcademicProgramId = 1, CompanyDetailsId = 1, UserId = "98aad5f7-1fdc-45a1-9015-fbbfaf79e351"};
    }

    public static User GetTestUser(int? institutionId = 1, int? academicProgramId = 1, UserTypeEnum? userType = null)
    {
        return new User()
        {
            Id = "98aad5f7-1fdc-45a1-9015-fbbfaf79e351",
            InstitutionId = institutionId.Value,
            AcademicProgramId = academicProgramId.Value,
            UserType = userType ?? UserTypeEnum.Admin,
        };
    }
}