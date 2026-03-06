# API Authentication Test Scenarios

Base URL used in examples: `http://localhost:5000`

## 1) Successful Login

- **Endpoint URL:** `POST /api/auth/login`
- **Request body example:**

```json
{
  "email": "user1@example.com",
  "password": "Password123!"
}
```

- **Expected response (example):**

```json
{
  "accessToken": "<jwt_access_token>",
  "refreshToken": "<refresh_token>",
  "expiresAt": "2026-03-06T10:30:00Z",
  "tokenType": "Bearer"
}
```

- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"user1@example.com\",\"password\":\"Password123!\"}"
```

---

## 2) Failed Login (Wrong Password)

- **Endpoint URL:** `POST /api/auth/login`
- **Request body example:**

```json
{
  "email": "user1@example.com",
  "password": "WrongPassword!"
}
```

- **Expected response (example):**

```json
"Invalid credentials."
```

- **Expected HTTP status code:** `401 Unauthorized`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"user1@example.com\",\"password\":\"WrongPassword!\"}"
```

---

## 3) Registration

- **Endpoint URL:** `POST /api/auth/register`
- **Request body example:**

```json
{
  "email": "newuser@example.com",
  "password": "Password123!",
  "displayName": "New Rider"
}
```

- **Expected response (example):**

```json
{
  "accessToken": "<jwt_access_token>",
  "refreshToken": "<refresh_token>",
  "expiresAt": "2026-03-06T10:35:00Z",
  "tokenType": "Bearer"
}
```

- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"newuser@example.com\",\"password\":\"Password123!\",\"displayName\":\"New Rider\"}"
```

---

## 4) Token Refresh

- **Endpoint URL:** `POST /api/auth/refresh`
- **Request body example:**

```json
{
  "accessToken": "<expired_or_expiring_access_token>",
  "refreshToken": "<valid_refresh_token>"
}
```

- **Expected response (example):**

```json
{
  "accessToken": "<new_jwt_access_token>",
  "refreshToken": "<new_refresh_token>",
  "expiresAt": "2026-03-06T10:50:00Z",
  "tokenType": "Bearer"
}
```

- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/auth/refresh" \
  -H "Content-Type: application/json" \
  -d "{\"accessToken\":\"<expired_or_expiring_access_token>\",\"refreshToken\":\"<valid_refresh_token>\"}"
```

---

## 5) Access Protected Endpoint with Valid Token

- **Endpoint URL:** `GET /api/brevet/sample`
- **Request body example:** `N/A` (GET request)
- **Expected response (example):**

```json
{
  "message": "Brevet endpoint is protected."
}
```

- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/brevet/sample" \
  -H "Authorization: Bearer <valid_access_token>"
```

---

## 6) Access Protected Endpoint with Expired Token

- **Endpoint URL:** `GET /api/brevet/sample`
- **Request body example:** `N/A` (GET request)
- **Expected response (example):**

```json
{
  "status": 401
}
```

- **Expected HTTP status code:** `401 Unauthorized`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/brevet/sample" \
  -H "Authorization: Bearer <expired_access_token>"
```

---

## Optional Quick Health Check (Anonymous)

To confirm anonymous endpoint behavior:

```bash
curl -X GET "http://localhost:5000/api/health/live"
```

Expected status: `200 OK`
