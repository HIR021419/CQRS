# TodoList.Server

Microservice .NET pour gérer une TODO List, implémentant le pattern CQRS et utilisant Entity Framework Core avec une base de données InMemory.

---

## Lancer le projet

### En local (dotnet run)

```bash
cd TodoList.Server
dotnet run
```

L’API sera disponible sur :
[http://localhost:5237/api/todo](http://localhost:5237/api/todo)

### Avec Docker

```bash
docker build -t todolist-server .
docker run -d -p 5000:8080 --name todolist-server todolist-server
```

L’API sera disponible sur :
[http://localhost:5000/api/todo](http://localhost:5000/api/todo)

---

## Documentation Swagger

L’API expose Swagger pour explorer et tester les endpoints
* local : [http://localhost:5237/swagger](http://localhost:5237/swagger)
* docker : [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## Endpoints

### 1. Ajouter une tâche

```
POST /api/todo
```

**Body (JSON)** :

```json
"Acheter du pain"
```

**Réponse** :

* `201 Created` avec l’objet `TodoItem` créé.

---

### 2. Mettre à jour une tâche

```
PUT /api/todo
```

**Body (JSON)** :

```json
{
  "id": "GUID_DE_LA_TACHE",
  "name": "Acheter du pain et du lait",
  "isDone": true
}
```

**Réponse** :

* `200 OK` avec l’objet `TodoItem` mis à jour
* `404 Not Found` si l’ID n’existe pas

---

### 3. Récupérer toutes les tâches

```
GET /api/todo
```

**Réponse** :

* `200 OK` avec un tableau de `TodoItem`

---

### Exemple `TodoItem`

```json
{
  "id": "8a4f9b5c-1c2d-4d34-a3c2-123456789abc",
  "name": "Acheter du pain",
  "isDone": false
}
```

---

## Test rapide avec `curl`

**Ajouter une tâche** :

```bash
curl -X POST http://localhost:5000/api/todo \
     -H "Content-Type: application/json" \
     -d "\"Acheter du pain\""
```

**Récupérer toutes les tâches** :

```bash
curl http://localhost:5000/api/todo
```

**Mettre à jour une tâche** :

```bash
curl -X PUT http://localhost:5000/api/todo \
     -H "Content-Type: application/json" \
     -d '{"id":"GUID_DE_LA_TACHE","name":"Acheter du lait","isDone":true}'
```

---

## Note

* L’API utilise **InMemoryDatabase**, donc les données disparaissent à chaque redémarrage.
