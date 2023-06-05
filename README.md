# File Sharing Web Project


An ASP .NET web app used for uploading and sharing files and texts.


## Features

- Register and login to start uploading and sharing your files.
- Files is uploaded to AWS S3, information related to files and user is stored in a SQL Database.
- File URL are encrypted with SHA256.
- Guests can view file/text using link shared by users.


## Installation
- Reconfigure appsettings.json (replace my test keys and database DefaultConnection)
- Replace RegionEndPoint in Home and User Controllers if your region is not Asia Pacific (Singapore)
- Please note that the link used for sharing files feature is hard coded at Views/User/ManageFiles.cshtml at line 133 (I'm testing with mutiple ports so I need a way to easily change my links)
- Run database migration or use your preferred method.
- Build the solution.


## IDE/Tools used

- Visual Studio 2022
- Microsoft SQL Server Management Studio
- AWS S3

## Features in development

- Docker File
- Alerts on successful operations.
- Tests

##Known bugs
- Sometimes the uploaded file are not immediately accessible via shared link
