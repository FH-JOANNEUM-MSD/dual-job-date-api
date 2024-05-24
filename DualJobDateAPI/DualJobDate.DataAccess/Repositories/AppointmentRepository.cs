using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Interface.Repository;

namespace DualJobDate.DataAccess.Repositories;

public class AppointmentRepository(AppDbContext dbContext) : BaseRepository<Appointment>(dbContext), IAppointmentRepository;