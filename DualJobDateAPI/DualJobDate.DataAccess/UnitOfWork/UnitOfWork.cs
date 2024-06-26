﻿using DualJobDate.BusinessObjects.Entities.Interface;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DualJobDate.DataAccess;

public class UnitOfWork(
    AppDbContext dbContext,
    IInstitutionRepository institutionRepository,
    IAcademicProgramRepository academicProgramRepository,
    IActivityRepository activityRepository,
    ICompanyRepository companyRepository,
    ICompanyDetailsRepository companyDetailsRepository,
    ICompanyActivityRepository companyActivityRepository,
    // IAdressRepository adressRepository,
    IStudentCompanyRepository studentCompanyRepository,
    IAppointmentRepository appointmentRepository) : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private IDbContextTransaction? _transaction;

    public IInstitutionRepository InstitutionRepository =>
        institutionRepository ?? throw new ArgumentNullException(nameof(institutionRepository));

    public IAcademicProgramRepository AcademicProgramRepository => academicProgramRepository ??
                                                                   throw new ArgumentNullException(
                                                                       nameof(academicProgramRepository));

    public IActivityRepository ActivityRepository =>
        activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));

    public ICompanyRepository CompanyRepository =>
        companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));

    public ICompanyDetailsRepository CompanyDetailsRepository => companyDetailsRepository ??
                                                                 throw new ArgumentNullException(
                                                                     nameof(companyDetailsRepository));

    public ICompanyActivityRepository CompanyActivityRepository => companyActivityRepository ??
                                                                   throw new ArgumentNullException(
                                                                       nameof(companyActivityRepository));

    // public IAdressRepository AdressRepository =>
    //     adressRepository ?? throw new ArgumentNullException(nameof(adressRepository));
    
    public IStudentCompanyRepository StudentCompanyRepository =>
        studentCompanyRepository ?? throw new ArgumentNullException(nameof(studentCompanyRepository));

    public IAppointmentRepository AppointmentRepository =>
        appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));

    public void BeginTransaction()
    {
        _transaction = _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        catch (InvalidOperationException ex)
        {
            Rollback();
            throw new InvalidOperationException("Error committing transaction.", ex);
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("Error rolling back transaction.", ex);
        }
    }

    public async Task SaveChanges()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Error updating entities.", ex);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}