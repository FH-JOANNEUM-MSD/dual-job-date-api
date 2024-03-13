using DualJobDate.BusinessObjects.Entities.Base;

namespace DualJobDate.BusinessObjects.Entities;

public class CompanyActivity : BaseEntity
{
    public int Value { get; set; }
    
    //navigation properties
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public int ActivityId { get; set; }
    public Activity Activity { get; set; }
}