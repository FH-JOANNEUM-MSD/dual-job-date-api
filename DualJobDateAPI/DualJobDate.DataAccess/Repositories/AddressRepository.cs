using DualJobDate.BusinessObjects.Entities;

namespace DualJobDate.DataAccess.Repositories;

public class AddressRepository(AppDbContext dbContext) : BaseRepository<Address>(dbContext);