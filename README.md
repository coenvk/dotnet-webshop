# dotnet-webshop

Example project that shows how to implement a microservices architecture in .NET.
The architecture constitutes an API gateway with service registry using Aspire to detect deployed microservices.
Microservices communicate with each other over a Kafka topic.
The saga pattern was implemented to support distributed transactions.

#### Used technologies
- .NET Aspire
- Kafka
- Rebus
- Entity Framework Core
- Ocelot API Gateway
