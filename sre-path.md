# SRE / DevOps Learning Path

Practice project: this repository (.NET 10 Clean Architecture API with unit and integration tests).
Every phase ends with something **you build yourself**. Claude guides, you type.

## How to use this plan

Every step has the same shape:

- **Do**: what to build, with hints (never the full solution).
- **Verify**: observable proof that it works. No proof, no checkmark.
- **Understand**: a question you must answer in your own words before moving on.

Rules:

1. Concepts before commands. If you cannot explain the problem, do not touch the tool.
2. Do it by hand first, automate after.
3. Work on a branch per step (`git switch -c ci/step-1-3`), open a PR, merge when green. The PR flow is part of the training.
4. Keep a `notes/` log: what broke, why, how you fixed it.
5. Hint ladder when stuck: concept hint, then command hint, then ask Claude to review your attempt. Solution last.
6. Mark steps as done by changing `[ ]` to `[x]`.

## Roadmap at a glance

| Phase | Topic | Outcome | Time |
|-------|-------|---------|------|
| 0 | Foundations | CLI, Git, HTTP, request path | 1 week |
| 1 | GitHub Actions (CI) | Every PR is built, tested, gated | 2 weeks |
| 2 | Docker | API runs identically anywhere | 2 weeks |
| 3 | CD and registries | Versioned images published automatically | 1-2 weeks |
| 4 | Azure | API live on the internet | 3 weeks |
| 5 | Infrastructure as Code | Environment reproducible from code | 2-3 weeks |
| 6 | Observability | You can see what the system does | 3 weeks |
| 7 | SRE core | SLIs, SLOs, error budgets, incidents | 3-4 weeks |
| 8 | Reliability and security | Resilience, supply chain, restore drills | ongoing |
| 9 | Kubernetes (optional) | Orchestration concepts | optional |

Pace: 3 to 5 hours per week. Do not rush. Depth beats speed.

---

## Phase 0: Foundations

**Concepts**: DevOps (shared ownership of build and run) vs SRE (reliability as an engineering discipline with measurable targets). Delivery lifecycle: commit, build, test, package, deploy, operate, observe.

- [ ] **0.1 Install the toolbox**
  - Do: install and check versions of `git`, `dotnet` (10), `gh` (GitHub CLI), `docker` (Docker Desktop), `az` (Azure CLI). Run `gh auth login`.
  - Verify: `git --version; dotnet --version; gh auth status; docker --version; az --version` all print without errors.
  - Understand: why do we pin tool versions (`global.json`) instead of trusting whatever is installed?

- [ ] **0.2 Run the API locally and poke it with curl**
  - Do: `dotnet run --project src/ApiCleanArch.Api`. Call the users endpoints with `curl -i`. Read status codes and headers. Send a bad request and read the ProblemDetails body.
  - Verify: you can explain every header in one response.
  - Understand: what is the difference between a 4xx and a 5xx, and which one counts against reliability?

- [ ] **0.3 Trace a request end to end**
  - Do: draw on paper: browser, DNS, TCP, TLS, reverse proxy, Kestrel, controller, use case, repository. Use `curl -v` and `nslookup` against a public site to see DNS and TLS.
  - Verify: you can narrate the path out loud in two minutes.
  - Understand: where could latency hide on each hop?

- [ ] **0.4 Exit codes and the shell**
  - Do: run `dotnet test`, then print the exit code (`$LASTEXITCODE` in PowerShell, `$?` in bash). Break a test, run again, compare.
  - Verify: you saw 0 and non-zero.
  - Understand: CI decides pass/fail only from exit codes. Why does that matter?

- [ ] **0.5 Read (in parallel, 15 min a day)**: Google SRE Book chapters 1-3 (free online).

**Checkpoint**: explain what happens when you type `https://example.com` and what an exit code is.

---

## Phase 1: CI with GitHub Actions

**Concepts**: CI is integrating small changes often and failing fast. A workflow is triggered by **events**, contains **jobs**, which contain **steps**, which run on **runners** (fresh ephemeral VMs). Nothing persists between jobs unless you use artifacts or cache. `GITHUB_TOKEN` has permissions you should minimize.

Work in `.github/workflows/`.

- [ ] **1.1 Hello workflow**
  - Do: create `.github/workflows/hello.yml` that triggers on `push` and runs one step printing `hello`. Look up in the docs: `name`, `on`, `jobs.<id>.runs-on`, `steps`, `run`.
  - Verify: Actions tab shows a green run. Open it and read the log of each step, including the hidden "Set up job".
  - Understand: where does that code actually run, and what is on that machine?

- [ ] **1.2 Check out the code**
  - Do: add the `actions/checkout` step and then `run: ls` (or `dir`). Remove checkout, run again, compare.
  - Verify: you saw an empty workspace without checkout and your repo with it.
  - Understand: why is the repo not there by default?

- [ ] **1.3 Build and test (your first real CI)**
  - Do: create `ci.yml`. Triggers: `pull_request` and `push` to `master`. Steps: checkout, `actions/setup-dotnet` (version 10.x), `dotnet restore`, `dotnet build --no-restore -c Release`, `dotnet test --no-build -c Release`.
  - Verify: green on a PR. Hint: pin the version with `global.json` and `global-json-file`.
  - Understand: why do we use `--no-restore` and `--no-build` on later steps?

- [ ] **1.4 Watch it fail on purpose**
  - Do: on a throwaway branch break one test, push, open a PR. Read the failure. Fix. Push again.
  - Verify: red, then green. You found the failing test from the log alone.
  - Understand: what turns a step red? What would `continue-on-error: true` change?

- [ ] **1.5 Branch protection**
  - Do: Repo Settings, Rules (or Branches), protect `master`: require pull request, require the `ci` status check, block force-push.
  - Verify: try to merge the red PR from 1.4. It must be blocked.
  - Understand: a CI that cannot block a merge is only a suggestion. Why?

- [ ] **1.6 Caching**
  - Do: enable NuGet caching (`actions/setup-dotnet` has a `cache` input; it needs `packages.lock.json`, so enable lock files with `RestorePackagesWithLockFile`). Run twice.
  - Verify: second run is faster. Write both timings in `notes/`.
  - Understand: what is the cache key and when is the cache invalidated?

- [ ] **1.7 Split into jobs**
  - Do: separate `build`, `unit-tests` (Domain, Application), `integration-tests` (Api). Use `needs:` so tests wait for build. Think about what runs in parallel.
  - Verify: the run graph shows the dependency graph you designed.
  - Understand: each job starts on a clean machine. What did that cost you (rebuilds) and how do artifacts solve it?

- [ ] **1.8 Test results and coverage as artifacts**
  - Do: run tests with `--logger trx` and `--collect:"XPlat Code Coverage"`, upload with `actions/upload-artifact`. Use `if: always()` so results upload even on failure.
  - Verify: download the artifact from the run page.
  - Understand: why `if: always()`?

- [ ] **1.9 Formatting gate**
  - Do: add `dotnet format --verify-no-changes`. Make a badly formatted commit and watch it fail. Respect the repo `.editorconfig` rules in AGENTS.md.
  - Verify: red on bad formatting, green after fix.
  - Understand: why check formatting in CI instead of arguing in reviews?

- [ ] **1.10 Least privilege and concurrency**
  - Do: add top-level `permissions: contents: read`. Add `concurrency` with `cancel-in-progress: true` per ref.
  - Verify: push twice quickly; the first run is cancelled.
  - Understand: what can a compromised step do with a write token?

- [ ] **1.11 Supply chain hygiene**
  - Do: pin every third-party `uses:` to a full commit SHA (keep the version in a comment). Add `.github/dependabot.yml` for `nuget` and `github-actions`.
  - Verify: Dependabot opens a PR within a day or when triggered from the Insights tab.
  - Understand: why is `uses: some/action@v1` a risk (tags are mutable)?

- [ ] **1.12 Status badge and README**
  - Do: add the CI badge and a short "how CI works" section to README.
  - Verify: badge shows passing.

**Checkpoint**: explain `permissions`, why fork PRs do not get your secrets, and why jobs do not share a filesystem.

---

## Phase 2: Docker

**Concepts**: container vs VM (namespaces and cgroups, shared kernel), image vs container, layers and cache, registry, multi-stage builds, twelve-factor config (env vars, logs to stdout).

- [ ] **2.1 Run something before building something**
  - Do: `docker run --rm hello-world`, then `docker run --rm -it mcr.microsoft.com/dotnet/sdk:10.0 bash`. Explore inside, exit, run again, see it is clean.
  - Verify: you understand the container is disposable.
  - Understand: where did the files you created inside go?

- [ ] **2.2 First Dockerfile (naive)**
  - Do: write a single-stage `Dockerfile` using the SDK image: copy everything, `dotnet publish`, run. Build with `docker build -t api:dev .`, run with `-p 8080:8080`.
  - Verify: `curl localhost:8080/...` works. Hint: set `ASPNETCORE_URLS`.
  - Understand: check `docker images`. Why is it so big?

- [ ] **2.3 Multi-stage build**
  - Do: stage 1 `sdk` publishes; stage 2 `aspnet` runtime copies the output only.
  - Verify: image is several hundred MB smaller. Record sizes in `notes/`.
  - Understand: what is not in the final image that was in the first one, and why is that a security gain?

- [ ] **2.4 Layer cache optimization**
  - Do: copy only `*.csproj` files first, run `dotnet restore`, then copy the rest. Rebuild after editing a `.cs` file.
  - Verify: restore layer shows `CACHED`.
  - Understand: why does instruction order determine rebuild speed?

- [ ] **2.5 `.dockerignore`**
  - Do: exclude `bin/`, `obj/`, `.git`, tests output.
  - Verify: build context size shrinks (see the "transferring context" line).

- [ ] **2.6 Non-root and health check**
  - Do: run as a non-root user (the aspnet images ship an `app` user). Add a health endpoint to the API (`/health`) and a `HEALTHCHECK` or use compose healthchecks.
  - Verify: `docker exec <c> whoami` is not root; `docker ps` shows `healthy`.
  - Understand: what can an attacker do if the container runs as root and escapes?

- [ ] **2.7 Configuration through the environment**
  - Do: run with `-e` to override a setting. Never bake secrets into the image. Inspect with `docker history` and `docker inspect` to see what leaks.
  - Verify: change behavior without rebuilding.
  - Understand: why is the same image used in every environment?

- [ ] **2.8 Docker Compose**
  - Do: write `compose.yaml` for the API (and a Postgres or SQL container once you add a database). Networks, volumes, `depends_on` with health conditions.
  - Verify: `docker compose up` starts everything; `down -v` cleans it.
  - Understand: how does the API find the DB by name?

- [ ] **2.9 Build the image in CI**
  - Do: add a CI job running `docker build` (use `docker/build-push-action` with `push: false` and GitHub Actions cache).
  - Verify: the job is green and a broken Dockerfile turns it red.

**Checkpoint**: why is `latest` dangerous, where do logs go, and why does changing one source line not re-run restore?

---

## Phase 3: CD and registries

**Concepts**: Delivery vs Deployment, immutable artifacts (build once, promote), versioning, rollback as a first-class feature.

- [ ] **3.1 Publish to GHCR**
  - Do: workflow triggered on push to `master`. Log in with `docker/login-action` using `GITHUB_TOKEN` and `packages: write` permission only on this job. Push to `ghcr.io/<you>/api-clean-arch`.
  - Verify: image appears in the repo Packages page; `docker pull` works.

- [ ] **3.2 Tagging strategy**
  - Do: tag with the git SHA always, and a SemVer tag on releases (`docker/metadata-action` helps). Create a GitHub Release or tag `v0.1.0`.
  - Verify: you can name the exact commit behind any running image.
  - Understand: why is a SHA tag better than `latest` for rollback?

- [ ] **3.3 Environments and approvals**
  - Do: create GitHub Environments `staging` and `production`. Add required reviewers on production.
  - Verify: a run pauses and waits for your approval.

- [ ] **3.4 Vulnerability scanning**
  - Do: add Trivy (`aquasecurity/trivy-action`, pinned by SHA). Fail on `CRITICAL`.
  - Verify: scan report visible; investigate one finding and decide: fix, accept, or ignore with justification.

- [ ] **3.5 Rollback drill**
  - Do: deploy (locally via compose for now) version A, then B, then roll back to A using only a tag.
  - Verify: under five minutes, by memory of the procedure.

**Checkpoint**: explain "build once, deploy many" and how a rollback works in your setup.

---

## Phase 4: Azure

**Concepts**: IaaS/PaaS/serverless, subscription, resource group, region, Entra ID, RBAC, managed identity, cost control.

- [ ] **4.1 Account safety first**
  - Do: create the free account. **Before creating anything**, set a Cost Management budget with email alerts at 50%, 80%, 100%. Enable MFA.
  - Verify: budget visible in the portal.

- [ ] **4.2 Manual deploy via portal**
  - Do: create a resource group, then an Azure Container Registry (or reuse GHCR), then a **Container App** running your image. Expose HTTP ingress.
  - Verify: public URL answers your API.
  - Understand: what did the portal create behind the scenes (environment, log workspace)?

- [ ] **4.3 Repeat with Azure CLI**
  - Do: delete everything (`az group delete`), then redo with `az login`, `az group create`, `az containerapp up` or the explicit commands.
  - Verify: same result; save the commands in `notes/` (they become your IaC draft in Phase 5).
  - Understand: what could the portal not teach you that the CLI did?

- [ ] **4.4 Deploy from GitHub with OIDC**
  - Do: create an app registration or user-assigned identity with a **federated credential** for your repo and the `production` environment. Grant it the minimum RBAC role on the resource group. Use `azure/login` with `id-token: write`.
  - Verify: workflow deploys with no stored password anywhere.
  - Understand: what happens to this credential if the repo is compromised, and how did scoping reduce the risk?

- [ ] **4.5 Secrets with Key Vault and managed identity**
  - Do: create a Key Vault, store a secret, give the Container App a managed identity with `Key Vault Secrets User`. Read it from the app configuration.
  - Verify: no secret in the repo, in env files, or in the image.

- [ ] **4.6 Real database**
  - Do: provision Azure SQL or PostgreSQL (smallest tier). Add EF Core migrations to the project and apply them from the pipeline as a separate, controlled step.
  - Verify: the API persists data across restarts.
  - Understand: why should migrations not run at app startup in production?

- [ ] **4.7 Domain and HTTPS**
  - Do: add a custom domain and managed certificate, or at least confirm the default TLS cert.
  - Verify: `curl -v` shows a valid certificate chain.

- [ ] **4.8 Cost review**
  - Do: check Cost Analysis, scale to zero where possible, delete unused resources.
  - Verify: you can state the monthly cost and its main driver.

**Checkpoint**: draw your architecture with every network hop and explain a managed identity.

---

## Phase 5: Infrastructure as Code

**Concepts**: declarative vs imperative, desired state, idempotency, drift, state files, review plans like code.

- [ ] **5.1 Choose and justify**: Bicep (Azure-native, simple) or Terraform (portable, industry standard). Write the decision and tradeoffs in `docs/adr/0001-iac-tool.md`.
- [ ] **5.2 Translate one resource**: resource group plus Container App environment from your `notes/` CLI commands. Verify: `what-if` or `terraform plan` shows the expected diff.
- [ ] **5.3 Whole environment as code**: registry, app, Key Vault, DB, identities, role assignments. Verify: destroy and recreate everything from code; time it.
- [ ] **5.4 Parameters and environments**: one module, two parameter files (`staging`, `production`). Verify: both deploy from the same code.
- [ ] **5.5 Plan in PRs, apply on merge**: CI runs `what-if`/`plan` on pull requests and posts the result; `apply` runs on merge with the production approval gate. Verify: an infra change goes through the full PR flow.
- [ ] **5.6 Drift drill**: change a setting in the portal, detect it with a plan, reconcile through code. Understand: why is portal editing in production a bad habit?
- [ ] **5.7 State safety (Terraform only)**: remote state in a storage account with locking, never committed. Verify: two simultaneous applies cannot corrupt state.

**Checkpoint**: if the subscription vanished tonight, how long to rebuild and what is not in code?

---

## Phase 6: Observability

**Concepts**: monitoring vs observability, logs/metrics/traces, structured logging, correlation IDs, RED and USE methods, OpenTelemetry.

- [ ] **6.1 Structured logs**: JSON logs with `ILogger` or Serilog, with a correlation ID per request. Never log tokens or PII (see AGENTS.md). Verify: you can filter logs by one request ID.
- [ ] **6.2 Health endpoints**: separate **liveness** (process is alive) from **readiness** (can serve traffic, DB reachable). Verify: stop the DB and watch only readiness fail. Understand: what happens if liveness depends on the DB?
- [ ] **6.3 OpenTelemetry instrumentation**: add traces and metrics (ASP.NET Core, HttpClient, runtime). Export via OTLP. Verify: spans visible in a local collector or Jaeger.
- [ ] **6.4 Local stack**: Prometheus and Grafana via Compose. Build a RED dashboard: requests per second, error ratio, p50/p95/p99 latency. Verify: generate load with a loop of `curl` and watch the graphs move.
- [ ] **6.5 Cloud telemetry**: send to Application Insights / Azure Monitor. Verify: the same RED data in the cloud, plus an end-to-end trace.
- [ ] **6.6 Actionable alerts**: alert on user-facing symptoms (error ratio, latency), not on CPU. Each alert states who acts and what they do. Verify: trigger one deliberately and receive it.
- [ ] **6.7 Investigation drill**: introduce a slow path (artificial delay). From telemetry only, find it. Write the investigation steps in `notes/`.

**Checkpoint**: "it was slow at 3pm", show the investigation path with your tools.

---

## Phase 7: SRE core

**Concepts**: SLI, SLO, error budget, SLA, toil, incident roles, blameless postmortem, capacity, graceful degradation.

- [ ] **7.1 Pick the user journeys**: list what users do (create user, get user). Which matter most? Write in `docs/slo.md`.
- [ ] **7.2 Define SLIs**: for example availability = good requests / valid requests, latency = fraction under 300 ms. State exactly what counts as good. Understand: why is a 4xx usually not a failure, but a 5xx is?
- [ ] **7.3 Set SLOs**: for example 99.5% over 30 days. Compute the error budget in minutes. Verify: you can say how many bad requests or minutes per month you can afford.
- [ ] **7.4 Dashboards for SLOs**: show current compliance and remaining budget.
- [ ] **7.5 Burn-rate alerts**: implement fast burn (page now) and slow burn (ticket). Read the SRE Workbook chapter on alerting on SLOs first.
- [ ] **7.6 Error budget policy**: write down what happens when the budget is exhausted (freeze features, fix reliability). Verify: a one-page policy in `docs/`.
- [ ] **7.7 Load test with k6**: ramp up traffic, find the breaking point and the first bottleneck. Verify: a report with the capacity limit and what failed first.
- [ ] **7.8 Runbooks**: one per alert, with symptoms, checks, mitigation, escalation. Verify: someone who did not write it could follow it.
- [ ] **7.9 Game day**: inject failures (kill the container, break the DB connection, add latency). Practice incident roles: commander, ops, comms. Mitigate first, root cause later.
- [ ] **7.10 Blameless postmortem**: write one for the game day: impact, timeline, causes, what went well, action items with owners. Verify: no person is blamed, only system conditions.
- [ ] **7.11 Toil audit**: list manual repetitive tasks you did during this path and automate the top one.

**Checkpoint**: SLO 99.9% means how many minutes of downtime per month, and what do you do when the budget is gone?

---

## Phase 8: Reliability and security

- [ ] **8.1 Resilience patterns**: timeouts, retries with backoff and jitter, circuit breaker using `Microsoft.Extensions.Resilience` or Polly on outbound calls. Verify: a test simulating a failing dependency. Understand: why must retried operations be idempotent?
- [ ] **8.2 Graceful shutdown and zero-downtime deploys**: handle SIGTERM, drain requests, use readiness probes and rolling updates. Verify: deploy under constant load with no failed requests.
- [ ] **8.3 Safe schema changes**: practice expand and contract (add column, dual write, backfill, switch, remove old). Verify: a rename with zero downtime.
- [ ] **8.4 GitHub security features**: CodeQL, secret scanning, push protection, dependency review. Verify: each shows up in the Security tab.
- [ ] **8.5 SBOM and image signing**: generate an SBOM and sign the image with cosign (keyless via OIDC). Verify: signature verifies with `cosign verify`.
- [ ] **8.6 Backups and restore drill**: configure DB backups and actually restore into a new instance. Time it; that is your RTO. Decide your RPO. Verify: data restored and the app works against it.
- [ ] **8.7 Secret rotation**: rotate a Key Vault secret with no downtime.

---

## Phase 9: Kubernetes (optional)

Do this only after the rest. Many systems do not need it; Container Apps covers a lot.

- [ ] **9.1** Concepts: Pod, Deployment, Service, Ingress, ConfigMap, Secret, probes, requests and limits.
- [ ] **9.2** Run the API on a local cluster (kind or minikube).
- [ ] **9.3** Package with Helm.
- [ ] **9.4** Deploy to AKS.
- [ ] **9.5** GitOps with Argo CD or Flux: git as the source of truth.

---

## Milestones

| Milestone | Proof |
|-----------|-------|
| M1 (end of Phase 1) | PRs blocked by failing CI, passing badge in README |
| M2 (end of Phase 2) | Small, non-root, healthy image built in CI |
| M3 (end of Phase 4) | Merge to `master` deploys to Azure with OIDC and an approval gate |
| M4 (end of Phase 5) | Whole environment recreated from code in under 30 minutes |
| M5 (end of Phase 7) | SLO dashboard, burn-rate alerts, runbooks, one postmortem |

Optional certifications as structure, not as a goal: AZ-900, AZ-104, AZ-400, CKA.

## Resources

- Google SRE Book and SRE Workbook (free online).
- *The Phoenix Project*, *The DevOps Handbook*.
- *Implementing Service Level Objectives* (Alex Hidalgo).
- GitHub Actions docs: "Understanding GitHub Actions" and "Security hardening".
- Microsoft Learn: Azure Fundamentals, Container Apps, Bicep.
- Docker docs: "Best practices for writing Dockerfiles".

## Working agreement with Claude

- You write the code and configs. Claude explains, reviews, and asks questions.
- After each step, tell Claude what you did and what you learned. Claude checks your **Understand** answer.
- Never copy a command you cannot explain.

## Start here

Phase 0, step 0.1. When you finish it, tell Claude and move to 0.2.
