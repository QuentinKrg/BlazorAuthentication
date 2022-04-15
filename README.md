# Blazor Webassembly Custom Authentication
#### Projet template for any project that uses on premises ressources

This project allows authentication to any services of your choice, like from LDAP or a DB.

## Features
- Authentication and Authorization with JWT token stored in local storage
- Manage authorization with roles and policies on Client and Server side
- Custom HTTP Extension to secure all requests made on the server from the client and handle Authorization on the server side

## Configuration
This part describes the configuration elements specific to each project

- In the appsettings.json file in Project.Server, modify the `JWTSettings:SecretKey`. This value is used to hash the JWT token and must be unique for each projet.
- You can reference the different AD groups that have access to the application in Controllers/UserController.cs and use those same groups in the Client and Server projects to check the authorizations.

## Description
This diagram describes the operation and the actors of the authentication system at the time of the connection of a user.

This diagram describes the authorizazion system when an user tries to access a protected page or controller.


## Tech

Different resources used :

- [JWT] - JSON Web Tokens!
- [BlazingChat] - Project insprirations

   [JWT]: <https://jwt.io/>
   [BlazingChat]: <https://github.com/CuriousDrive/BlazingChat>
