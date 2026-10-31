# Evidencias de pruebas — SGCAE

- **Fecha de ejecución:** 2026-10-30 20:14:42
- **API:** `http://localhost:5125`
- **Generado con:** `dotnet run scripts/evidencias.cs`

**Resultado: 43 de 43 pasos coinciden con lo esperado.**

Cada paso muestra la petición enviada, la respuesta real de la API y si coincide con el resultado esperado.

## RF-01 — Gestión de empleados

### 01. Registrar empleado — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 114 ms

```http
POST /api/employees
Content-Type: application/json

{
  "identification": "1020144201",
  "firstName": "Laura",
  "lastName": "Gómez",
  "email": "laura.gomez.201442@empresa.com",
  "position": "Auxiliar de bodega",
  "area": "Operaciones"
}
```

Respuesta:

```json
{
  "id": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e"
}
```

### 02. Registrar segundo empleado — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 7 ms

```http
POST /api/employees
Content-Type: application/json

{
  "identification": "2020144202",
  "firstName": "Pedro",
  "lastName": "Ruiz",
  "email": "pedro.ruiz.201442@empresa.com",
  "position": "Supervisor",
  "area": "Logística"
}
```

Respuesta:

```json
{
  "id": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9"
}
```

### 03. Rechazar identificación duplicada — OK

**Esperado:** `409` · **Obtenido:** `409 Conflict` · 3 ms

```http
POST /api/employees
Content-Type: application/json

{
  "identification": "1020144201",
  "firstName": "Otra",
  "lastName": "Persona",
  "email": "otra.201442@empresa.com",
  "position": "X",
  "area": "Y"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflicto con datos existentes",
  "status": 409,
  "detail": "Ya existe un empleado con identificación 1020144201.",
  "traceId": "00-70790683f765ec98433cec2834abcf09-13f66784b44af84b-00"
}
```

### 04. Rechazar correo con formato inválido — OK

**Esperado:** `400` · **Obtenido:** `400 Bad Request` · 1 ms

```http
POST /api/employees
Content-Type: application/json

{
  "identification": "3020144203",
  "firstName": "Ana",
  "lastName": "Díaz",
  "email": "correo-malo",
  "position": "X",
  "area": "Y"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Regla de negocio no cumplida",
  "status": 400,
  "detail": "El correo 'correo-malo' no tiene un formato válido.",
  "traceId": "00-0e63cc4a97085243d147b618e34055b8-681ab756c4bf0bb2-00"
}
```

### 05. Consultar empleado por id — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 5 ms

```http
GET /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e
```

Respuesta:

```json
{
  "id": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
  "identification": "1020144201",
  "firstName": "Laura",
  "lastName": "Gómez",
  "email": "laura.gomez.201442@empresa.com",
  "position": "Auxiliar de bodega",
  "area": "Operaciones",
  "status": "Active"
}
```

### 06. Actualizar empleado — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 6 ms

```http
PUT /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e
Content-Type: application/json

{
  "firstName": "Laura",
  "lastName": "Gómez Pérez",
  "email": "laura.gomez.201442@empresa.com",
  "position": "Coordinadora de bodega",
  "area": "Operaciones"
}
```

Respuesta:

```json
(sin contenido)
```

### 07. Listar empleados activos (paginado) — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 4 ms

```http
GET /api/employees?status=Active&page=1&pageSize=5
```

Respuesta:

```json
{
  "items": [
    {
      "id": "ee53bba6-c3b6-4ba9-96df-f89ab21938b9",
      "identification": "1032456789",
      "firstName": "Laura",
      "lastName": "Gómez",
      "email": "laura.gomez@empresa.com",
      "position": "Auxiliar de bodega",
      "area": "Operaciones",
      "status": "Active"
    },
    {
      "id": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "identification": "1020144201",
      "firstName": "Laura",
      "lastName": "Gómez Pérez",
      "email": "laura.gomez.201442@empresa.com",
      "position": "Coordinadora de bodega",
      "area": "Operaciones",
      "status": "Active"
    },
    {
      "id": "35e01b75-69f7-4546-860b-ae164016c7ea",
      "identification": "99887766",
      "firstName": "Pedro",
      "lastName": "Ruiz",
      "email": "pedro.ruiz@empresa.com",
      "position": "Supervisor",
      "area": "Logística",
      "status": "Active"
    },
    {
      "id": "ad331ecb-ef70-4fe5-ab09-c7a9a40fa6ba",
      "identification": "2020084502",
      "firstName": "Pedro",
      "lastName": "Ruiz",
      "email": "pedro.ruiz.200845@empresa.com",
      "position": "Supervisor",
      "area": "Logística",
      "status": "Active"
    },
    {
      "id": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "identification": "2020144202",
      "firstName": "Pedro",
      "lastName": "Ruiz",
      "email": "pedro.ruiz.201442@empresa.com",
      "position": "Supervisor",
      "area": "Logística",
      "status": "Active"
    }
  ],
  "page": 1,
  "pageSize": 5,
  "totalCount": 5,
  "totalPages": 1
}
```

## RF-06 — Administración de puntos de acceso

### 08. Registrar punto: bodega — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 6 ms

```http
POST /api/access-points
Content-Type: application/json

{
  "code": "BOD-201442",
  "name": "Bodega principal",
  "type": "Warehouse"
}
```

Respuesta:

```json
{
  "id": "8a4df789-a79a-4412-8b25-924ebde43abd"
}
```

### 09. Registrar punto: recepción — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 4 ms

```http
POST /api/access-points
Content-Type: application/json

{
  "code": "REC-201442",
  "name": "Recepción",
  "type": "Reception"
}
```

Respuesta:

```json
{
  "id": "8e7ab5af-6435-4737-80cf-8dc873b4b8fc"
}
```

### 10. Registrar punto: parqueadero — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 3 ms

```http
POST /api/access-points
Content-Type: application/json

{
  "code": "PAR-201442",
  "name": "Parqueadero",
  "type": "Parking"
}
```

Respuesta:

```json
{
  "id": "4d4d7a92-9acc-4adb-ac04-a921efb98867"
}
```

### 11. Registrar punto: área restringida — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 3 ms

```http
POST /api/access-points
Content-Type: application/json

{
  "code": "RES-201442",
  "name": "Cuarto de servidores",
  "type": "RestrictedArea"
}
```

Respuesta:

```json
{
  "id": "0c29f5f8-8630-4e16-b4c7-91f0ed6a5601"
}
```

### 12. Rechazar código de punto duplicado — OK

**Esperado:** `409` · **Obtenido:** `409 Conflict` · 2 ms

```http
POST /api/access-points
Content-Type: application/json

{
  "code": "BOD-201442",
  "name": "Otra bodega",
  "type": "Warehouse"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflicto con datos existentes",
  "status": 409,
  "detail": "Ya existe un punto de acceso con el código BOD-201442.",
  "traceId": "00-465e7df9921b04d241c93dfa330fb52d-6bfeedf75ef96bb8-00"
}
```

### 13. Deshabilitar área restringida — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 4 ms

```http
PATCH /api/access-points/0c29f5f8-8630-4e16-b4c7-91f0ed6a5601/disable
```

Respuesta:

```json
(sin contenido)
```

### 14. Consultar punto deshabilitado — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 2 ms

```http
GET /api/access-points/0c29f5f8-8630-4e16-b4c7-91f0ed6a5601
```

Respuesta:

```json
{
  "id": "0c29f5f8-8630-4e16-b4c7-91f0ed6a5601",
  "code": "RES-201442",
  "name": "Cuarto de servidores",
  "type": "RestrictedArea",
  "isEnabled": false
}
```

### 15. Listar solo puntos habilitados — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 3 ms

```http
GET /api/access-points?isEnabled=true
```

Respuesta:

```json
[
  {
    "id": "695e1614-fd25-4e24-94ba-29abb6c4276b",
    "code": "BOD-01",
    "name": "Bodega principal",
    "type": "Warehouse",
    "isEnabled": true
  },
  {
    "id": "c07b6d79-bac5-4bc3-af79-b3a142ae7a15",
    "code": "BOD-200845",
    "name": "Bodega principal",
    "type": "Warehouse",
    "isEnabled": true
  },
  {
    "id": "8a4df789-a79a-4412-8b25-924ebde43abd",
    "code": "BOD-201442",
    "name": "Bodega principal",
    "type": "Warehouse",
    "isEnabled": true
  },
  {
    "id": "2937856e-cda7-4d6e-8150-90e9d05933ae",
    "code": "PAR-01",
    "name": "Parqueadero",
    "type": "Parking",
    "isEnabled": true
  },
  {
    "id": "872f3f7b-d3cc-4791-9a4a-f39cc9652520",
    "code": "PAR-200845",
    "name": "Parqueadero",
    "type": "Parking",
    "isEnabled": true
  },
  {
    "id": "4d4d7a92-9acc-4adb-ac04-a921efb98867",
    "code": "PAR-201442",
    "name": "Parqueadero",
    "type": "Parking",
    "isEnabled": true
  },
  {
    "id": "47daff52-f5a5-4c1a-b03d-d77db061f60b",
    "code": "REC-01",
    "name": "Recepción",
    "type": "Reception",
    "isEnabled": true
  },
  {
    "id": "50f66aa9-e261-4819-be8d-4f7d54b0f74b",
    "code": "REC-200845",
    "name": "Recepción",
    "type": "Reception",
    "isEnabled": true
  },
  {
    "id": "8e7ab5af-6435-4737-80cf-8dc873b4b8fc",
    "code": "REC-201442",
    "name": "Recepción",
    "type": "Reception",
    "isEnabled": true
  },
  {
    "id": "ec790f47-e11d-4378-82d7-21244c63e289",
    "code": "RES-01",
    "name": "Cuarto de servidores",
    "type": "RestrictedArea",
    "isEnabled": true
  }
]
```

## RF-02 — Gestión de credenciales

### 16. Asignar credencial a Laura — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 7 ms

```http
POST /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e/credentials
Content-Type: application/json

{
  "code": "CRD-L201442"
}
```

Respuesta:

```json
{
  "id": "b87596a5-36c7-4604-86ee-58ff5c889931"
}
```

### 17. Asignar credencial a Pedro — OK

**Esperado:** `201` · **Obtenido:** `201 Created` · 5 ms

```http
POST /api/employees/0372b59b-d19a-4281-a89d-09b7cd1cb5f9/credentials
Content-Type: application/json

{
  "code": "CRD-P201442"
}
```

Respuesta:

```json
{
  "id": "775407bd-f825-431e-9a32-644f04850578"
}
```

### 18. Rechazar segunda credencial activa — OK

**Esperado:** `409` · **Obtenido:** `409 Conflict` · 4 ms

```http
POST /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e/credentials
Content-Type: application/json

{
  "code": "CRD-X201442"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflicto con datos existentes",
  "status": 409,
  "detail": "El empleado ya tiene una credencial activa. Bloquéela o revóquela primero.",
  "traceId": "00-c5f2c1843952965a2f5dcf36283f54b9-c1723530a964980c-00"
}
```

### 19. Rechazar código de credencial duplicado — OK

**Esperado:** `409` · **Obtenido:** `409 Conflict` · 3 ms

```http
POST /api/employees/0372b59b-d19a-4281-a89d-09b7cd1cb5f9/credentials
Content-Type: application/json

{
  "code": "CRD-L201442"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflicto con datos existentes",
  "status": 409,
  "detail": "Ya existe una credencial con el código CRD-L201442.",
  "traceId": "00-123ceb78642748f5d039b7e3d230b661-987eac9b9571dfa6-00"
}
```

### 20. Consultar estado de credencial — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 2 ms

```http
GET /api/credentials/b87596a5-36c7-4604-86ee-58ff5c889931
```

Respuesta:

```json
{
  "id": "b87596a5-36c7-4604-86ee-58ff5c889931",
  "code": "CRD-L201442",
  "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
  "status": "Active",
  "issuedAtUtc": "2026-10-31T01:14:42.4868925",
  "statusChangedAtUtc": null
}
```

### 21. Listar credenciales del empleado — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 2 ms

```http
GET /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e/credentials
```

Respuesta:

```json
[
  {
    "id": "b87596a5-36c7-4604-86ee-58ff5c889931",
    "code": "CRD-L201442",
    "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
    "status": "Active",
    "issuedAtUtc": "2026-10-31T01:14:42.4868925",
    "statusChangedAtUtc": null
  }
]
```

## RF-03 — Registro de entradas y salidas

### 22. Acceso: Entrada autorizada — OK

**Esperado:** `200` — `Authorized` · **Obtenido:** `200 OK` · 7 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "aa104f23-ef04-496b-9e86-a6ba083a388f",
  "result": "Authorized",
  "rejectionReason": null,
  "occurredAtUtc": "2026-10-31T01:14:42.5169508Z"
}
```

### 23. Acceso: Segunda entrada sin salida — OK

**Esperado:** `200` — `Rejected / InvalidMovementSequence` · **Obtenido:** `200 OK` · 7 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "5e93abf2-9adf-4724-92a2-0fcee499382d",
  "result": "Rejected",
  "rejectionReason": "InvalidMovementSequence",
  "occurredAtUtc": "2026-10-31T01:14:42.5250675Z"
}
```

### 24. Acceso: Salida autorizada — OK

**Esperado:** `200` — `Authorized` · **Obtenido:** `200 OK` · 5 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Exit"
}
```

Respuesta:

```json
{
  "recordId": "84bd2ce4-ab1e-44db-bbe0-aeb13f37e7f6",
  "result": "Authorized",
  "rejectionReason": null,
  "occurredAtUtc": "2026-10-31T01:14:42.5310008Z"
}
```

### 25. Acceso: Salida sin entrada previa — OK

**Esperado:** `200` — `Rejected / InvalidMovementSequence` · **Obtenido:** `200 OK` · 6 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Exit"
}
```

Respuesta:

```json
{
  "recordId": "7a3a3bd8-3ab6-4dc8-85d8-78223bf9bd67",
  "result": "Rejected",
  "rejectionReason": "InvalidMovementSequence",
  "occurredAtUtc": "2026-10-31T01:14:42.5378008Z"
}
```

### 26. Acceso: Credencial inexistente — OK

**Esperado:** `200` — `Rejected / CredentialNotFound` · **Obtenido:** `200 OK` · 5 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "NOEXISTE201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "fa99169e-179b-4168-a411-d674829ee032",
  "result": "Rejected",
  "rejectionReason": "CredentialNotFound",
  "occurredAtUtc": "2026-10-31T01:14:42.543347Z"
}
```

### 27. Acceso: Punto de acceso inexistente — OK

**Esperado:** `200` — `Rejected / AccessPointNotFound` · **Obtenido:** `200 OK` · 7 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "XXX-99",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "b7eac177-1497-41d8-865e-1a615895b3c7",
  "result": "Rejected",
  "rejectionReason": "AccessPointNotFound",
  "occurredAtUtc": "2026-10-31T01:14:42.5512118Z"
}
```

### 28. Acceso: Punto de acceso deshabilitado — OK

**Esperado:** `200` — `Rejected / AccessPointDisabled` · **Obtenido:** `200 OK` · 6 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "RES-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "2543f5f9-893f-419e-9194-33af3c4d1447",
  "result": "Rejected",
  "rejectionReason": "AccessPointDisabled",
  "occurredAtUtc": "2026-10-31T01:14:42.5601674Z"
}
```

### 29. Bloquear credencial de Pedro — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 4 ms

```http
POST /api/credentials/775407bd-f825-431e-9a32-644f04850578/block
```

Respuesta:

```json
(sin contenido)
```

### 30. Acceso: Credencial bloqueada — OK

**Esperado:** `200` — `Rejected / CredentialBlocked` · **Obtenido:** `200 OK` · 8 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-P201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "a1baff65-4a26-40f3-840a-67e75a156ef5",
  "result": "Rejected",
  "rejectionReason": "CredentialBlocked",
  "occurredAtUtc": "2026-10-31T01:14:42.5734781Z"
}
```

### 31. Desbloquear credencial de Pedro — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 5 ms

```http
POST /api/credentials/775407bd-f825-431e-9a32-644f04850578/unblock
```

Respuesta:

```json
(sin contenido)
```

### 32. Acceso: Credencial desbloqueada vuelve a funcionar — OK

**Esperado:** `200` — `Authorized` · **Obtenido:** `200 OK` · 11 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-P201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "67862326-0543-42eb-a65b-62d9702718ca",
  "result": "Authorized",
  "rejectionReason": null,
  "occurredAtUtc": "2026-10-31T01:14:42.592288Z"
}
```

### 33. Revocar credencial de Pedro — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 4 ms

```http
POST /api/credentials/775407bd-f825-431e-9a32-644f04850578/revoke
```

Respuesta:

```json
(sin contenido)
```

### 34. Acceso: Credencial revocada — OK

**Esperado:** `200` — `Rejected / CredentialRevoked` · **Obtenido:** `200 OK` · 7 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-P201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Exit"
}
```

Respuesta:

```json
{
  "recordId": "94fc00f3-26a3-4c10-87a3-d4fb3a8114a0",
  "result": "Rejected",
  "rejectionReason": "CredentialRevoked",
  "occurredAtUtc": "2026-10-31T01:14:42.6049883Z"
}
```

### 35. Rechazar bloqueo de credencial revocada — OK

**Esperado:** `400` · **Obtenido:** `400 Bad Request` · 2 ms

```http
POST /api/credentials/775407bd-f825-431e-9a32-644f04850578/block
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Regla de negocio no cumplida",
  "status": 400,
  "detail": "Una credencial revocada no puede bloquearse.",
  "traceId": "00-780da4dba6035cae406c6227f0d20336-5a67020bf03d70bc-00"
}
```

### 36. Desactivar empleado — OK

**Esperado:** `204` · **Obtenido:** `204 No Content` · 4 ms

```http
PATCH /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e/deactivate
```

Respuesta:

```json
(sin contenido)
```

### 37. Acceso: Empleado inactivo — OK

**Esperado:** `200` — `Rejected / EmployeeInactive` · **Obtenido:** `200 OK` · 6 ms

```http
POST /api/access/requests
Content-Type: application/json

{
  "credentialCode": "CRD-L201442",
  "accessPointCode": "BOD-201442",
  "movementType": "Entry"
}
```

Respuesta:

```json
{
  "recordId": "1becb63f-a7e7-40a9-87e5-539b8b3c0354",
  "result": "Rejected",
  "rejectionReason": "EmployeeInactive",
  "occurredAtUtc": "2026-10-31T01:14:42.622553Z"
}
```

### 38. Rechazar actualización de empleado inactivo — OK

**Esperado:** `400` · **Obtenido:** `400 Bad Request` · 3 ms

```http
PUT /api/employees/0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e
Content-Type: application/json

{
  "firstName": "Laura",
  "lastName": "Gómez",
  "email": "laura.gomez.201442@empresa.com",
  "position": "X",
  "area": "Y"
}
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Regla de negocio no cumplida",
  "status": 400,
  "detail": "No se puede actualizar un empleado inactivo.",
  "traceId": "00-dc27decb7a40cb7cd921b7c30a1e8530-fd788e6104410a11-00"
}
```

## RF-04 — Consulta del historial de accesos

### 39. Historial por empleado (página 1 de 3 registros) — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 4 ms

```http
GET /api/access/records?employeeId=0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e&page=1&pageSize=3
```

Respuesta:

```json
{
  "items": [
    {
      "id": "1becb63f-a7e7-40a9-87e5-539b8b3c0354",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "EmployeeInactive",
      "occurredAtUtc": "2026-10-31T01:14:42.622553"
    },
    {
      "id": "2543f5f9-893f-419e-9194-33af3c4d1447",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "RES-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "0c29f5f8-8630-4e16-b4c7-91f0ed6a5601",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "AccessPointDisabled",
      "occurredAtUtc": "2026-10-31T01:14:42.5601674"
    },
    {
      "id": "b7eac177-1497-41d8-865e-1a615895b3c7",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "XXX-99",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": null,
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "AccessPointNotFound",
      "occurredAtUtc": "2026-10-31T01:14:42.5512118"
    }
  ],
  "page": 1,
  "pageSize": 3,
  "totalCount": 7,
  "totalPages": 3
}
```

### 40. Historial por empleado (página 2) — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 4 ms

```http
GET /api/access/records?employeeId=0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e&page=2&pageSize=3
```

Respuesta:

```json
{
  "items": [
    {
      "id": "7a3a3bd8-3ab6-4dc8-85d8-78223bf9bd67",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5378008"
    },
    {
      "id": "84bd2ce4-ab1e-44db-bbe0-aeb13f37e7f6",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Authorized",
      "rejectionReason": null,
      "occurredAtUtc": "2026-10-31T01:14:42.5310008"
    },
    {
      "id": "5e93abf2-9adf-4724-92a2-0fcee499382d",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5250675"
    }
  ],
  "page": 2,
  "pageSize": 3,
  "totalCount": 7,
  "totalPages": 3
}
```

### 41. Historial por punto de acceso y rango de fechas — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 4 ms

```http
GET /api/access/records?accessPointId=8a4df789-a79a-4412-8b25-924ebde43abd&fromUtc=2026-10-31T01:13:42&toUtc=2026-10-31T01:15:42&page=1&pageSize=20
```

Respuesta:

```json
{
  "items": [
    {
      "id": "1becb63f-a7e7-40a9-87e5-539b8b3c0354",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "EmployeeInactive",
      "occurredAtUtc": "2026-10-31T01:14:42.622553"
    },
    {
      "id": "94fc00f3-26a3-4c10-87a3-d4fb3a8114a0",
      "credentialCode": "CRD-P201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Rejected",
      "rejectionReason": "CredentialRevoked",
      "occurredAtUtc": "2026-10-31T01:14:42.6049883"
    },
    {
      "id": "67862326-0543-42eb-a65b-62d9702718ca",
      "credentialCode": "CRD-P201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Authorized",
      "rejectionReason": null,
      "occurredAtUtc": "2026-10-31T01:14:42.592288"
    },
    {
      "id": "a1baff65-4a26-40f3-840a-67e75a156ef5",
      "credentialCode": "CRD-P201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "CredentialBlocked",
      "occurredAtUtc": "2026-10-31T01:14:42.5734781"
    },
    {
      "id": "fa99169e-179b-4168-a411-d674829ee032",
      "credentialCode": "NOEXISTE201442",
      "accessPointCode": "BOD-201442",
      "employeeId": null,
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "CredentialNotFound",
      "occurredAtUtc": "2026-10-31T01:14:42.543347"
    },
    {
      "id": "7a3a3bd8-3ab6-4dc8-85d8-78223bf9bd67",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5378008"
    },
    {
      "id": "84bd2ce4-ab1e-44db-bbe0-aeb13f37e7f6",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Authorized",
      "rejectionReason": null,
      "occurredAtUtc": "2026-10-31T01:14:42.5310008"
    },
    {
      "id": "5e93abf2-9adf-4724-92a2-0fcee499382d",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5250675"
    },
    {
      "id": "aa104f23-ef04-496b-9e86-a6ba083a388f",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Authorized",
      "rejectionReason": null,
      "occurredAtUtc": "2026-10-31T01:14:42.5169508"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 9,
  "totalPages": 1
}
```

### 42. Historial solo de accesos rechazados — OK

**Esperado:** `200` · **Obtenido:** `200 OK` · 3 ms

```http
GET /api/access/records?result=Rejected&fromUtc=2026-10-31T01:13:42&toUtc=2026-10-31T01:15:42&page=1&pageSize=20
```

Respuesta:

```json
{
  "items": [
    {
      "id": "1becb63f-a7e7-40a9-87e5-539b8b3c0354",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "EmployeeInactive",
      "occurredAtUtc": "2026-10-31T01:14:42.622553"
    },
    {
      "id": "94fc00f3-26a3-4c10-87a3-d4fb3a8114a0",
      "credentialCode": "CRD-P201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Rejected",
      "rejectionReason": "CredentialRevoked",
      "occurredAtUtc": "2026-10-31T01:14:42.6049883"
    },
    {
      "id": "a1baff65-4a26-40f3-840a-67e75a156ef5",
      "credentialCode": "CRD-P201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0372b59b-d19a-4281-a89d-09b7cd1cb5f9",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "CredentialBlocked",
      "occurredAtUtc": "2026-10-31T01:14:42.5734781"
    },
    {
      "id": "2543f5f9-893f-419e-9194-33af3c4d1447",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "RES-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "0c29f5f8-8630-4e16-b4c7-91f0ed6a5601",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "AccessPointDisabled",
      "occurredAtUtc": "2026-10-31T01:14:42.5601674"
    },
    {
      "id": "b7eac177-1497-41d8-865e-1a615895b3c7",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "XXX-99",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": null,
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "AccessPointNotFound",
      "occurredAtUtc": "2026-10-31T01:14:42.5512118"
    },
    {
      "id": "fa99169e-179b-4168-a411-d674829ee032",
      "credentialCode": "NOEXISTE201442",
      "accessPointCode": "BOD-201442",
      "employeeId": null,
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "CredentialNotFound",
      "occurredAtUtc": "2026-10-31T01:14:42.543347"
    },
    {
      "id": "7a3a3bd8-3ab6-4dc8-85d8-78223bf9bd67",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Exit",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5378008"
    },
    {
      "id": "5e93abf2-9adf-4724-92a2-0fcee499382d",
      "credentialCode": "CRD-L201442",
      "accessPointCode": "BOD-201442",
      "employeeId": "0d62d2cc-e0ec-496b-b4b0-34b1e3c1a46e",
      "accessPointId": "8a4df789-a79a-4412-8b25-924ebde43abd",
      "movementType": "Entry",
      "result": "Rejected",
      "rejectionReason": "InvalidMovementSequence",
      "occurredAtUtc": "2026-10-31T01:14:42.5250675"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 8,
  "totalPages": 1
}
```

### 43. Rechazar rango de fechas invertido — OK

**Esperado:** `400` · **Obtenido:** `400 Bad Request` · 1 ms

```http
GET /api/access/records?fromUtc=2026-12-01&toUtc=2026-01-01
```

Respuesta:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Regla de negocio no cumplida",
  "status": 400,
  "detail": "La fecha inicial no puede ser mayor que la fecha final.",
  "traceId": "00-7c713040d603d3a5f45aa4931b4d2526-d41535f2653f07e0-00"
}
```

