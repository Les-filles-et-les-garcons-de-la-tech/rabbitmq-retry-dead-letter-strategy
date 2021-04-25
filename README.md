# rabbitmq-retry-dead-letter-strategy



## Construire l'image de RabbitMQ

```
docker build rabbitmq/ -t messagebroker:latest
```

La définition correspondant à l'exemple est chargé via le fichier *definitions.json*.  
Un utilisateur administrateur est défini dans le Dockerfile.  

## Lancer RabbitMQ

```
docker-compose up -d
```

## Connexion à l'interface de RabbitMQ

Se connecter à http://localhost:15672/   
user: rabbitmq  
password: rabbitmq

## Lancer l'exemple de Consumer et Publisher

Dans l'exemple de Consumer, le même bloc de code se trouve dans le try et le catch pour que le message soit systématiquement NACK et donc retry.  
Vous pouvez créer un 2ème Publisher avec *main-queue2* comme routing queue pour tester que la stratégie s'adapte à plusieurs queue de travail.


Dans 2 terminaux différents lancer d'abord le Consumer:
```
dotnet run -p ReceiveLog/ReceiveLog.csproj
```

Puis le Publisher:
```
dotnet run -p EmitLog/EmitLog.csproj
```
