# Addition to Swagger Documentation
This documentation is only an addition to the already existing [swagger documentation](https://dual-dating-backend.msd-moss-test.fh-joanneum.at/swagger/index.html)

## Roles
There are 4 user roles for this application:
1. Admin - rights for everything including creating new admin users and institutions
2. Institution - rights for creating and updating new user and companies within the institution, reset password (send email via email-client), get liked/disliked companies of a student, delete user/companies (delete on cascade)
3. Company - rights for the company related to the user: update company properties, details, activities, locations, isActive
4. Student - rights for liking/disliking companies, get active companies

All users can change their password, and they also need to change it after the password got reset (isNew flag).

## Login
Every endpoint in this application needs authorization, therefore a user login is mandatory.

### Mock User:
- Admin: 
  - admin@fh-joanneum.at
  - Administrator!1
- Institution:
  - institution@fh-joanneum.at
  - Institution!1
- Company 1:
  - company1@fh-joanneum.at
  - Company!1
- Company 2:
  - company2@fh-joanneum.at
  - Company!1
- Student 1:
  - student1@fh-joanneum.at
  - Student!1
- Student 2:
    - student2@fh-joanneum.at
    - Student!1

### Admin, Institution, Company (WebApp)
The Api uses Cookie authorization. There is no need to do anything.
Response after logging in: 200 OK

### Student (App)
The Api also uses Bearer Token Authorization.
#### Login
Response after logging in:

```
{
"tokenType": "Bearer",
"accessToken": „string“,
"expiresIn": 3600, (1 hour after login)
"refreshToken": „string“,
"isNew": bool (to be implemented for changePassword)
}
```

The AccessToken (Bearer) and RefreshToken need to be saved in a secure storage and the AccessToken has to be included in the request header as Authorization using the prefix 'Bearer'
Example:

`-H 'Authorization: Bearer <TOKEN>'`

#### Refresh Token
After the token is expired, it needs to be refreshed. Simply send the RefreshToken and you get a new AccessToken as same response as in Login.

## Student Endpoints (App)

Following Endpoints are for student users:

### GET
- /Company/ActiveCompanies
- /Company/Details?id=\<COMPANYID>
- /Company/Activities?id=\<COMPANYID>


- /Company/Locations?id=\<COMPANYID> (to be implemented)
- /Company/LikedCompanies (to be implemented)
- /Schedule/Dates (to be implemented)
- Filtering (to be implemented)

### POST
- /User/Login
- /User/Refresh
- /User/ChangePassword
- /User/DeleteUserWithPassword


- /Company/LikeCompany (to be implemented)

Response Types are listed in Swagger Documentation

### Admin Endpoints (WebApp)

Following Endpoints are for admin users:

### GET
- /Company?id=<COMPANYID>
- /Company/Companies?institutionId=\<INSTITUTIONID>&academicProgramId=\<ACADEMICPROGRAMID>
- /Company/Details?id=\<COMPANYID>
- /Company/Activities?id=\<COMPANYID>
- /User/GetAllUsers?institutionId=\<INSTITUTIONID>&academicProgramId=\<ACADEMICPROGRAMID>
- /User/GetUser?id=\<USERID>


- Filtering (to be implemented)
- /Institution/GetInstitutions (to be implemented)
- /Institution/GetInstitution?id=\<INSTITUTIONID> (to be implemented)
- /AcademicProgram/GetPrograms?id=\<INSTITUTIONID> (to be implemented)
- /AcademicProgram/GetProgram?id=\<ACADEMICPROGRAMID> (to be implemented)
- /Likes/GetLikes?id=\<STUDENTID> (to be implemented)
- /Schedule/GetStudentDates?id=\<STUDENTID> (to be implemented)
- /Schedule/GetCompanyDates?id=\<COMPANYID> (to be implemented)

### POST
- /User/Login
- /User/ResetPassword?id=\<USERID>
- /User/ChangePassword
- /User/DeleteUserWithPassword
- /Company/Register

### PUT
- /User/Register
- /Company/IsActive?id=\<COMPANYID>


- /Student/CreateStudents (with csv -> json) (to be implemented)
- /Company/CreateCompanies (with csv -> json) (to be implemented)

### DELETE
- /User/DeleteUser?id=\<USERID>


- /User/DeleteStudents?id=<ACADEMICPROGRAM> (all students from academic program) (to be implemented)


### Institution Endpoints (WebApp)

Following Endpoints are for admin users:

### GET
- /Company?id=\<COMPANYID>
- /Company/Companies?academicProgramId=\<ACADEMICPROGRAMID>
- /Company/Details?id=\<COMPANYID>
- /Company/Activities?id=\<COMPANYID>
- /User/GetAllUsers?academicProgramId=\<ACADEMICPROGRAMID>
- /User/GetUser?id=\<USERID>


- /AcademicProgram/GetPrograms (to be implemented)
- /AcademicProgram/GetProgram?id=\<ACADEMICPROGRAMID> (to be implemented)
- /Likes/GetLikes?id=\<STUDENTID> (to be implemented)
- /Schedule/GetStudentDates?id=\<STUDENTID> (to be implemented)
- /Schedule/GetCompanyDates?id=\<COMPANYID> (to be implemented)

### POST
- /User/Login
- /User/ResetPassword?id=\<USERID>
- /User/ChangePassword
- /User/DeleteUserWithPassword
- /Company/Register

### PUT
- /User/Register
- /Company/IsActive?id=\<COMPANYID>


- /Student/CreateStudents (with csv -> json) (to be implemented)
- /Company/CreateCompanies (with csv -> json) (to be implemented)

### DELETE
- /User/DeleteUser?id=\<USERID>


- /User/DeleteStudents?id=\<ACADEMICPROGRAM> (all students from academic program) (to be implemented)

### Company Endpoints (WebApp)

Following Endpoints are for admin users:

### GET
- /Company
- /Company/Details
- /Company/Activities


- /Schedule/GetCompanyDates (to be implemented)

### POST
- /User/Login
- /Company/Activities
- /User/ChangePassword
- /User/DeleteUserWithPassword

### PUT
- /Company
- /Company/Details
- /Company/IsActive

There is more endpoints to be implemented.

User Information is included in Token/Cookie, which means there is no need to provide an id for endpoints related to the logged in user. 
