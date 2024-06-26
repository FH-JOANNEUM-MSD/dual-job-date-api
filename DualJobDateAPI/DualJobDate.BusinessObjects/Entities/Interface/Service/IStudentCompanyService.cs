﻿using DualJobDate.BusinessObjects.Entities.Models;

namespace DualJobDate.BusinessObjects.Entities.Interface.Service;

public interface IStudentCompanyService
{
    Task<List<StudentCompany>> GetStudentCompaniesAsync();
    Task<List<StudentCompany>> GetStudentCompaniesByStudentIdAsync(string studentId);
    Task<StudentCompany?> GetStudentCompanyByIdAsync(int id);
    Task<StudentCompany?> CreateStudentCompanyAsync(bool like, int companyId, string studentId);
    Task<bool> DeleteStudentCompanyAsync(int id);

    Dictionary<User, List<Tuple<Company, DateTime>>> MatchCompaniesToStudents(List<User> students,
        List<Company> companies, MatchModel model);

    Task SaveAppointments(List<Appointment> appointments);

    Task DeleteAppointments(int academicProgramId);

}