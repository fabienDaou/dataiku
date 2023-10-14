![Build/test](https://github.com/fabienDaou/dataiku/actions/workflows/dotnet.yml/badge.svg)
![Build docker image](https://github.com/fabienDaou/dataiku/actions/workflows/docker-image.yml/badge.svg)

# General informations
## WebApp
At the root of the repository:
```
docker-compose up
```
Then you can access the web app at **http://localhost:38190/**
The open api specification of the api is at ****

If you want to specify a link to a different **millennium-falcon.json** file, you can change the docker compose (check the comments). A volume needs to be mounted with the files necessary.
## CLI
Different builds for different targets are available in the **build** folder.
## Code
- **Webapp** code is in **MilleniumFalconChallenge/MFC.Api** folder.
- **CLI** code is in **MilleniumFalconChallenge/MFC.CmdLine** folder.
They share libraries (these are the other folders).
- **SPA** code is in **MilleniumFalconChallenge/MFC.Api/Client** folder.
- MFC.Actors is code related to actors communicating to get the scenarios processes.
- MFC.Persistence is code related to database and file access.
- MFC.Domain is code related to the overall business logic, it has no dependencies.
## CI
I used github actions, they build and run backend tests on each commit. See in **.github/workflows/dotnet.yml**.

# What I wanted to demonstrate with this test
- Build a solution that meets the minimum setup bar of what I consider a professional project.
    - Code Versioning
    - Test suite
    - CI -> Github actions
    - CD -> dockercompose
    - some basic architecture documentation and motivations behind decisions

# Thought process
From a high level view, this is an application hosting a feature that can, in extreme cases (number of planets for example), require quite some CPU and RAM resources. While a monolithic application is plenty enough for this test, I want to propose an approach that could lead us to "easily" move towards a more distributed architecture. It would have the benefit of increasing the availability and scalability of the application.
A great tool for this is Akka .Net. This is an implementation of the actor model for .Net. It allows developers to structure and split their application from the get-go as an event-driven applications while still keeping the operational simplicity of a monolithic application. Once scalability requirements evolve, it is pretty easy to move these actors in different services and benefit from horizontal scalability.

In our case, the actors actually doing the work are instanciated in the web server but could very well be moved to specific compute instances while not changing much in the code except some akka configuration to enable remoting or clustering depending on the choice made at this point.

# Architecture
Frontend is a SPA developed with Vue3, TypeScript. Split into components to help with reusability.

Backend is a monolithic application. An API is available to create and list scenarios.
Scenarios sent for processing enter the actor model realm. They are queued for processing and picked up when resources are available. One actor is responsible to keep track of the workers available and the scenarios to process (called supervisor later).

Persistence wise, I considered the sqlite database as a readonly resource. I used a different relational database to store information regarding scenarios.

```
Table Scenario (Id:PK, Name:string, Countdown:int, SuccessProbability:double?)

Table BountyHunter (Id:PK, Planet:string, Day:int)

BountyHunter has a ForeignKey constraint on Scenario:Id.
```

**For simplicity**, this is an in-memory relational database, so if you restart the webapp, you will lose your scenarios. Because I used a well known ORM (EntityFramework), it is easy to use a different relational database. Likewise, another type of storage could be used (NoSql (I considered MongoDb to store scenario but found it faster to use the in memory db)).
However, having a different storage for scenarios prevent me from creating relations between the routes and the planets of the bounty hunters which could lead to invalid scenarios being accepted (scenario with an unknown planet). I added validation for this, but in a context of a team or simply the addition of a bug in the validation it could lead to corrupt data (unit tests help here to have some guarantee).

## Best odds algorithm
First thoughts were using some kind of path finding algorithm. I considered Dijkstra that can give use the shortest path and somehow put some weights also on the odds of encoutering bounty hunters. But I could not prove this would give me the best solution everytime.
Therefore I went ahead and considered all possible scenarios when on a planet (staying put, refueling, moving to all nearby planets). Initially, the structure holding the unfinished itineraries was a queue. And it was growing large fast, so I was worried about RAM usage. I noticed there must be quite some overlapping states, so I changed it to a hash set which greatly improved metrics such as: max size of set and number of loops. Check the following:

```
example1
  queue
    max size 514
    loop 1044
  hashset
    max size 18
    loop 38

example2
  queue
    max size 1026
    loop 2134
  hashset
    max size 21
    loop 62

example3
  queue
    max size 2050
    loop 4382
  hashset
    max size 24
    loop 99

example4
  queue
    max size 4098
    loop 9038
  hashset
    max size 27
    loop 150
```

Other possible improvements could be to consider that the graph is shrinking as days pass. All itineraries in planets that are not in the subgraph are not considered anymore.

# Technologies used
## WebServer
- AspNetCore, .Net6
- Akka .Net
- Entity Framework (ORM)
## Front
- Vue3
- VueRouter, for view navigation
- Vuex, for state management
- Vuetify, component library
- TypeScript

# Limitations and thoughts about future development
## Frontend
### Scenarios pagination
Scenarios pagination is available API wise, but I did not add it in the front. I just specified a large page size.
### WebSocket instead of polling
At the moment, I do polling every two seconds to get the latest updates regarding scenarios.
We could use WebSocket later on.
### Frontend composability
I created two views and some components. In the eventuality, there is more development made on this project, it would be interesting thinking about building a design system for this application -> more consistence and reusability.
## Backend
### Error handling
There is a lack of advanced error handling. I focused more on validation of inputs, and their tests, rather than error handling as it is quite time consuming. But in real life application, transient errors can happen (temporary network issue, so you have to handle those).
### Towards a distributed system
**In the current situation, the supervisor is a SPOF (single point of failure).**
In Akka cluster, this is a singleton. The reality is that the migration of this singleton actor from one instance to the other is almost instant (by experience, it is a matter of ms), plus it seems (would have to test) there is some level of buffering of messages that could not be posted so no messages are lost.

To completely remedy the issue, we can think of using a dedicated system (Redis as a queue for example) to keep the queue and several supervisors could pick up work in this queue making it more resilient.
## Multi tenancy
At the moment, there is no authentication or authorization. Every users see the scenarios of all the other users. This leads to a lot of things to consider, especially regarding security and data access:

Passwords management:
- hash them with a salt
- pick a hash  algortihm recommended by OWASP (pbkdf2 for example)
- hash compute to compare passwords should be made constant whether passwords are the same or not (this prevents timing attacks).

CSRF: implement anti forgery tokens

Authorization:
- Work on an authorization model. What are the different features/data we need to protect?
- requests should be validated against what the user is authorized to do and see.