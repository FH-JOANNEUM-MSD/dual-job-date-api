﻿using DualJobDate.BusinessObjects.Entities.Enum;

namespace DualJobDate.BusinessObjects.Resources;

public class AcademicProgramResource
{
    public string Id { get; set; }
    public int Year { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public AcademicDegreeEnum AcademicDegreeEnum { get; set; } = AcademicDegreeEnum.Default;
}