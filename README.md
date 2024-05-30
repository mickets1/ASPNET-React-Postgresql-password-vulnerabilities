# Password Vulnerability Statistics Web API

## Description

This is a password vulnerability API to help detect and handle unsecure passwords.

## Features

- CRUD operations for categories, passwords, and time units.
- Paginated endpoint for fetching passwords with related information.
- Easy to extend

## Install and run
- I used PostgreSQL together with PHPpgAdmin:
https://help.clouding.io/hc/en-us/articles/360016532280-How-to-Install-PostgreSQL-and-phpPgAdmin-on-Ubuntu-20-04

- Open appsettings.json and modify the ConnectionStrings section with your database details.

- dotnet ef migrations add mig1
- dotnet ef database update
- dotnet run

## Routes
CategoriesController.cs

    GET: api/Categories
    Fetches all categories.
    Not accessible via the interface; use Postman or similar.

    GET: api/Categories/{id}
    Fetches a specific category based on ID.
    Not accessible via the interface; use Postman or similar.

    POST: api/Categories/add
    Creates a new category.

    PUT: api/Categories/{id}
    Updates a specific category based on ID.
    Not accessible via the interface; use Postman or similar.

    DELETE: api/Categories/{id}
    Deletes a specific category based on ID.
    Not accessible via the interface; use Postman or similar.

PasswordsController.cs

    GET: api/Passwords/with-relations
    Fetches all passwords with related information.
    Specifically, we call the route with a specific page for pagination to work: GET: api/Passwords/with-relations?page={page}
    This route "displays" the results we can see in the view.

    GET: api/Passwords/{id}
    Fetches a specific password based on ID.
    Not accessible via the interface; use Postman or similar.

    POST: api/Passwords/add
    Creates a new password.

    PUT: api/Passwords/{id}
    Updates a specific password based on ID.

    DELETE: api/Passwords/{id}
    Deletes a specific password based on ID.

TimeUnitController.cs
Routes here are used in a similar way as in CategoriesController.