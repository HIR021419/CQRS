# Microservices TODO List

Solution composée de deux microservices en .NET 8 implémentant le pattern CQRS avec Entity Framework Core (InMemory) et communiquant via RabbitMQ.

* **UserService** : gère les utilisateurs et publie des événements (`user.add`, `user.update`) sur RabbitMQ.
* **TodoService** : gère les TODO items et consomme les événements pour enrichir les tâches avec les informations utilisateur.
* **RabbitMQ** : bus de messages utilisé pour la communication inter-services.

---

## 🚀 Lancer le projet

### Avec Docker Compose

```bash
docker-compose up --build
```

* UserService → [http://localhost:5001/swagger](http://localhost:5001/swagger)
* TodoService → [http://localhost:5002/swagger](http://localhost:5002/swagger)
* RabbitMQ Management UI → [http://localhost:15672](http://localhost:15672) (login : `guest`, mot de passe : `guest`)

---

## 📑 Endpoints

### 🔹 UserService

#### 1. Ajouter un utilisateur

```
POST /api/user
```

**Body (JSON)** :

```json
{
  "name": "Alice",
  "mail": "alice@example.com"
}
```

#### 2. Mettre à jour un utilisateur

```
PUT /api/user/{id}
```

**Body (JSON)** :

```json
{
  "id": "GUID_UTILISATEUR",
  "name": "Alice Updated",
  "mail": "alice.new@example.com"
}
```

#### 3. Récupérer tous les utilisateurs

```
GET /api/user
```

---

### 🔹 TodoService

#### 1. Ajouter une tâche

```
POST /api/todo
```

**Body (JSON)** :

```json
{
  "name": "Acheter du pain",
  "userId": "GUID_UTILISATEUR"
}
```

#### 2. Récupérer toutes les tâches

```
GET /api/todo
```

**Réponse (JSON)** :

```json
[
  {
    "id": "GUID_TODO",
    "name": "Acheter du pain",
    "isDone": false,
    "user": {
      "id": "GUID_UTILISATEUR",
      "name": "Alice"
    }
  }
]
```

---

## ⚡ Test rapide avec `curl`

**Ajouter un utilisateur** :

```bash
curl -X POST http://localhost:5001/api/user \
     -H "Content-Type: application/json" \
     -d '{"name":"Alice","mail":"alice@example.com"}'
```

**Ajouter une tâche liée à l’utilisateur** :

```bash
curl -X POST http://localhost:5002/api/todo \
     -H "Content-Type: application/json" \
     -d '{"name":"Acheter du pain","userId":"GUID_UTILISATEUR"}'
```

**Lister les tâches** :

```bash
curl http://localhost:5002/api/todo
```

---

## Note

* Les deux services utilisent **InMemoryDatabase**, donc les données disparaissent à chaque redémarrage.
* RabbitMQ est requis pour la communication inter-services (via `docker-compose`).
