# SGCAE — Sistema de Gestión y Control de Acceso de Empleados

Backend en **ASP.NET Core Web API (.NET 10)** con **Clean Architecture**, **DDD**, **CQRS** y **EF Core + SQL Server**.

Implementa los requerimientos RF-01 (empleados), RF-02 (credenciales), RF-03 (entradas y salidas),
RF-04 (historial paginado) y RF-06 (puntos de acceso).

---

## 1. Arquitectura

```
src/
├── SGCAE.Domain          ← Núcleo. Entidades, objetos de valor, reglas. NO depende de nada.
├── SGCAE.Application     ← Casos de uso (comandos/consultas) e interfaces (puertos). Depende solo de Domain.
├── SGCAE.Infrastructure  ← EF Core, SQL Server, repositorios, consultas. Implementa las interfaces de Application.
└── SGCAE.Api             ← Controladores REST, manejo de errores, composición (DI).
```

**Regla de dependencias:** las flechas apuntan siempre hacia adentro.

```
Api ──► Application ──► Domain
 │            ▲
 └──► Infrastructure
```

- El **dominio no conoce** EF Core, HTTP ni SQL Server. Por eso es fácil de probar y de cambiar.
- `Application` define *qué* necesita (`IEmployeeRepository`) e `Infrastructure` define *cómo* (`EmployeeRepository` con EF).
  Esto es **inversión de dependencias** (la D de SOLID).

## 2. Modelo de dominio (DDD)

| Agregado | Propiedades clave | Invariantes / reglas |
|---|---|---|
| **Employee** | Identification (VO), FirstName, LastName, Email (VO), Position, Area, Status | Identificación y correo únicos y válidos; un inactivo no se actualiza ni se desactiva otra vez |
| **Credential** | Code (VO), EmployeeId, Status, IssuedAtUtc | Código único; solo `Active` puede usarse; `Revoked` es estado final; máx. 1 activa por empleado |
| **AccessPoint** | Code, Name, Type, IsEnabled | Código único; tipo válido (Recepción, Parqueadero, Bodega, Área restringida) |
| **AccessRecord** | CredentialCode, AccessPointCode, EmployeeId?, AccessPointId?, MovementType, Result, RejectionReason?, OccurredAtUtc | **Inmutable** (hecho histórico). Se registran también los rechazos |

- **Objetos de valor** (`Email`, `Identification`, `CredentialCode`): inmutables, se validan al crearse → no existe un `Email` inválido en el sistema.
- **Servicio de dominio** `AccessPolicy`: la decisión de acceso combina varios agregados, por eso no vive en ninguno.
- Los agregados se referencian **solo por Id** (`Credential.EmployeeId`), nunca con navegación.

### Máquina de estados de la credencial

```
 Active ──Block──► Blocked
   ▲                 │
   └────Unblock──────┘
 Active/Blocked ──Revoke──► Revoked (final)
```

## 3. CQRS

| | Comando (escritura) | Consulta (lectura) |
|---|---|---|
| Ejemplo | `RegisterEmployeeCommand` | `GetEmployeeByIdQuery` |
| Usa | `IEmployeeRepository` → agregado con tracking | `IEmployeeQueries` → DTO con `AsNoTracking` |
| Efecto | Cambia estado + `IUnitOfWork.SaveChangesAsync` | Nunca modifica nada |

El `IDispatcher` (propio, sin librerías) busca el handler correcto por tipo. Los handlers se registran
automáticamente por escaneo del ensamblado (`Application/DependencyInjection.cs`), así que **crear un handler nuevo
no requiere registrarlo a mano**.

Flujo de una petición:

```
HTTP → Controller → dispatcher.SendAsync(command) → Handler → Dominio (reglas) → Repositorio → UnitOfWork → SQL Server
```

Errores → `GlobalExceptionHandler` los convierte en ProblemDetails:

| Excepción | HTTP |
|---|---|
| `DomainException` | 400 |
| `NotFoundException` | 404 |
| `ConflictException` | 409 |

## 4. Cómo ejecutarlo

Requisitos: .NET 10 SDK y SQL Server (LocalDB en Windows, o Docker).

```bash
# Herramienta de migraciones (una sola vez)
dotnet tool install --global dotnet-ef

dotnet restore
dotnet build
```

**Cadena de conexión** en `src/SGCAE.Api/appsettings.json` (`ConnectionStrings:Sgcae`). Por defecto usa LocalDB.
Con Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sgcae_Pass123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
# y en appsettings:
# "Server=localhost,1433;Database=SGCAE;User Id=sa;Password=Sgcae_Pass123!;TrustServerCertificate=True"
```

**Base de datos** (desde la raíz del repo). La migración inicial ya está incluida en
`src/SGCAE.Infrastructure/Persistence/Migrations`, solo hay que aplicarla:

```bash
dotnet ef database update -p src/SGCAE.Infrastructure -s src/SGCAE.Api
```

**Ejecutar:**

```bash
dotnet run --project src/SGCAE.Api
```

Swagger UI: `http://localhost:5125/swagger` · OpenAPI: `/openapi/v1.json` · Peticiones de ejemplo: `src/SGCAE.Api/SGCAE.Api.http`.

## 5. Decisión de acceso (RF-03)

`AccessPolicy.Evaluate` revisa en este orden y devuelve el **primer** motivo que aplique:

| # | Condición | Resultado |
|---|---|---|
| 1 | La credencial no existe (o su formato es inválido) | `CredentialNotFound` |
| 2 | Credencial revocada | `CredentialRevoked` |
| 3 | Credencial bloqueada | `CredentialBlocked` |
| 4 | Empleado inactivo | `EmployeeInactive` |
| 5 | Punto de acceso no existe | `AccessPointNotFound` |
| 6 | Punto de acceso deshabilitado | `AccessPointDisabled` |
| 7 | Entrada estando adentro, o salida estando afuera | `InvalidMovementSequence` |
| — | Todo correcto | `Authorized` |

**Un rechazo no es un error HTTP:** la API responde `200` con `result = Rejected` y el intento queda guardado en el
historial. Así se cumple la trazabilidad que pide el caso (también de los accesos rechazados).
La regla 7 evita registros duplicados (dos entradas seguidas sin salida).

Otras reglas de negocio implementadas:
- Un empleado tiene **máximo una credencial activa**. Para asignar otra, primero se bloquea o revoca la actual.
- No se asignan credenciales a empleados inactivos.
- Una credencial revocada no se puede bloquear ni desbloquear (estado final).
- En el historial, la fecha inicial no puede ser mayor que la final; `pageSize` máximo 100.

## 6. Endpoints

| RF | Método | Ruta | Descripción |
|---|---|---|---|
| 01 | POST | `/api/employees` | Registrar empleado |
| 01 | GET | `/api/employees?status=&area=&page=&pageSize=` | Listar (paginado) |
| 01 | GET | `/api/employees/{id}` | Consultar |
| 01 | PUT | `/api/employees/{id}` | Actualizar |
| 01 | PATCH | `/api/employees/{id}/deactivate` | Desactivar |
| 02 | POST | `/api/employees/{employeeId}/credentials` | Asignar credencial |
| 02 | GET | `/api/employees/{employeeId}/credentials` | Credenciales del empleado |
| 02 | GET | `/api/credentials/{id}` | Consultar estado |
| 02 | POST | `/api/credentials/{id}/block` | Bloquear |
| 02 | POST | `/api/credentials/{id}/unblock` | Desbloquear |
| 02 | POST | `/api/credentials/{id}/revoke` | Revocar |
| 03 | POST | `/api/access/requests` | Solicitud de entrada/salida |
| 04 | GET | `/api/access/records?employeeId=&fromUtc=&toUtc=&accessPointId=&result=&page=&pageSize=` | Historial |
| 06 | POST | `/api/access-points` | Registrar punto |
| 06 | GET | `/api/access-points?isEnabled=` | Listar puntos |
| 06 | GET | `/api/access-points/{id}` | Consultar punto |
| 06 | PATCH | `/api/access-points/{id}/enable` · `/disable` | Habilitar / deshabilitar |

## 7. Pruebas

El archivo `src/SGCAE.Api/SGCAE.Api.http` contiene un guion completo que recorre todos los requerimientos en orden:
registro de empleados, puntos de acceso y credenciales; accesos autorizados y cada motivo de rechazo; y consultas
del historial con filtros y paginación. Se ejecuta desde Visual Studio, Rider o VS Code (extensión REST Client).
Los ids se encadenan automáticamente entre peticiones.

También se pueden probar todos los endpoints desde Swagger UI.

## 8. Estructura del repositorio y flujo de trabajo

- `main`: rama estable.
- `feature/*`: una rama por capa de la solución, integrada a `main` mediante Pull Request.
- Mensajes de commit con el formato *Conventional Commits* (`feat:`, `docs:`, `chore:`).