# Pokedex



### Technology/Tools Used
    1. .Net Core 5.0 (C# language)
    2. Rider IDE
    3. XUnit Testing Framework, FluentAssertions, Moq (mocking framework)
    4. Docker

### How To RUN Locally
#### Required Setup
    Jetbrains Rider
    Docker
    .Net Core 5.0

#### Run locally
    Clone the repo and use any of the following options.

    1. Command promt/console
        a. open console
        b. navigate to pokedex (using cd command)
        c. execute dotnet run command

    2. Run using Rider
        a. Open the solution file in Rider and hit F5

    3. Using Docker
        a. open cosole and execute below commands
        b. docker build -t pokedex:v1 .
        c. docker run -dp 5000:80 --name pokedex -t pokedex:v1

    Visit the following url in any browser
    http://localhost:5000/swagger

#### To run tests
    1. Open console
    2. cd pokedex.tests
    3. dotnet test

### Assumptions / Considerations
1. For the sake of simplecity I have used in memory distributed cache, however in real production case, I would use requirements to use a better choice.
2. I have used mediator pattern to handle the request.
3. The PokemonDetailsHandler class contains the handling logic of both the endpoints.
4. The documentation of transalation states that it would accept only 5 requests per hour, this means after 5 requests all subsequent request would fail and translation would stop working.
5. The pokemon api asks to use cache, however it does not talk about any expiration policy for the cache. Hence to keep it simple, I have made an assumpetion that the cache would never expire. In real production case this would never be the case.
6. For production systems, I would configure observability tools like Graphna, or Azure App Insights.
7. Since this application primarily depends on external source for its data, it is necessary to implement dependency health checks & circuit breaker. I did not find any documentation about the health checks of poke api and fun translator api. Also, instead of circuit breaker I have implemented a retry policy using polly.

