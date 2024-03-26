using System.Diagnostics;
using DualJobDate.BusinessObjects.Entities;
using DualJobDate.BusinessObjects.Entities.Enum;
using DualJobDate.BusinessObjects.Entities.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Activity = DualJobDate.BusinessObjects.Entities.Activity;

namespace DualJobDate.DatabaseInitializer
{
    public static class DbInitializer
    {
        public static void InitializeDb(ILoggerFactory loggerFactory)
        {

            var logger = loggerFactory.CreateLogger("DbInitializer");
            logger.LogInformation("Starting container using Docker Compose...");

            logger.LogInformation("Starting container using Docker Compose...");
            var workingDirectory = Directory.GetCurrentDirectory();
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker-compose",
                Arguments = "-f db-dev-compose.yml up -d",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            using var process = Process.Start(startInfo);
            process?.WaitForExit();

            var output = process?.StandardOutput.ReadToEnd();
            var error = process?.StandardError.ReadToEnd();

            if (!string.IsNullOrWhiteSpace(output))
            {
                logger.LogInformation("Output: {Output}", output);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                logger.LogInformation("Output: {Error}", error);
            }
        }

        public static async Task SeedData(IServiceProvider services)
        {
            var uow = services.GetRequiredService<IUnitOfWork>();
            uow.BeginTransaction();
            await SeedRoles(services);
            await SeedUser(services);
            await SeedInstitution(uow);
            await SeedDegreePrograms(uow);
            await SeedAcademicProgram(uow);
            await SeedActivities(uow);
            uow.Commit();
            await uow.SaveChanges();
        }


        private static async Task SeedInstitution(IUnitOfWork unitOfWork)
        {

            if (await unitOfWork.InstitutionRepository.GetByName("IIT") == null)
            {
                await unitOfWork.InstitutionRepository.AddAsync(new Institution
                {
                    KeyName = "IIT",
                    Name = "Software Design und Security",
                    Website = "https://www.fh-joanneum.at/institut/software-design-und-security/"
                });
            }
        }

        private static async Task SeedDegreePrograms(IUnitOfWork unitOfWork)
        {
            if (unitOfWork.AcademicDegreeRepository.GetAllAsync().Result.IsNullOrEmpty())
            {
                await unitOfWork.AcademicDegreeRepository.AddAsync(new AcademicDegree
                {
                    Name = "Bachelor",
                    AcademicDegreeEnum = AcademicDegreeEnum.Bachelor
                });
                await unitOfWork.AcademicDegreeRepository.AddAsync(new AcademicDegree
                {
                    Name = "Master",
                    AcademicDegreeEnum = AcademicDegreeEnum.Master
                });
                await unitOfWork.AcademicDegreeRepository.AddAsync(new AcademicDegree
                {
                    Name = "PhD",
                    AcademicDegreeEnum = AcademicDegreeEnum.PhD
                });
                await unitOfWork.AcademicDegreeRepository.AddAsync(new AcademicDegree
                {
                    Name = "",
                    AcademicDegreeEnum = AcademicDegreeEnum.Default
                });
            }
        }

        private static async Task SeedAcademicProgram(IUnitOfWork unitOfWork)
        {
            var institution = await unitOfWork.InstitutionRepository.GetByName("IIT");
            if (await unitOfWork.AcademicProgramRepository.GetByName("MSD21") == null)
            {
                await unitOfWork.AcademicProgramRepository.AddAsync(new AcademicProgram
                {
                    Id = 1,
                    KeyName = "MSD21",
                    Name = "Mobile Software Development",
                    Year = 2021,
                    AcademicDegreeEnum = AcademicDegreeEnum.Bachelor,
                    Institution = institution,
                    InstitutionId = institution.Id
                });
            }
        }

        private static async Task SeedActivities(IUnitOfWork unitOfWork)
        {

            var academicProgram = await unitOfWork.AcademicProgramRepository.GetByName("MSD21");

            if (academicProgram.Activities.IsNullOrEmpty())
            {
                await unitOfWork.ActivityRepository.AddAsync(new Activity
                    {
                        Name = "Informatik",
                        AcademicProgram = academicProgram,
                        AcademicProgramId = academicProgram.Id
                    }
                );
                await unitOfWork.ActivityRepository.AddAsync(new Activity
                    {
                        Name = "Betriebssysteme & Datenmanagement",
                        AcademicProgram = academicProgram,
                        AcademicProgramId = academicProgram.Id
                    }
                );
                await unitOfWork.ActivityRepository.AddAsync(new Activity
                    {
                        Name = "Web Engineering",
                        AcademicProgram = academicProgram,
                        AcademicProgramId = academicProgram.Id
                    }
                );
                await unitOfWork.ActivityRepository.AddAsync(new Activity
                    {
                        Name = "Datenbanken",
                        AcademicProgram = academicProgram,
                        AcademicProgramId = academicProgram.Id
                    }
                );
                await unitOfWork.ActivityRepository.AddAsync(new Activity
                    {
                        Name = "Objektorientierte Programmierung",
                        AcademicProgram = academicProgram,
                        AcademicProgramId = academicProgram.Id
                    }
                );
            }
        }

    private static async Task SeedUser(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var userStore = services.GetRequiredService<IUserStore<User>>();
            var user = await userManager.FindByEmailAsync("dualjobdate@fh-joanneum.at");
            if (user == null)
            {
                var user1 = new User
                {
                    Email = "dualjobdate@fh-joanneum.at",
                    UserType = UserTypeEnum.Admin,
                    IsNew = false,
                    IsActive = false
                };

                await userStore.SetUserNameAsync(user1, user1.Email, CancellationToken.None);

                var password = "Administrator!1";

                var result = await userManager.CreateAsync(user1, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "Admin");
                }
            }
        }

        private static async Task SeedRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }            
            if (!await roleManager.RoleExistsAsync("Institution"))
            {
                await roleManager.CreateAsync(new IdentityRole("Institution"));
            }
            if (!await roleManager.RoleExistsAsync("Student"))
            {
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }            
            if (!await roleManager.RoleExistsAsync("Company"))
            {
                await roleManager.CreateAsync(new IdentityRole("Company"));
            }
        }
    }
}