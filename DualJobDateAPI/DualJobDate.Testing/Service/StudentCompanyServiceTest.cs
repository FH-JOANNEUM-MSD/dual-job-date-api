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
            var students = GetStudent();
            var companies = GetCompanies();
            var matchPerStudent = 6;
            var matches = _service.MatchCompaniesToStudents(students, companies, matchPerStudent);

            foreach (var student in students)
            {
                Assert.True(matches.ContainsKey(student));
                var matchedCompanies = matches[student];
                Assert.Equal(matchPerStudent, matchedCompanies.Count);
                var votes = student.StudentCompanies;

                var likedCompanies = votes
                    .Where(x => x.StudentId == student.Id && x.Like)
                    .Select(x => x.CompanyId).ToList();

                var dislikedCompanies = votes
                    .Where(x => x.StudentId == student.Id && !x.Like)
                    .Select(x => x.CompanyId).ToList();

                var eligibleCompanies = companies.Select(x => x.Id)
                    .Except(likedCompanies.Concat(dislikedCompanies)).ToList();

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

        [Fact]
        public void MatchStudentWithCompanyDistributionTest()
        {
            var students = GetStudent();
            var companies = GetCompanies().Take(10).ToList();
            var matchPerStudent = 6;
            var matches = _service.MatchCompaniesToStudents(students, companies, matchPerStudent);

            var companyCounts = companies.ToDictionary(c => c.Id, c => 0);

            foreach (var company in matches.Values.SelectMany(x => x))
            {
                companyCounts[company.Id]++;
            }

            var test = companies.Count / students.Count;
            foreach (var count in companyCounts.Values)
            {

                Assert.InRange(count, 5, 6);
            }
        }

        private List<Company> GetCompanies() =>
        [
            new Company { Id = 1, Name = "Company1", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 2, Name = "Company2", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 3, Name = "Company3", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 4, Name = "Company4", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 5, Name = "Company5", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 6, Name = "Company6", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 7, Name = "Company7", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 8, Name = "Company8", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 9, Name = "Company9", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 10, Name = "Company10", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 11, Name = "Company11", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 12, Name = "Company12", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 13, Name = "Company13", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 14, Name = "Company14", AcademicProgramId = 1, InstitutionId = 1 },
            new Company { Id = 15, Name = "Company15", AcademicProgramId = 1, InstitutionId = 1 },
        ];

        private static List<User> GetStudent()
        {
            var students = new List<User>();
            for (int i = 0; i < 10; i++)
            {
                students.Add(new User
                {
                    Id = i.ToString(),
                    StudentCompanies = CreateStudentCompany(i.ToString())
                });
            }

            return students;
        }
            
        private static List<StudentCompany> CreateStudentCompany(string studentId) =>
        [
            new StudentCompany { StudentId = studentId, CompanyId = 1, Like = true },
            new StudentCompany { StudentId = studentId, CompanyId = 2, Like = true },
            new StudentCompany { StudentId = studentId, CompanyId = 3, Like = true },
            new StudentCompany { StudentId = studentId, CompanyId = 4, Like = true },
            new StudentCompany { StudentId = studentId, CompanyId = 5, Like = true },
            new StudentCompany { StudentId = studentId, CompanyId = 6, Like = false },
            new StudentCompany { StudentId = studentId, CompanyId = 7, Like = false },
            new StudentCompany { StudentId = studentId, CompanyId = 8, Like = false },
            new StudentCompany { StudentId = studentId, CompanyId = 9, Like = false },
            new StudentCompany { StudentId = studentId, CompanyId = 10, Like = false }
        ];
    }
}
