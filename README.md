# Client Features:
- Create a Contract with optional upload of contract file
- View all users contracts

# Logistics Manager Features:
- Create Service Request
- Update Contract Status
- View Active Contracts
- Filter Contracts
- View Contract Files

# Admin Features:
- View All Contracts
- Manage Users which include Create user, Delete user, change role of user and view all users

# Implemented Design Patterns:
- Factory Pattern
- Observer Pattern
- Strategy Pattern

# Login Details for testing purposes:
- Account: Client
  - email: client@test.com
  - password: Client@123

- Account: Logistics Manager
  - email: manager@test.com
  - password: Manager@123

- Account: Admin
  - email: admin@test.com
  - password: Admin@123

# Details of where I implemented Architectural and Design Patterns:
 **Architectural Pattern**
 - I created a Blazor Web App which follows the MVC Architecture. This is shown by having different files in my project solution such as Data folder for Data related files, Controller folder for Controllers, Models folder for data model files, Services file for the service layer and Pages folder for the UI pages.

**Design Patterns**
- Factory Pattern:
  - The Factory Pattern was implemented to allow the creation of different types of contract. A ContractFactory class was created which implements the IContractFactory interface. A client selects what contract type they would like to create when creating a contract and then that contract type is then passed to the factory. The factory then uses a switch statement to decide which contract object to instantiate being either StandardContract, PremiumContract or InternationalContract.

- Observer Pattern:
  - The Observer Pattern was implemented to handle notifications when important actions happen, such as contract creation, service request creation etc. ContractService class maintains a list of IObserver objects. When an important action is performed, observers are attached to the list and are then notified by the NotifyObservers method which calls UpdateObservers on each observer and then detaches the observer once the notification has been sent. There is an email and an alert notification for each client and each manager.

- Strategy Pattern:
  - The Strategy Pattern was implemented to handle validation rules that need to be applied to contracts and contract files. The IValidationStrategy interface defines a Validate method that takes a Contract object. The strategy implements 3 validation checks, CheckCurrencyConversion which checks that International contracts have a valid currency, CheckFileTypeStrategy which checks that validates that files are .PDF and a CheckContractStatus which validates that contract status transition is valid.

# Tech Stack Used:
- ASP .NET Framework
- Blazor Web App
- Visual Studio 2026
- Github
- SQL Server

# Programming Languages Used:
- C#
- HTML
- CSS
- Javascript
- Bootstrap framework

# YouTube Video Link:
- https://youtu.be/PQAc6p9J_i4
