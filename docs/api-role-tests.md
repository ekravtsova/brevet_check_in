# API Role-Based Access Test Scenarios

Base URL used in examples: `http://localhost:5000`

Token placeholders:
- `<admin_access_token>`: JWT for a user in `Admin` role
- `<participant_access_token>`: JWT for a user in `Participant` role

## 1) Register as Participant (Default Role)

- **Endpoint URL:** `POST /api/auth/register`
- **Request headers (with token):**
  - `Content-Type: application/json`
  - `Authorization: N/A` (anonymous endpoint)
- **Request body example:**

```json
{
  "email": "participant1@example.com",
  "password": "Password123!",
  "displayName": "Participant One"
}
```

- **Expected response:** Token response is returned and user is created with default `Participant` role.
- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"participant1@example.com\",\"password\":\"Password123!\",\"displayName\":\"Participant One\"}"
```

---

## 2) Admin Assigns Admin Role to User

- **Endpoint URL:** `POST /api/admin/roles/assign`
- **Request headers (with token):**
  - `Content-Type: application/json`
  - `Authorization: Bearer <admin_access_token>`
- **Request body example:**

```json
{
  "userId": "<target_user_id>",
  "roleName": "Admin"
}
```

- **Expected response:** Role assignment success message.
- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X POST "http://localhost:5000/api/admin/roles/assign" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <admin_access_token>" \
  -d "{\"userId\":\"<target_user_id>\",\"roleName\":\"Admin\"}"
```

---

## 3) Admin Accesses Admin-Only Endpoint (Should Succeed)

- **Endpoint URL:** `GET /api/admin/roles/list`
- **Request headers (with token):**
  - `Authorization: Bearer <admin_access_token>`
- **Request body example:** `N/A` (GET request)
- **Expected response:** List of available roles (for example `["Admin","Participant"]`).
- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/admin/roles/list" \
  -H "Authorization: Bearer <admin_access_token>"
```

---

## 4) Participant Accesses Admin-Only Endpoint (Should Fail)

- **Endpoint URL:** `GET /api/admin/roles/list`
- **Request headers (with token):**
  - `Authorization: Bearer <participant_access_token>`
- **Request body example:** `N/A` (GET request)
- **Expected response:** Access denied/forbidden response.
- **Expected HTTP status code:** `403 Forbidden`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/admin/roles/list" \
  -H "Authorization: Bearer <participant_access_token>"
```

---

## 5) Participant Accesses Participant Endpoint (Should Succeed)

- **Endpoint URL:** `GET /api/brevet`
- **Request headers (with token):**
  - `Authorization: Bearer <participant_access_token>`
- **Request body example:** `N/A` (GET request)
- **Expected response:** Brevet list endpoint response (policy `ParticipantOrAdmin` allows participant).
- **Expected HTTP status code:** `200 OK`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/brevet" \
  -H "Authorization: Bearer <participant_access_token>"
```

---

## 6) Unauthenticated User Accesses Protected Endpoint (Should Fail)

- **Endpoint URL:** `GET /api/brevet`
- **Request headers (with token):**
  - `Authorization: N/A` (missing bearer token)
- **Request body example:** `N/A` (GET request)
- **Expected response:** Unauthorized response because authentication is required.
- **Expected HTTP status code:** `401 Unauthorized`
- **curl example:**

```bash
curl -X GET "http://localhost:5000/api/brevet"
```
