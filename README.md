# 🧩 System Architecture (Text-Based)

This system follows an **event-driven architecture** using Kafka and MongoDB.

---

## 🔄 End-to-End Flow

1. **Producer (C# Application)**
   - Generates order events
   - Serializes data into JSON
   - Sends messages to Kafka topic

2. **Kafka (Message Broker)**
   - Topic: `orders`
   - Stores and distributes events
   - Ensures decoupling between services

3. **Consumer (C# Service)**
   - Subscribes to Kafka topic (`orders`)
   - Reads messages
   - Deserializes JSON into C# objects
   - Performs processing

4. **Success Path**
   - Stores processed data in MongoDB (`shop.orders` collection)

5. **Failure Path**
   - If processing fails:
     - Retry the operation (configured attempts)
     - If still failing → send message to DLQ topic

6. **Dead Letter Queue (DLQ)**
   - Topic: `orders-dlq`
   - Stores failed events
   - Enables debugging and reprocessing

7. **MongoDB**
   - Collection: `orders`
   - Stores structured, clean documents
   - Optimized for querying and analytics

8. **API Layer (C# Minimal API)**
   - Reads data from MongoDB
   - Exposes REST endpoints
   - Provides analytics and reporting

9. **Clients**
   - Swagger UI
   - Postman
   - Frontend applications

---

## 🔁 Error Handling Flow

- Message fails in consumer
- Retry logic (e.g. 3 attempts)
- If all retries fail → send to `orders-dlq`
- DLQ messages can be:
  - analyzed
  - reprocessed
  - fixed manually

---

## 📦 Data Flow Summary

Producer → Kafka (`orders`) → Consumer  
→ MongoDB (on success)  
→ DLQ (`orders-dlq`) (on failure)  
→ API → Clients  

---

# 🔁 Retry + DLQ Strategy

### ✅ Retry Logic

- Max attempts: 3 (configurable)
- Immediate retry

### ✅ DLQ Usage

- Captures corrupted or failed messages
- Prevents data loss
- Enables safe reprocessing

---

# 🌐 API Endpoints

## Orders

- `GET /orders` → Fetch all orders  
- `GET /orders/{orderId}` → Get specific order  
- `GET /orders/paid` → Get paid orders  

---

## Analytics

- `GET /analytics/revenue` → Total revenue  
- `GET /analytics/orders-per-customer` → Orders grouped by customer  
- `GET /analytics/top-customers` → High-value customers  

---

# 📊 API Access

### Swagger UI

```
http://localhost:5000/swagger
```

### Postman

Use the provided collection to test endpoints.

---

# 🚀 Execution Flow (How System Runs)

1. Start Docker (MongoDB + Kafka)
2. Create Kafka topics:
   - `orders`
   - `orders-dlq`
3. Start Consumer (listens for events)
4. Start Producer (sends events)
5. Start API (query MongoDB)

---

# ✅ Key Benefits of This Architecture

- Decoupled services
- Scalable event processing
- Fault tolerance using DLQ
- Real-time data ingestion
- Flexible schema with MongoDB
- Easy API-based access

---

# 🧠 Important Design Decisions

### Kafka

- Used for asynchronous communication
- Allows scaling consumers independently

### MongoDB

- Stores denormalized documents
- Optimized for read-heavy operations

### DLQ

- Prevents system crashes
- Ensures reliability

---

# ✅ Final System Capabilities

✔ Real-time event ingestion  
✔ Fault-tolerant processing  
✔ MongoDB persistence  
✔ REST API for analytics  
✔ Swagger documentation  
✔ DLQ-enabled error handling  

# 🔎 Pagination & Filtering

```
GET /orders/search?customer=Manoj&status=PAID&page=1&pageSize=10
```

Supports:

- customer filter
- status filter
- pagination

---

# 📦 Sample Responses

(Include JSON examples above)

---

# 🛡️ Enterprise Improvements

- DTO layer for response safety
- Logging for observability
- Retry with backoff
- DLQ for fault tolerance
- Indexing for performance
- Health check endpoint

# 🔎 Pagination & Filtering

```
GET /orders/search?customer=Manoj&status=PAID&page=1&pageSize=10
```

Supports:

- customer filter
- status filter
- pagination

---

# 📦 Sample Responses

(Include JSON examples above)

---

# 🛡️ Enterprise Improvements

- DTO layer for response safety
- Logging for observability
- Retry with backoff
- DLQ for fault tolerance
- Indexing for performance
- Health check endpoint

# 🔁 Dead Letter Queue (DLQ)

## ✅ What is DLQ?

A **Dead Letter Queue (DLQ)** is a Kafka topic used to store messages that **failed processing** after multiple retries.

---

## ✅ Why DLQ is Needed

In real-world systems:

- Messages may fail due to:
  - Invalid JSON
  - Database errors
  - Unexpected exceptions

Without DLQ:

- ❌ System crashes
- ❌ Data loss occurs

With DLQ:

- ✅ Failures are isolated
- ✅ Data is preserved
- ✅ Debugging becomes easier

---

## ✅ DLQ Implementation in This Project

- **Primary Topic:** `orders`
- **DLQ Topic:** `orders-dlq`

---

## 🔄 Processing Flow

1. Consumer reads message from `orders`
2. Attempts processing
3. If failure:
   - Retry (3 attempts)
4. If still failing:
   - Send message to `orders-dlq`

---

## ✅ Example Failed Message

```json
{
  "orderId": "ORD-999",
  "error": "Invalid data format",
  "originalPayload": "{bad-json}"
}
```

---

## ✅ DLQ Benefits

- Prevents system crashes
- Ensures no data loss
- Allows manual reprocessing
- Improves reliability

---

## ✅ Future Enhancements

- DLQ reprocessing service
- Alerting on DLQ events
- Retry queue (separate topic)

# 🔁 Retry Strategy

## ✅ Approach Used

This project uses a **retry-first, DLQ-last** approach.

---

## ✅ Retry Logic

- Max retries: **3 attempts**
- Retry type: **synchronous retry**
- Delay: can be extended to exponential backoff

---

## ✅ Retry Flow

```
Message → Processing
     ↓
  Failure
     ↓
 Retry (up to 3 times)
     ↓
 Failure persists
     ↓
 Send to DLQ
```

---

## ✅ Example Enhancement (Future)

- Exponential backoff (1s → 2s → 4s)
- Retry topic instead of inline retry
- Circuit breaker pattern

# 📄 API Design (Pagination & Filtering)

## ✅ Why Pagination?

- Avoid large responses
- Improve performance
- Reduce API latency

---

## ✅ Endpoint

```
GET /orders/search
```

---

## ✅ Query Parameters

| Parameter | Description |
|----------|------------|
| customer | Filter by customer |
| status | Filter by order status |
| page | Page number |
| pageSize | Number of records |

---

## ✅ Example Request

```
GET /orders/search?customer=Manoj&status=PAID&page=1&pageSize=5
```

---

## ✅ Sample Response

```json
{
  "page": 1,
  "pageSize": 5,
  "totalCount": 25,
  "data": [
    {
      "orderId": "ORD-101",
      "customer": "Manoj",
      "amount": 1200,
      "status": "PAID"
    }
  ]
}
```

# 📦 API Response Format

All API responses follow a consistent structure:

```json
{
  "success": true,
  "data": {},
  "error": null
}
```

---

## ✅ Success Example

```json
{
  "success": true,
  "data": {
    "orderId": "ORD-101"
  },
  "error": null
}
```

---

## ✅ Error Example

```json
{
  "success": false,
  "data": null,
  "error": "Order not found"
}
```

# 🧠 Design Decisions

## ✅ Kafka

- Used for asynchronous communication
- Decouples Producer and Consumer
- Enables scalability

---

## ✅ MongoDB

- Used for flexible schema storage
- Supports fast reads and aggregations
- Stores denormalized documents

---

## ✅ DLQ

- Separates failed messages
- Prevents system failure
- Enables reprocessing strategy

---

## ✅ API Layer

- Provides abstraction over database
- Enables analytics & querying
- Supports external clients

# ⚡ Performance Considerations

## ✅ MongoDB

- Indexes on:
  - orderId
  - customer
  - status

---

## ✅ Kafka

- Message key ensures ordering per order
- Partitioning supports scaling

---

## ✅ API

- Pagination prevents large data loads
- Filtering reduces query size

``# 🛡️ Enterprise Considerations

## ✅ Security

- Authentication (future: JWT)
- Input validation

---

## ✅ Reliability

- Retry mechanism
- DLQ fallback

---

## ✅ Observability

- Logging (future: structured logging)
- Monitoring (future enhancement)

---

## ✅ Scalability

- Kafka supports horizontal scaling
- Consumers can be replicated

# 🛡️ Enterprise Considerations

## ✅ Security

- Authentication (future: JWT)
- Input validation

---

## ✅ Reliability

- Retry mechanism
- DLQ fallback

---

## ✅ Observability

- Logging (future: structured logging)
- Monitoring (future enhancement)

---

## ✅ Scalability

- Kafka supports horizontal scaling
- Consumers can be replicated
