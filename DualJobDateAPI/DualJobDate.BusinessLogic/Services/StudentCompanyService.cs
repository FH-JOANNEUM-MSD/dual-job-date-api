using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.EntityFrameworkCore;

namespace DualJobDate.BusinessLogic.Services;

public class StudentCompanyService(IUnitOfWork unitOfWork) : IStudentCompanyService
{
    public async Task<List<StudentCompany>> GetStudentCompaniesAsync()
    {
        var result = (await unitOfWork.StudentCompanyRepository.GetAllAsync());
        return await result.ToListAsync();
    }

    public async Task<List<StudentCompany>> GetStudentCompaniesByStudentIdAsync(string studentId)
    {
        var result = (await unitOfWork.StudentCompanyRepository.GetAllAsync()).Where(x => x.StudentId == studentId);
        return await result.ToListAsync();
    }

    public async Task<StudentCompany?> GetStudentCompanyByIdAsync(int id)
    {
        return await (await unitOfWork.StudentCompanyRepository.GetAllAsync()).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<StudentCompany?> CreateStudentCompanyAsync(bool like, int companyId, string studentId)
    {
        var studentCompany =
            (await unitOfWork.StudentCompanyRepository.GetAllAsync()).FirstOrDefault(x =>
                x.StudentId == studentId && x.CompanyId == companyId);

        try
        {
            unitOfWork.BeginTransaction();
            if (studentCompany == null)
            {
                studentCompany = new StudentCompany()
                {
                    CompanyId = companyId,
                    Like = like,
                    StudentId = studentId
                };

                await unitOfWork.StudentCompanyRepository.AddAsync(studentCompany);
            }
            else
            {
                studentCompany.Like = like;
                await unitOfWork.StudentCompanyRepository.UpdateAsync(studentCompany);
            }

            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
            return studentCompany;
        }
        catch
        {
            return null;
        }

    }

    public async Task<bool> DeleteStudentCompanyAsync(int id)
    {
        try
        {
            unitOfWork.BeginTransaction();
            await unitOfWork.StudentCompanyRepository.DeleteAsync(id);
            unitOfWork.Commit();
            await unitOfWork.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }

    }

    public Dictionary<User, List<Company>> MatchCompaniesToStudents(List<User> students, List<Company> companies,
        int matchesPerStudent = 6)
    {
        var companyPickCount = companies.ToDictionary(company => company, _ => 0);
        var dictionary = new Dictionary<User, List<Company>>();
        foreach (var student in students)
        {
            var availableCompanies = companyPickCount.Keys.OrderBy(x => companyPickCount[x]).ToList();

            var likedCompanyIds = student.StudentCompanies
                .Where(x => x.Like)
                .Select(x => x.CompanyId)
                .ToList();

            var likedCompanies = availableCompanies
                .Where(x => likedCompanyIds.Contains(x.Id))
                .ToList();

            var dislikedCompanyIds = student.StudentCompanies
                .Where(x => !x.Like)
                .Select(x => x.CompanyId)
                .ToList();

            var dislikedCompanies = availableCompanies
                .Where(x => dislikedCompanyIds.Contains(x.Id))
                .ToList();

            var neutralCompanies = availableCompanies
                .Except(likedCompanies.Concat(dislikedCompanies)).ToList();

            var selectCompanies = new List<Company>();
            selectCompanies.AddRange(likedCompanies.AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(matchesPerStudent / 2));
            selectCompanies.AddRange(neutralCompanies.AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(matchesPerStudent / 2));

            if (selectCompanies.Count < matchesPerStudent)
            {
                selectCompanies.AddRange(neutralCompanies.Except(selectCompanies).AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(matchesPerStudent - selectCompanies.Count));
            }
            
            if (selectCompanies.Count < matchesPerStudent)
            {
                selectCompanies.AddRange(dislikedCompanies.Except(selectCompanies).AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(matchesPerStudent - selectCompanies.Count));
            }

            dictionary.Add(student, selectCompanies);

            selectCompanies.ForEach(x => companyPickCount[x]++);
        }
        

        return dictionary;
    }

    public async Task SaveAppointments(List<Appointment> appointments)
    {
        unitOfWork.BeginTransaction();
        await unitOfWork.AppointmentRepository.AddRangeAsync(appointments);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges(); 
    }

    public async Task DeleteAppointments(int academicProgramId)
    {
        unitOfWork.BeginTransaction();
        var appointments = (await unitOfWork.AppointmentRepository.GetAllAsync()).Where(x => x.User.AcademicProgramId == academicProgramId);
        await unitOfWork.AppointmentRepository.DeleteRangeAsync(appointments);
        unitOfWork.Commit();
        await unitOfWork.SaveChanges();
    }
}