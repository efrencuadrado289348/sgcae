#:property PublishAot=false

// Ejecuta el escenario completo de pruebas contra la API en ejecución y guarda
// cada petición y respuesta en docs/evidencias/README.md.
//
// Uso (con la API corriendo, desde la raíz del repo):
//   dotnet run scripts/evidencias.cs
//   dotnet run scripts/evidencias.cs -- http://localhost:5125

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var baseUrl = (args.Length > 0 ? args[0] : "http://localhost:5125").TrimEnd('/');
var outputDir = Path.Combine(FindRepoRoot(), "docs", "evidencias");
Directory.CreateDirectory(outputDir);

using var http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(15) };
var report = new StringBuilder();
var jsonOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
int step = 0, passed = 0;

// Sufijo único: permite ejecutar el script varias veces sin chocar con datos existentes.
var run = DateTime.Now.ToString("HHmmss");
var startedAt = DateTime.Now;

report.AppendLine("# Evidencias de pruebas — SGCAE");
report.AppendLine();
report.AppendLine($"- **Fecha de ejecución:** {startedAt:yyyy-MM-dd HH:mm:ss}");
report.AppendLine($"- **API:** `{baseUrl}`");
report.AppendLine($"- **Generado con:** `dotnet run scripts/evidencias.cs`");
report.AppendLine();
report.AppendLine("Cada paso muestra la petición enviada, la respuesta real de la API y si coincide con el resultado esperado.");
report.AppendLine();

try
{
    // ------------------------------------------------------------ RF-01
    Section("RF-01 — Gestión de empleados");
    var laura = await Step("Registrar empleado", "POST", "/api/employees", new
    {
        identification = $"10{run}01", firstName = "Laura", lastName = "Gómez",
        email = $"laura.gomez.{run}@empresa.com", position = "Auxiliar de bodega", area = "Operaciones"
    }, 201);
    var lauraId = Id(laura);

    var pedro = await Step("Registrar segundo empleado", "POST", "/api/employees", new
    {
        identification = $"20{run}02", firstName = "Pedro", lastName = "Ruiz",
        email = $"pedro.ruiz.{run}@empresa.com", position = "Supervisor", area = "Logística"
    }, 201);
    var pedroId = Id(pedro);

    await Step("Rechazar identificación duplicada", "POST", "/api/employees", new
    {
        identification = $"10{run}01", firstName = "Otra", lastName = "Persona",
        email = $"otra.{run}@empresa.com", position = "X", area = "Y"
    }, 409);

    await Step("Rechazar correo con formato inválido", "POST", "/api/employees", new
    {
        identification = $"30{run}03", firstName = "Ana", lastName = "Díaz",
        email = "correo-malo", position = "X", area = "Y"
    }, 400);

    await Step("Consultar empleado por id", "GET", $"/api/employees/{lauraId}", null, 200);

    await Step("Actualizar empleado", "PUT", $"/api/employees/{lauraId}", new
    {
        firstName = "Laura", lastName = "Gómez Pérez", email = $"laura.gomez.{run}@empresa.com",
        position = "Coordinadora de bodega", area = "Operaciones"
    }, 204);

    await Step("Listar empleados activos (paginado)", "GET", "/api/employees?status=Active&page=1&pageSize=5", null, 200);

    // ------------------------------------------------------------ RF-06
    Section("RF-06 — Administración de puntos de acceso");
    var bod = $"BOD-{run}";
    var res = $"RES-{run}";
    var bodega = await Step("Registrar punto: bodega", "POST", "/api/access-points",
        new { code = bod, name = "Bodega principal", type = "Warehouse" }, 201);
    var bodegaId = Id(bodega);
    await Step("Registrar punto: recepción", "POST", "/api/access-points",
        new { code = $"REC-{run}", name = "Recepción", type = "Reception" }, 201);
    await Step("Registrar punto: parqueadero", "POST", "/api/access-points",
        new { code = $"PAR-{run}", name = "Parqueadero", type = "Parking" }, 201);
    var restringida = await Step("Registrar punto: área restringida", "POST", "/api/access-points",
        new { code = res, name = "Cuarto de servidores", type = "RestrictedArea" }, 201);
    var restringidaId = Id(restringida);

    await Step("Rechazar código de punto duplicado", "POST", "/api/access-points",
        new { code = bod, name = "Otra bodega", type = "Warehouse" }, 409);
    await Step("Deshabilitar área restringida", "PATCH", $"/api/access-points/{restringidaId}/disable", null, 204);
    await Step("Consultar punto deshabilitado", "GET", $"/api/access-points/{restringidaId}", null, 200);
    await Step("Listar solo puntos habilitados", "GET", "/api/access-points?isEnabled=true", null, 200);

    // ------------------------------------------------------------ RF-02
    Section("RF-02 — Gestión de credenciales");
    var crdLaura = $"CRD-L{run}";
    var crdPedro = $"CRD-P{run}";
    var credLaura = await Step("Asignar credencial a Laura", "POST", $"/api/employees/{lauraId}/credentials",
        new { code = crdLaura }, 201);
    var credLauraId = Id(credLaura);
    var credPedro = await Step("Asignar credencial a Pedro", "POST", $"/api/employees/{pedroId}/credentials",
        new { code = crdPedro }, 201);
    var credPedroId = Id(credPedro);

    await Step("Rechazar segunda credencial activa", "POST", $"/api/employees/{lauraId}/credentials",
        new { code = $"CRD-X{run}" }, 409);
    await Step("Rechazar código de credencial duplicado", "POST", $"/api/employees/{pedroId}/credentials",
        new { code = crdLaura }, 409);
    await Step("Consultar estado de credencial", "GET", $"/api/credentials/{credLauraId}", null, 200);
    await Step("Listar credenciales del empleado", "GET", $"/api/employees/{lauraId}/credentials", null, 200);

    // ------------------------------------------------------------ RF-03
    Section("RF-03 — Registro de entradas y salidas");
    await Access("Entrada autorizada", crdLaura, bod, "Entry", "Authorized", null);
    await Access("Segunda entrada sin salida", crdLaura, bod, "Entry", "Rejected", "InvalidMovementSequence");
    await Access("Salida autorizada", crdLaura, bod, "Exit", "Authorized", null);
    await Access("Salida sin entrada previa", crdLaura, bod, "Exit", "Rejected", "InvalidMovementSequence");
    await Access("Credencial inexistente", $"NOEXISTE{run}", bod, "Entry", "Rejected", "CredentialNotFound");
    await Access("Punto de acceso inexistente", crdLaura, "XXX-99", "Entry", "Rejected", "AccessPointNotFound");
    await Access("Punto de acceso deshabilitado", crdLaura, res, "Entry", "Rejected", "AccessPointDisabled");

    await Step("Bloquear credencial de Pedro", "POST", $"/api/credentials/{credPedroId}/block", null, 204);
    await Access("Credencial bloqueada", crdPedro, bod, "Entry", "Rejected", "CredentialBlocked");
    await Step("Desbloquear credencial de Pedro", "POST", $"/api/credentials/{credPedroId}/unblock", null, 204);
    await Access("Credencial desbloqueada vuelve a funcionar", crdPedro, bod, "Entry", "Authorized", null);
    await Step("Revocar credencial de Pedro", "POST", $"/api/credentials/{credPedroId}/revoke", null, 204);
    await Access("Credencial revocada", crdPedro, bod, "Exit", "Rejected", "CredentialRevoked");
    await Step("Rechazar bloqueo de credencial revocada", "POST", $"/api/credentials/{credPedroId}/block", null, 400);

    await Step("Desactivar empleado", "PATCH", $"/api/employees/{lauraId}/deactivate", null, 204);
    await Access("Empleado inactivo", crdLaura, bod, "Entry", "Rejected", "EmployeeInactive");
    await Step("Rechazar actualización de empleado inactivo", "PUT", $"/api/employees/{lauraId}", new
    {
        firstName = "Laura", lastName = "Gómez", email = $"laura.gomez.{run}@empresa.com",
        position = "X", area = "Y"
    }, 400);

    // ------------------------------------------------------------ RF-04
    Section("RF-04 — Consulta del historial de accesos");
    var from = startedAt.ToUniversalTime().AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss");
    var to = DateTime.UtcNow.AddMinutes(1).ToString("yyyy-MM-ddTHH:mm:ss");
    await Step("Historial por empleado (página 1 de 3 registros)", "GET",
        $"/api/access/records?employeeId={lauraId}&page=1&pageSize=3", null, 200);
    await Step("Historial por empleado (página 2)", "GET",
        $"/api/access/records?employeeId={lauraId}&page=2&pageSize=3", null, 200);
    await Step("Historial por punto de acceso y rango de fechas", "GET",
        $"/api/access/records?accessPointId={bodegaId}&fromUtc={from}&toUtc={to}&page=1&pageSize=20", null, 200);
    await Step("Historial solo de accesos rechazados", "GET",
        $"/api/access/records?result=Rejected&fromUtc={from}&toUtc={to}&page=1&pageSize=20", null, 200);
    await Step("Rechazar rango de fechas invertido", "GET",
        "/api/access/records?fromUtc=2026-12-01&toUtc=2026-01-01", null, 400);
}
catch (HttpRequestException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nNo se pudo conectar con {baseUrl}: {ex.Message}");
    Console.WriteLine("Verifica que la API esté corriendo (dotnet run --project src/SGCAE.Api).");
    Console.ResetColor();
    return 1;
}

var summary = $"**Resultado: {passed} de {step} pasos coinciden con lo esperado.**";
report.Insert(report.ToString().IndexOf("Cada paso", StringComparison.Ordinal), summary + "\n\n");

var file = Path.Combine(outputDir, "README.md");
await File.WriteAllTextAsync(file, report.ToString(), new UTF8Encoding(false));

Console.WriteLine();
Console.ForegroundColor = passed == step ? ConsoleColor.Green : ConsoleColor.Yellow;
Console.WriteLine($"{passed}/{step} pasos correctos. Evidencias guardadas en: {file}");
Console.ResetColor();
return passed == step ? 0 : 2;

// ================================================================== helpers

void Section(string title)
{
    report.AppendLine($"## {title}");
    report.AppendLine();
    Console.WriteLine($"\n== {title}");
}

async Task<JsonNode?> Access(string title, string credential, string point, string movement,
    string expectedResult, string? expectedReason)
{
    var body = await Step($"Acceso: {title}", "POST", "/api/access/requests",
        new { credentialCode = credential, accessPointCode = point, movementType = movement }, 200,
        json => json?["result"]?.GetValue<string>() == expectedResult
             && json?["rejectionReason"]?.GetValue<string>() == expectedReason,
        expectedReason is null ? expectedResult : $"{expectedResult} / {expectedReason}");
    return body;
}

async Task<JsonNode?> Step(string title, string method, string url, object? payload, int expectedStatus,
    Func<JsonNode?, bool>? extraCheck = null, string? expectedDetail = null)
{
    step++;
    using var request = new HttpRequestMessage(new HttpMethod(method), url);
    if (payload is not null) request.Content = JsonContent.Create(payload);

    var watch = Stopwatch.StartNew();
    using var response = await http.SendAsync(request);
    watch.Stop();

    var raw = await response.Content.ReadAsStringAsync();
    JsonNode? json = null;
    if (!string.IsNullOrWhiteSpace(raw))
    {
        try { json = JsonNode.Parse(raw); } catch (JsonException) { }
    }

    var status = (int)response.StatusCode;
    var ok = status == expectedStatus && (extraCheck is null || extraCheck(json));
    if (ok) passed++;

    var mark = ok ? "OK" : "FALLO";
    var expected = $"`{expectedStatus}`" + (expectedDetail is null ? "" : $" — `{expectedDetail}`");

    report.AppendLine($"### {step:00}. {title} — {mark}");
    report.AppendLine();
    report.AppendLine($"**Esperado:** {expected} · **Obtenido:** `{status} {response.ReasonPhrase}` · {watch.ElapsedMilliseconds} ms");
    report.AppendLine();
    report.AppendLine("```http");
    report.AppendLine($"{method} {url}");
    if (payload is not null)
    {
        report.AppendLine("Content-Type: application/json");
        report.AppendLine();
        report.AppendLine(JsonSerializer.Serialize(payload, jsonOptions));
    }
    report.AppendLine("```");
    report.AppendLine();
    report.AppendLine("Respuesta:");
    report.AppendLine();
    report.AppendLine("```json");
    report.AppendLine(json is null ? "(sin contenido)" : json.ToJsonString(jsonOptions));
    report.AppendLine("```");
    report.AppendLine();

    Console.ForegroundColor = ok ? ConsoleColor.Green : ConsoleColor.Red;
    Console.Write($"[{mark}] ");
    Console.ResetColor();
    Console.WriteLine($"{step:00}. {title} -> {status}");
    return json;
}

static string Id(JsonNode? json) =>
    json?["id"]?.GetValue<string>() ?? throw new InvalidOperationException("La respuesta no trajo un id; revisa el paso anterior.");

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "SGCAE.sln")))
        dir = dir.Parent;
    return dir?.FullName ?? Directory.GetCurrentDirectory();
}