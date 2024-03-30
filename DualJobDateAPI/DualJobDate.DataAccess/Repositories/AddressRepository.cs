using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class AddressRepository(AppDbContext dbContext) : BaseRepository<Address>(dbContext), IAdressRepository;