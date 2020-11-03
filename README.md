# Library
A library application in which you can create, edit and lend library items. You can also manage employees. Made in ASP.NET Core and as a part of a coding challenge. 


Commands in ../LibraryBackEnd for generating DB

dotnet tool install --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add FinalCreate

dotnet ef database update
