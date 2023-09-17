# Behavioral Testing
 Behavioral testing example application API. Working application example using behavioral testing patterns.
 The application is a simple employee movie ordering site that allows the creation of orders
 for a given employee using a moderate amount of business logic to calculate the order total. It has three
 dependencies:

 - LiteDb: a local file based document store database.
 - OmdbApi: a free movie database API based on OMDB.
 - Zipwise Api: a free zip code database that can return basic zip code info for 5 digit zipcodes.

 # Application Architecture

 ![Application Architecture](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/SimpleOrderingSystemArchitecture.jpg?raw=true)

 # Getting started (works with VS Code or Visual studio)

 After downloading repo, you will need to create free accounts to get access to the two external APIs used:

 - Omdb Api: Get your free key here: https://www.omdbapi.com/apikey.aspx
 - Zipwise Api: Get your free key here: https://www.zipwise.com/webservices/#verify

 Once you have these repos, you can plug the api keys into the appsettings.json file directly OR
 you can put an appsettings.json file with just the keys in the path below. This prevents you
 from accidentally committing your keys. 

 ```
c:\temp\appsettings.json
 ```

# Running the application
You should be able to launch the app and use swagger to navigate the endpoints. There is no authentication
for these examples. A file based document database will be created and seeded on the fly using the following path:

```
C:\Temp\SimpleOrderingSystem.db
```

If you don't like this path, you can change it. 

# Using Postman
Two convenience files for postman have been attached that make using the API even easier. You can import them
into Postman and explore the API once you have it running locally. 

- [Behavioral test postman collection](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/Behavioral%20Tests.postman_collection.json)
- [Behavioral test postman environment](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/blob/main/BehavioralTests.postman_environment.json)



# Refactor examples
See the following pull requests for refactoring examples:

-[More Domain Driven Refactoring Approach](https://github.com/Jose-A-ProfessorFrink/BehavioralTesting/pull/1)