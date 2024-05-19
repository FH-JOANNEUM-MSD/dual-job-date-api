using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using Moq;
using Xunit;

namespace DualJobDate.Testing.Service
{
    public class StudentCompanyServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly StudentCompanyService _service;

        public StudentCompanyServiceTest()
        {
            var userManagerMock = MockHelpers.MockUserManager<User>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new StudentCompanyService(_unitOfWorkMock.Object);
        }

        [Fact]
        public void MatchStudentWithCompanyTest()
        {
            var companies = GetCompanies();
            var students = GetStudent();
            var votes = CreateStudentCompany();
            var matchPerStudent = 6;
            var matches = _service.MatchCompaniesToStudents(students, companies, votes, matchPerStudent);

            foreach (var student in students)
            {
                Assert.True(matches.ContainsKey(student));
                var matchedCompanies = matches[student];
                Assert.Equal(matchPerStudent, matchedCompanies.Count);

                var likedCompanies = votes
                    .Where(x => x.StudentId == student.Id && x.Like)
                    .Select(x => x.CompanyId).ToList();

                var dislikedCompanies = votes
                    .Where(x => x.StudentId == student.Id && !x.Like)
                    .Select(x => x.CompanyId).ToList();

                var eligibleCompanies = companies
                    .Where(c => c.AcademicProgramId == student.AcademicProgramId && c.InstitutionId == student.InstitutionId)
                    .Select(c => c.Id)
                    .ToList();

                var neutralCompanies = eligibleCompanies
                    .Where(x => !likedCompanies.Contains(x) && !dislikedCompanies.Contains(x))
                    .ToList();

                var matchedLikedCompanies = matchedCompanies
                    .Where(x => likedCompanies.Contains(x.Id))
                    .ToList();

                var matchedNeutralCompanies = matchedCompanies
                    .Where(x => neutralCompanies.Contains(x.Id))
                    .ToList();

                Assert.Equal(matchPerStudent / 2, matchedLikedCompanies.Count);
                Assert.Equal(matchPerStudent / 2, matchedNeutralCompanies.Count);
            }
        }

        private List<Company> GetCompanies() =>
        [
            new Company { Id = 1, Name = "Company1", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 2, Name = "Company2", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 3, Name = "Company3", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 4, Name = "Company4", AcademicProgramId = 2, InstitutionId = 1 },
            new Company { Id = 5, Name = "Company5", AcademicProgramId = 2, InstitutionId = 1 },
            new Company { Id = 6, Name = "Company6", AcademicProgramId = 2, InstitutionId = 1 },
            new Company { Id = 7, Name = "Company7", AcademicProgramId = 1, InstitutionId = 2 },
            new Company { Id = 8, Name = "Company8", AcademicProgramId = 2, InstitutionId = 2 }
        ];

        private static List<User> GetStudent() =>
        [
            new User { Id = "1", AcademicProgramId = 1, InstitutionId = 1 },
            new User { Id = "2", AcademicProgramId = 1, InstitutionId = 1 },
            new User { Id = "3", AcademicProgramId = 2, InstitutionId = 1 }
        ];

        private List<StudentCompany> CreateStudentCompany() =>
        [
            new StudentCompany { StudentId = "1", CompanyId = 1, Like = true },
            new StudentCompany { StudentId = "1", CompanyId = 2, Like = true },
            new StudentCompany { StudentId = "1", CompanyId = 3, Like = true },
            new StudentCompany { StudentId = "2", CompanyId = 1, Like = true },
            new StudentCompany { StudentId = "2", CompanyId = 2, Like = true },
            new StudentCompany { StudentId = "3", CompanyId = 4, Like = true },
            new StudentCompany { StudentId = "3", CompanyId = 5, Like = true },
            new StudentCompany { StudentId = "3", CompanyId = 6, Like = true },
            new StudentCompany { StudentId = "1", CompanyId = 4, Like = false },
            new StudentCompany { StudentId = "2", CompanyId = 5, Like = false }
        ];
    }
}
