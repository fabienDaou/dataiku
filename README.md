![Build/test](https://github.com/github/docs/actions/workflows/dotnet.yml/badge.svg)

# What I wanted to demonstrate with this test
- Build a solution that meets the minimum setup bar of what I consider a professional project.
    - Code Versioning
    - Test suite
    - CI -> Github actions
    - CD -> dockercompose
    - some basic architecture documentation and motivations behind decisions

# Thought process
From a high level view, this is an application hosting a feature that can, in extreme cases (number of planets for example), require quite some CPU resources. While a monolithic application is plenty enough for this test, I want to propose an approach that could lead us to "easily" move towards a more distributed architecture. It would have the benefit of increasing the availability and scalability of the application.
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

**For simplicity**, this is an in-memory relational database, so if you restart the webapp, you will lose your scenarios. Because I used a well known ORM (EntityFramework), it is easy to use a different relational database. However, this prevents me from creating relations between the routes and the planets of the bounty hunters which could lead to invalid scenarios being accepted (scenario with an unknown planet). I added validation for this, but in a context of a team or simply the addition of a bug in the validation it could lead to corrupt data (unit tests help here have some guarantee).

# Technologies used
## WebServer
AspNetCore, .Net6
Akka .Net
Entity Framework (ORM)
## Front
Vue3, TypeScript

## Backend architecture
This is a monolithic application.
Details about the scenarios are stored in an in memory database.
Because I used EntityFramework, it would be easy to configure another relational database.
In the same spirit, database access is done in classes implementing interfaces. In case, we need to evolve to another kind of persistence (NoSQL database, MongoDb for example). Implementation would be pretty trivial.

# Some thoughts about future development
## Towards a distributed system
**In the current situation, the supervisor is a SPOF (single point of failure).**
In Akka cluster, this is a singleton. The reality is that the migration of this singleton actor from one instance to the other is almost instant (by experience, it is a matter of ms), plus it seems (would have to test) there is some level of buffering of messages that could not be posted so no message are lost.

To completely remedy the issue, we can think of using a dedicated system (Redis as a queue for example) to keep the queue and several supervisors could pick up work in this queue making it more resilient.

## Multi tenancy
At the moment, there is no authentication or authorization. Every users see the scenarios of all the other users. This leads to a lot of things to consider, especially regarding security.

Passwords:
- hash them with a salt
- pick a hash recommended by OWASP (pbkdf2 for example)
- hash compute to compare passwords should be made constant whether passwords are the same or not (this prevents timing attacks).

CSRF: implement anti forgery tokens

Authorization:
- Work on an authorization model. What are the different features/data we need to protect?
- requests should be validated against what the user is authorized to do and see.