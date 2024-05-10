using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.Testing;

public class TestHelper
{
    public static Company GetTestCompany()
    {
        return new Company() { Name = "TestCompany", IsActive = true, Id = 1, InstitutionId = 1, AcademicProgramId = 1, CompanyDetailsId = 1, UserId = "98aad5f7-1fdc-45a1-9015-fbbfaf79e351"};
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
    
    public static User GetTestAdminUser()
    {
        return new User() { UserType = UserTypeEnum.Admin, Id = "98aad5f7-1fdc-45a1-9015-fbbfaf79e351", };
    }
}