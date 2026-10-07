# SRE / DevOps Learning Path

Practice project: this repository (.NET 10 Clean Architecture API with unit and integration tests).
Every phase ends with something **you build yourself** in this repo. Claude guides, you type.

## How to use this plan

1. Read the "Concepts" of a phase first. No commands before you can explain the problem in your own words.
2. Do the "Build" exercises by hand. Ask Claude for hints, not solutions.
3. Pass the "Checkpoint" without looking at notes. If you cannot, repeat the phase.
4. Keep a `notes/` log: what broke, why, how you fixed it. Debugging stories are the real learning.

Rule of thumb: **understand the manual process first, then automate it.**
Never automate something you have not done by hand at least once.

## Roadmap at a glance

| Phase | Topic | Outcome |
|-------|-------|---------|
| 0 | Foundations | Linux/CLI, Git, networking, HTTP basics |
| 1 | CI with GitHub Actions | Every push builds, tests and gates the code |
| 2 | Docker | The API runs identically anywhere |
| 3 | CD and registries | Images published and versioned automatically |
| 4 | Cloud on Azure | The API is live on the internet |
| 5 | Infrastructure as Code | The whole environment is reproducible from code |
| 6 | Observability | You can see what the system is doing |
| 7 | SRE core | SLIs, SLOs, error budgets, incidents |
| 8 | Reliability and security | Resilience patterns, secrets, supply chain |
| 9 | Kubernetes (optional) | Orchestration concepts |

Suggested pace: 3 to 5 hours per week. Phases 1 to 3 are about 6 to 8 weeks total. Do not rush.

---

## Phase 0: Foundations (1 week, skip what you already know)

**Concepts**
- What DevOps is (culture: dev and ops share ownership) vs what SRE is (engineering discipline applied to operations, with measurable reliability targets).
- The software delivery lifecycle: commit, build, test, package, deploy, operate, observe.
- HTTP, DNS, TCP/IP, TLS, ports, reverse proxies, in plain terms.
- Linux shell basics: processes, files, permissions, environment variables, pipes, exit codes.
- Git beyond basics: branching strategies (trunk-based vs GitFlow), rebase vs merge, protected branches.

**Build**
- Run the API locally and call it with `curl`. Read the status codes and headers.
- Explain the path of a request from browser to your controller.

**Checkpoint**
- Explain what an exit code is and why CI depends on it.
- Explain what happens when you type `https://example.com` in a browser.

**Resources**: *The Phoenix Project* (novel, quick read), Google SRE Book chapters 1 to 3 (free online).

---

## Phase 1: CI with GitHub Actions (2 weeks)

**Concepts**
- Continuous Integration: why we integrate small changes often and fail fast.
- Workflow anatomy: events (`on`), jobs, steps, runners, `uses` vs `run`.
- Jobs run in isolated, ephemeral machines. Nothing persists unless you cache or upload artifacts.
- Secrets vs variables, `GITHUB_TOKEN` and its permissions (least privilege).
- Matrix builds, caching, concurrency, path filters.
- Branch protection and required status checks: CI is only useful if it can block a merge.

**Build** (in this repo, `.github/workflows/`)
1. `ci.yml`: on push and pull request, run `dotnet restore`, `build`, `test`. Make it green.
2. Break a test on purpose in a branch. Watch it fail. Fix it.
3. Add NuGet caching. Measure the time before and after.
4. Split into jobs: `build`, `unit-tests`, `integration-tests`. Understand `needs`.
5. Publish test results and code coverage as an artifact.
6. Add a format/lint check (`dotnet format --verify-no-changes`).
7. Enable branch protection on `master`: PR required, CI required.
8. Add Dependabot for NuGet and for Actions versions.
9. Pin third-party actions to a commit SHA. Understand why (supply chain).

**Checkpoint**
- Why is a failing step marked red but a `continue-on-error` step is not?
- What does `permissions:` do and what is the safest default?
- Why should a PR from a fork not receive your secrets?

**Resources**: official GitHub Actions docs ("Understanding GitHub Actions", "Security hardening").

---

## Phase 2: Docker (2 weeks)

**Concepts**
- Container vs VM. Namespaces and cgroups (what isolation really is).
- Image, layer, container, registry. Layer caching and why instruction order matters.
- Multi-stage builds: SDK image to build, runtime image to run.
- Ports, volumes, networks, environment variables, health checks.
- Running as non-root. Minimal base images. `.dockerignore`.
- The twelve-factor app: config in the environment, logs to stdout, stateless processes.

**Build**
1. Write a `Dockerfile` for the API by hand. Make `docker run` serve a request.
2. Convert it to multi-stage. Compare image sizes.
3. Reorder instructions to maximize layer cache hits. Prove it with rebuild times.
4. Run as a non-root user. Add a `HEALTHCHECK`.
5. Add `.dockerignore`.
6. Write `docker-compose.yml` for the API plus a database (when the project gets one).
7. Add a CI job that builds the image on every PR.

**Checkpoint**
- Why does changing one line of source invalidate the restore layer, and how do you avoid it?
- Where do container logs go and why does that matter?
- Why is `latest` a bad tag in production?

---

## Phase 3: CD and registries (1 to 2 weeks)

**Concepts**
- Continuous Delivery vs Continuous Deployment.
- Immutable artifacts: build once, promote the same image across environments.
- Versioning: SemVer, git SHA tags, conventional commits driving releases.
- Environments, approvals, deployment strategies (rolling, blue/green, canary) at a conceptual level.
- Rollback: the most important deployment feature.

**Build**
1. Push the image to GitHub Container Registry (GHCR) from a workflow on merge to `master`.
2. Tag with git SHA and semantic version.
3. Use GitHub Environments (`staging`, `production`) with a manual approval gate.
4. Authenticate with **OIDC** instead of long-lived credentials (you will reuse this in Phase 4).
5. Scan the image for vulnerabilities (Trivy) and fail on critical findings.

**Checkpoint**
- How do you roll back in under five minutes? Prove it.
- Why is OIDC safer than a stored cloud password?

---

## Phase 4: Cloud on Azure (3 weeks)

**Concepts**
- Cloud model: IaaS, PaaS, serverless. Regions, resource groups, subscriptions.
- Identity: Entra ID, managed identities, RBAC, least privilege.
- Networking basics: VNets, public vs private endpoints, DNS.
- Cost awareness: budgets and alerts **before** creating anything. Delete what you do not use.

**Build** (start with the simplest hosting, then grow)
1. Create a free Azure account, set a **budget alert** first.
2. Deploy the container to **Azure Container Apps** (or App Service for containers) by hand in the portal. Understand each setting.
3. Repeat with the Azure CLI (`az`). Notice how the portal hides details.
4. Connect GitHub Actions to Azure via OIDC (federated credential). Deploy on merge.
5. Move secrets into Azure Key Vault. Read them with a managed identity.
6. Add a real database (Azure SQL or PostgreSQL) and run migrations safely in the pipeline.
7. Configure a custom domain and HTTPS.

**Checkpoint**
- What is a managed identity and what problem does it remove?
- Draw the architecture of your deployment with every network hop.
- What does it cost per month and how would you reduce it?

**Resources**: Microsoft Learn paths for AZ-900 (fundamentals) then AZ-104 or AZ-400 as certification goals.

---

## Phase 5: Infrastructure as Code (2 to 3 weeks)

**Concepts**
- Declarative vs imperative. Desired state, drift, idempotency.
- State management and why state files are sensitive.
- Modules, environments, and reviewing infrastructure changes like code (plan before apply).

**Build**
1. Recreate your Azure setup with **Bicep** (native Azure) or **Terraform** (portable, industry standard). Pick one, learn it well.
2. Add a CI job that runs `plan` or `what-if` on PRs and `apply` on merge with approval.
3. Destroy and recreate the entire environment from code. Time it.
4. Create `staging` and `production` from the same module with different parameters.
5. Detect and fix drift: change something in the portal, then reconcile.

**Checkpoint**
- If the subscription were deleted tonight, how long to rebuild? What is not in code?
- Why must IaC changes go through pull requests?

---

## Phase 6: Observability (3 weeks)

**Concepts**
- Monitoring (known failures) vs observability (asking new questions of the system).
- Three pillars: **logs**, **metrics**, **traces**. Plus events and profiles.
- Structured logging, correlation IDs, log levels, never logging PII or secrets.
- RED (Rate, Errors, Duration) and USE (Utilization, Saturation, Errors) methods.
- OpenTelemetry as the vendor-neutral standard.

**Build**
1. Add structured logging (Serilog or built-in `ILogger` with JSON output) to the API.
2. Add health endpoints: liveness vs readiness. Understand the difference.
3. Instrument with **OpenTelemetry**: traces and metrics.
4. Run Prometheus and Grafana locally with Docker Compose. Build a dashboard with RED metrics.
5. Send telemetry to Azure Monitor / Application Insights in the cloud.
6. Create alerts that are **actionable** (someone must do something when they fire).

**Checkpoint**
- A user says "it was slow at 3pm". Show how you find the cause using only your telemetry.
- Why is alerting on CPU usually worse than alerting on user-facing symptoms?

---

## Phase 7: SRE core (3 to 4 weeks)

This is the heart of the discipline. Read the Google SRE Book and the SRE Workbook in parallel.

**Concepts**
- **SLI**: a measurable indicator (for example, percentage of requests under 300 ms with a non-5xx status).
- **SLO**: the target for that SLI (for example, 99.5% over 30 days).
- **Error budget**: `1 - SLO`. It is the allowed unreliability and decides whether to ship features or fix reliability.
- SLA vs SLO: contract vs internal target.
- Toil: manual, repetitive, automatable work. Measure and eliminate it.
- Incident management: roles, severity levels, communication, mitigation before root cause.
- Blameless postmortems: focus on systems and process, never on people.
- Capacity planning, load testing, and graceful degradation.

**Build**
1. Define 2 SLIs and SLOs for this API. Write them down in `docs/slo.md`.
2. Implement them as dashboards and **burn-rate alerts** (fast burn and slow burn).
3. Load test with k6. Find the breaking point and record it.
4. Run a **game day**: inject a failure (kill the container, break the DB connection, add latency) and practice the incident process.
5. Write a real blameless postmortem for that exercise: timeline, impact, causes, action items.
6. Write a runbook for each alert. An alert without a runbook is noise.

**Checkpoint**
- Your SLO is 99.9%. How many minutes of downtime per month is that? What do you do when the budget is spent?
- Why is 100% reliability the wrong target?

**Resources**: *Site Reliability Engineering* (Google, free), *The Site Reliability Workbook* (free), *Implementing Service Level Objectives* (Alex Hidalgo).

---

## Phase 8: Reliability and security (ongoing)

**Concepts**
- Resilience patterns: timeouts, retries with backoff and jitter, circuit breakers, bulkheads, idempotency.
- Health checks, graceful shutdown, zero-downtime deployments, database migration strategies (expand and contract).
- DevSecOps: shift-left security, SAST, dependency scanning, secret scanning, SBOM, image signing.
- Backup and restore: a backup you never restored is not a backup.

**Build**
1. Add Polly (or `Microsoft.Extensions.Resilience`) to outbound calls and test failure behavior.
2. Enable CodeQL, secret scanning and dependency review in GitHub.
3. Generate an SBOM and sign images (cosign).
4. Perform a real restore drill of the database. Time it (this is your RTO).
5. Define your RPO and RTO and check that reality matches.

**Checkpoint**
- What is idempotency and why do retries require it?
- Explain the expand/contract pattern for a column rename with zero downtime.

---

## Phase 9: Kubernetes (optional, only after everything above)

**Concepts**: Pods, Deployments, Services, Ingress, ConfigMaps, Secrets, probes, resource requests and limits, Helm, GitOps (Argo CD or Flux).

**Build**: run the API on a local cluster (kind or minikube), then on AKS. Deploy with Helm and GitOps.

**Warning**: Kubernetes is a heavy tool. Many systems do not need it. Container Apps covers a lot of real-world cases. Learn it to make an informed choice, not because it is trendy.

---

## Milestones and portfolio

| Milestone | Proof |
|-----------|-------|
| M1 | PRs blocked by failing CI, green pipeline badge in README |
| M2 | Image under 150 MB, non-root, health check, published to GHCR |
| M3 | One-click deploy to Azure with OIDC and approval gate |
| M4 | Entire environment recreated from IaC in under 30 minutes |
| M5 | Dashboard, SLOs, burn-rate alerts, and one written postmortem |

Optional certifications as structure, not as the goal: AZ-900, AZ-104, AZ-400, CKA.

## Working agreement with Claude

- You write the code and configs. Claude explains concepts, reviews, and asks questions.
- When stuck, ask for a hint first, then a bigger hint, and the solution last.
- After each phase, Claude reviews your work against the checkpoint.
- Never copy a command you cannot explain.

## Next step

Start Phase 0, then Phase 1, step 1: create `.github/workflows/ci.yml` yourself and make `dotnet test` run on every push.
