using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class CompanyDetails : BaseEntity
{
    public string ShortDescription { get; set; }
    public string JobDescription { get; set; }
}