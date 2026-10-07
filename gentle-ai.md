# Gentle AI + GGA: Guía de configuración y comandos

Los comandos de este documento fueron verificados contra `gentle-ai 4.0.0` y `gga 2.10.1` (salida de `--help`). Lo que no se verificó está marcado como tal.

## 1. El modelo mental

Dos herramientas separadas, dos trabajos distintos:

| Herramienta | Qué es | Cuándo se ejecuta | Alcance |
| --- | --- | --- | --- |
| **Gentle AI** (`gentle-ai`) | Configurador de ecosistema / harness para agentes de IA de programación (persona, skills, memoria, flujo de trabajo, revisión). | Una vez por máquina (instalación), luego en actualizaciones. | Tu máquina y tu agente de IA (por ejemplo Claude Code). |
| **GGA** (Gentleman Guardian Angel, `gga`) | Revisor de código con IA, agnóstico al proveedor, conectado como hook de git. | En cada `git commit`. | Un repositorio. |

¿Por qué dos herramientas? Separación de responsabilidades. Gentle AI define *cómo trabaja tu agente mientras programás*. GGA es una *barrera de calidad al momento del commit* que valida el código en staging contra tus propias reglas. Se complementan; ninguna requiere a la otra.

```
 Vos + agente de IA (configurado por Gentle AI)  ->  escribís código  ->  git commit
                                                                            |
                                                              Hook pre-commit de GGA
                                                              lee las reglas de AGENTS.md
                                                              pide al proveedor de IA revisar los archivos staged
                                                              PASSED -> el commit sigue / FAILED -> el commit se bloquea
```

## 2. Orden de ejecución (desde cero)

### Parte A: Configuración de la máquina (una vez por máquina)

1. **Instalar el ecosistema**

   ```bash
   gentle-ai install
   ```

   Configura los agentes de IA de programación en esta máquina (persona/estilo de salida, skills, servidores MCP, instrucciones del orquestador). Va primero porque todo lo demás (skills, sync, revisión) depende de que esos archivos gestionados existan.

2. **Verificar la salud**

   ```bash
   gentle-ai doctor
   ```

   Ejecuta diagnósticos del ecosistema. Usalo después de instalar y cada vez que algo se comporte raro (por ejemplo, un servidor MCP que no conecta).

3. **Verificar versión y actualizaciones**

   ```bash
   gentle-ai version
   gentle-ai update      # solo comprueba si hay actualizaciones
   gentle-ai upgrade     # aplica las actualizaciones a las herramientas gestionadas
   ```

   `update` es de solo lectura; `upgrade` modifica. Mantenerlos separados te permite mirar antes de actuar.

4. **Sincronizar configuraciones tras un upgrade**

   ```bash
   gentle-ai sync
   ```

   Lleva las configuraciones de los agentes y las skills a la versión actual. Ejecutalo después de `upgrade`, o cuando una herramienta reporte "managed assets outdated".

### Parte B: Configuración por proyecto (una vez por repositorio)

Ejecutá esto dentro de la raíz del repositorio.

5. **Inicializar git** (el hook de GGA vive en `.git/hooks`, por lo que se necesita un repo)

   ```bash
   git init
   ```

   Sin esto, `gga install` falla con `Not a git repository`.

6. **Crear la configuración de GGA**

   ```bash
   gga init
   ```

   Crea un archivo `.gga` con valores por defecto. Editalo después (ver sección 4).

7. **Escribir tus reglas en `AGENTS.md`**

   No es un comando. GGA envía el código en staging más este archivo a la IA y le pregunta "¿el código cumple estas reglas?". La calidad de la revisión es igual a la calidad de este archivo. Reglas vagas producen revisiones vagas.

8. **Instalar el hook de pre-commit**

   ```bash
   gga install
   ```

   Escribe `.git/hooks/pre-commit`. A partir de ahora cada `git commit` dispara una revisión.

   Opcional: `gga install --commit-msg` instala un hook commit-msg (validación del mensaje de commit).

9. **Refrescar el registro de skills** (opcional, por proyecto)

   ```bash
   gentle-ai skill-registry refresh
   ```

   Reconstruye `.atl/skill-registry.md`, el índice de skills disponibles por trigger y ruta, para que el agente cargue la skill correcta según la tarea.

### Parte C: Flujo diario

```bash
git add .
git commit -m "feat: add create-user use case"   # GGA se ejecuta automáticamente
```

Ejecuciones manuales y variantes:

```bash
gga run                 # Revisa los archivos staged ahora (usa caché)
gga run --no-cache      # Fuerza una revisión completa, ignora la caché
gga run --ci            # Revisa el último commit (HEAD~1..HEAD), para pipelines de CI
gga run --pr-mode       # Revisa todos los archivos cambiados en el PR completo
gga run --pr-mode --diff-only   # Igual, pero envía solo los diffs (más rápido y barato)
```

¿Por qué una caché? Revisar cuesta tiempo y tokens. GGA omite los archivos que ya fueron revisados y no cambiaron.

## 3. Referencia de comandos de GGA

| Comando | Propósito |
| --- | --- |
| `gga init` | Crea un `.gga` de ejemplo en la raíz del proyecto. |
| `gga install` | Instala el hook de pre-commit. |
| `gga install --commit-msg` | Instala el hook commit-msg. |
| `gga uninstall` | Elimina los hooks de git de este repo. |
| `gga config` | Muestra la configuración vigente. Útil para depurar "¿por qué ignora mi ajuste?". |
| `gga run [--no-cache]` | Revisa los archivos staged. |
| `gga run --ci` | Modo CI: revisa el último commit. |
| `gga run --pr-mode [--diff-only]` | Revisa el PR completo contra la rama base (autodetecta main/master/develop). |
| `gga cache status` | Muestra información de la caché de este proyecto. |
| `gga cache clear` | Limpia la caché de este proyecto. |
| `gga cache clear-all` | Limpia todos los datos en caché. |
| `gga version` / `gga help` | Versión / ayuda. |

Para saltar el hook en una emergencia: `git commit --no-verify`. Usalo con moderación; anula la barrera de calidad.

## 4. El archivo `.gga` explicado

La configuración se lee de `.gga` en la raíz del proyecto, o de `~/.config/gga/config` para valores globales.

| Clave | Significado | Este proyecto |
| --- | --- | --- |
| `PROVIDER` | Qué IA ejecuta la revisión (`claude`, `gemini`, `codex`, `opencode`, `ollama:<modelo>`, ...). | `claude` |
| `FILE_PATTERNS` | Qué archivos se revisan. | `*.cs,*.csproj` |
| `EXCLUDE_PATTERNS` | Qué archivos se omiten (código generado, migraciones). | `*.Designer.cs,*.g.cs,*Migrations/*` |
| `RULES_FILE` | Archivo con tus reglas de revisión. | `AGENTS.md` |
| `STRICT_MODE` | Falla si la respuesta de la IA es ambigua. Mantiene la barrera determinista. | `true` |
| `TIMEOUT` | Segundos máximos de espera a la IA. | `300` |
| `PR_BASE_BRANCH` | Rama base para `--pr-mode` (autodetectada si está vacía). | sin definir |

### Por qué `AGENTS.md` termina con un formato de respuesta

GGA necesita convertir texto libre de la IA en un código de salida de aprobado/rechazado. La regla "la primera línea debe ser `STATUS: PASSED` o `STATUS: FAILED`" es lo que lo hace interpretable. Con `STRICT_MODE="true"`, cualquier otra cosa hace fallar el commit en lugar de adivinar.

## 5. Referencia de comandos de Gentle AI

| Comando | Propósito |
| --- | --- |
| `gentle-ai` | Abre la TUI interactiva. |
| `gentle-ai install` | Configura los agentes de IA de programación en esta máquina. |
| `gentle-ai uninstall` | Elimina los archivos gestionados por Gentle AI. |
| `gentle-ai sync` | Sincroniza configuraciones y skills a la versión actual. |
| `gentle-ai skill-registry refresh` | Refresca `.atl/skill-registry.md`. |
| `gentle-ai update` | Comprueba actualizaciones (solo lectura). |
| `gentle-ai upgrade` | Aplica actualizaciones a las herramientas gestionadas. |
| `gentle-ai restore` | Restaura un backup de configuración. |
| `gentle-ai doctor` | Diagnósticos de salud. |
| `gentle-ai version` | Muestra la versión. |
| `gentle-ai telemetry <status\|enable\|disable\|preview\|trigger>` | Telemetría anónima y opt-out; `preview` muestra el payload exacto sin enviarlo. |

## 6. Revisión nativa (receipt-driven development, "RDD")

Un segundo sistema de revisión, integrado en Gentle AI y dirigido por tu agente, separado del hook de commit de GGA. Revisa un *candidato* (un commit o porción de trabajo) con "lentes" de revisión acotadas y registra el resultado.

### El interruptor

```bash
gentle-ai review mode status              # solo lectura: quién decidió y el modo efectivo
gentle-ai review mode disable             # desactivar
gentle-ai review mode enable              # reactivar (solo para candidatos futuros)
gentle-ai review mode disable --scope clone   # desactivado solo para este clon
```

Está **activado por defecto**. Si cualquier alcance dice "off", queda apagado. Un repositorio puede desactivarlo para un clon, pero nunca puede forzarlo a activarse.

### Comandos del ciclo de vida

Normalmente no los escribís vos; tu agente ejecuta los comandos exactos que la herramienta devuelve. Entender el flujo ayuda a leer lo que hace el agente:

1. `gentle-ai review assess` evalúa el riesgo (`passive`, `medium`, `high`) del cambio.
2. `gentle-ai review start [--cwd <repo>] [--base-ref <ref>] [--focus <lente>] [--locale <en|es>]` congela el candidato y, para riesgo medio/alto, te pide consentimiento solo para *ese candidato*.
3. `gentle-ai review capture-result ...` admite el resultado de cada revisor. Las cuatro lentes son `risk`, `resilience`, `readability` y `reliability`.
4. Si se encuentran problemas: `capture-correction-plan` (pronóstico acotado de la corrección), `capture-refuter` (cuestiona hallazgos inferenciales) y `capture-validation` (valida la corrección).
5. `gentle-ai review acknowledge-approved` es el paso final; quema la autoridad de aprobación.
6. `gentle-ai review status` es un inventario de solo lectura del estado de la revisión.

Recuperación y mantenimiento: `repair --preflight`, `recover`, `invalidate`, `abandon`.

Principio clave: el resultado de una revisión es informativo. Commit, push, PR y merge siguen siendo decisiones tuyas.

## 7. GGA vs revisión nativa

| | GGA | Revisión nativa (RDD) |
| --- | --- | --- |
| Disparador | `git commit` (hook) | Dirigida por el agente, por commit de unidad de trabajo |
| Fuente de reglas | Tu `AGENTS.md` | Lentes integradas (risk, resilience, readability, reliability) |
| Bloquea el commit | Sí (FAILED sale con código distinto de cero) | No (informativa) |
| Ideal para | Hacer cumplir *tus* convenciones de equipo | Segunda opinión independiente en cambios riesgosos |

## 8. Solución de problemas

| Síntoma | Causa | Solución |
| --- | --- | --- |
| `gga install`: `Not a git repository` | No existe el directorio `.git`. | `git init`, luego `gga install`. |
| El hook nunca corre | Hook no instalado o commit hecho con `--no-verify`. | `gga install`; revisá `.git/hooks/pre-commit`. |
| La revisión ignora archivos nuevos | Caché o `FILE_PATTERNS` no coincide. | `gga config`, `gga run --no-cache`. |
| Los ajustes parecen ignorados | Precedencia de configuración. | `gga config` muestra lo vigente. |
| Herramientas o memoria del agente no disponibles | Un servidor MCP falló al conectar (por ejemplo, falta el binario `engram`). | `gentle-ai doctor`, `claude mcp list`, corregí la causa (ver sección 9) y reiniciá la sesión. |
| El agente usa skills o configuraciones viejas | Assets gestionados desactualizados. | `gentle-ai sync`. |

## 9. Engram: memoria persistente

Engram (`engram 3.2.1`) es la memoria persistente del agente. Guarda decisiones, bugs resueltos y descubrimientos entre sesiones. Se conecta al agente como servidor MCP (stdio) con el comando `engram mcp --tools=agent`, que es lo que Claude Code ejecuta al iniciar.

### Instalación (Windows)

`gentle-ai install` registra el servidor MCP en la config del agente, pero el binario `engram` debe estar en el PATH. Si falta, `gentle-ai doctor` marca `tool:engram: engram not found in PATH` y `claude mcp list` muestra `engram ... Failed to connect — CONNECTION_CLOSED`.

```bash
gh release download -R Gentleman-Programming/engram -p "engram_*_windows_amd64.zip"
# extraer engram.exe a una carpeta del PATH (aquí: C:\Users\anton\bin)
engram version
gentle-ai doctor        # debe mostrar tool:engram [ok] y Status: healthy
```

Después hay que reiniciar Claude Code: un servidor MCP que falló al arrancar no se reconecta solo.

### Comandos principales

| Comando | Propósito |
| --- | --- |
| `engram mcp [--tools=agent\|admin\|all] [--project NAME]` | Inicia el servidor MCP (stdio). Perfil `agent` = 19 herramientas, `admin` = 4, `all` = 23. |
| `engram serve [port]` | Inicia la API HTTP (puerto por defecto 7437). |
| `engram tui` | Interfaz de terminal interactiva para explorar memorias. |
| `engram search <consulta>` | Busca memorias (`--type`, `--project`, `--all`, `--limit`). |
| `engram save <título> <mensaje>` | Guarda una memoria manualmente. |
| `engram context [proyecto]` | Muestra contexto reciente de sesiones previas. |
| `engram timeline <obs_id>` | Contexto cronológico alrededor de una observación. |
| `engram stats` | Estadísticas del sistema de memoria. |
| `engram projects list` | Lista proyectos con sus conteos. |
| `engram init [nombre]` | Inicializa un proyecto Engram (`.engram/config.json`) en el directorio actual. |
| `engram export` / `engram import <archivo>` | Exporta / importa memorias en JSON. |
| `engram delete <obs_id> [--hard]` | Borra una observación (soft-delete por defecto). |
| `engram doctor` | Diagnósticos operativos de solo lectura. |
| `engram conflicts <list\|show\|stats\|scan\|deferred>` | Inspecciona y gestiona relaciones de conflicto entre memorias. |

### Por qué importa

El agente no recuerda nada entre sesiones por sí mismo. Engram es el mecanismo que le permite retomar trabajo: al iniciar consulta contexto previo, y durante el trabajo guarda lo importante. Sin Engram el agente funciona, pero empieza cada sesión de cero.

## 10. No verificado aquí

- Los detalles internos de `gentle-ai install` (qué archivos escribe exactamente) no fueron inspeccionados.
- Las herramientas MCP individuales de Engram (`mem_save`, `mem_search`, etc.) no se probaron; la tabla anterior cubre solo la CLI.

Fuentes: `gentle-ai --help`, `gentle-ai review --help`, `gentle-ai review mode --help`, `gga --help`, `engram --help`, `gentle-ai doctor` y el `.gga` generado por `gga init`.
