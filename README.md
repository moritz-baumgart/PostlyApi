
# PostlyApi

PostlyApi is a .NET Web API written as a part of a University Project. It serves as the Backend of [PostlyWebpage](https://github.com/moritz-baumgart/PostlyWebpage) and [PostlyApp](https://github.com/moritz-baumgart/PostlyApp). The goal of the project was to create a microblogging plattform.



# Installation

To run the project a file called `secrets.json` has to be placed in inside the PostlyApi directoy with the database password and a JWT-secret with the following format:

```JSON
{
  "dbPassword": "MY_DB_PASSWORD",
  "Jwt": {
    "Secret": "MY_JWT_SECRET"
  }
}
```
## Documentation

Code documentation can be generated using [doxygen](https://www.doxygen.nl/index.html) and the provided Doxyfile. To do so run `doxygen` inside the root directory.
## Authors

- [@Delal1505](https://github.com/Delal1505)
- [@moritz-baumgart](https://github.com/moritz-baumgart)
- [@Fofo1999](https://github.com/Fofo1999)
- [@beyza762](https://github.com/beyza762)
- [@Malin0ne](https://github.com/Malin0ne)
- [@laurenrzv](https://github.com/laurenrzv)
