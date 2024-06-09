using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using DualJobDate.BusinessObjects.Entities.Models;
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

public Dictionary<User, List<Tuple<Company, DateTime>>> MatchCompaniesToStudents(List<User> students, List<Company> companies, MatchModel model)
{
    var companyPickCount = companies.ToDictionary(company => company, _ => 0);
    var companySchedules = companies.ToDictionary(company => company, _ => new List<DateTime>());
    var dictionary = new Dictionary<User, List<Tuple<Company, DateTime>>>();

    int totalSlots = (int)(model.EndTime - model.StartTime).TotalHours;
    var duration = TimeSpan.FromHours(1);

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
        selectCompanies.AddRange(likedCompanies.AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(model.MatchesPerStudent / 2));
        selectCompanies.AddRange(neutralCompanies.AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(model.MatchesPerStudent / 2));

        if (selectCompanies.Count < model.MatchesPerStudent)
        {
            selectCompanies.AddRange(neutralCompanies.Except(selectCompanies).AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(model.MatchesPerStudent - selectCompanies.Count));
        }

        var studentSchedule = new List<Tuple<Company, DateTime>>();
        DateTime currentTime = model.StartTime;

        for (int i = 0; i < model.MatchesPerStudent; i++)
        {
            Company assignedCompany = null;
            foreach (var company in selectCompanies)
            {
                if (!studentSchedule.Any(s => s.Item1 == company) && !companySchedules[company].Contains(currentTime))
                {
                    assignedCompany = company;
                    break;
                }
            }

            if (assignedCompany == null)
            {
                foreach (var company in neutralCompanies)
                {
                    if (!studentSchedule.Any(s => s.Item1 == company) && !companySchedules[company].Contains(currentTime))
                    {
                        assignedCompany = company;
                        selectCompanies.Add(company); // Add to selected to prevent re-adding
                        break;
                    }
                }
            }

            if (assignedCompany != null)
            {
                studentSchedule.Add(new Tuple<Company, DateTime>(assignedCompany, currentTime));
                companySchedules[assignedCompany].Add(currentTime);
                companyPickCount[assignedCompany]++;
            }
            currentTime = currentTime.Add(duration);
        }

        while (studentSchedule.Count < model.MatchesPerStudent && studentSchedule.Count < totalSlots)
        {
            bool slotFound = false;
            foreach (var company in availableCompanies.Except(dislikedCompanies))
            {
                if (studentSchedule.Count >= model.MatchesPerStudent) break;
                currentTime = model.StartTime + duration * studentSchedule.Count;
                if (currentTime >= model.EndTime) break;
                if (!studentSchedule.Any(s => s.Item1 == company) && !companySchedules[company].Contains(currentTime))
                {
                    studentSchedule.Add(new Tuple<Company, DateTime>(company, currentTime));
                    companySchedules[company].Add(currentTime);
                    companyPickCount[company]++;
                    slotFound = true;
                }
            }
            if (!slotFound) break;
        }

        dictionary.Add(student, studentSchedule);
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