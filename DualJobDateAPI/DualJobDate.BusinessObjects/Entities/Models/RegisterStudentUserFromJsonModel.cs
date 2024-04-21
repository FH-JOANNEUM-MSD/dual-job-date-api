﻿namespace DualJobDate.BusinessObjects.Entities.Models;

public class RegisterStudentUserFromJsonModel
{
    public required string Email { get; set; }
    public required int AcademicProgramYear { get; set; }
    public required string AcademicProgramKeyName { get; set; }
    public required string InstitutionKeyName { get; set; }
}