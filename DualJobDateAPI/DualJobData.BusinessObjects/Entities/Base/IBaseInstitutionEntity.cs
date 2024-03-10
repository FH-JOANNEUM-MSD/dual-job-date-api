namespace DualJobData.BusinessLogic.Entities.Base
{
    public interface IBaseInstitutionEntity : IBaseEntity
    {
        public Institution? Institution { get; set; }
        public int InstitutionId { get; set; }
    }
}
