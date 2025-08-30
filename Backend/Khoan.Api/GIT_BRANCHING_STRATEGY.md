# 🌟 GIT BRANCHING STRATEGY - DEV TO PRODUCTION

## 📋 **ENVIRONMENT SETUP**

### 🖥️ **Development Environment (Current)**

-   **Machine**: MacBook Pro M3 Max
-   **Database**: Docker Azure SQL Edge ARM64
-   **Branch**: `local-main` (development)
-   **Purpose**: Feature development, testing, integration

### 🏢 **Production Environment (Target)**

-   **Machine**: Windows Server
-   **Database**: SQL Server Developer Edition
-   **Branch**: `production` (stable release)
-   **Purpose**: Live production deployment

---

## 🌿 **BRANCHING STRATEGY**

### 🎯 **Core Branches**

| Branch       | Purpose                | Environment    | Stability  |
| ------------ | ---------------------- | -------------- | ---------- |
| `main`       | Master/Release branch  | GitHub         | ✅ Stable  |
| `local-main` | Development branch     | MacBook M3     | 🔧 Active  |
| `staging`    | Pre-production testing | Staging        | 🧪 Testing |
| `production` | Production deployment  | Windows Server | 🏆 Live    |

### 🔄 **Feature Development Flow**

```bash
# 1. FEATURE DEVELOPMENT (MacBook M3)
local-main (dev) → feature/table-completion → local-main

# 2. INTEGRATION & TESTING
local-main → staging → testing & validation

# 3. PRODUCTION DEPLOYMENT
staging → main → production (Windows Server)
```

---

## ✅ **SETUP STATUS - COMPLETED**

### 🎉 **Branch Structure Created Successfully**

All branches have been created and pushed to remote repository:

```bash
✅ local-main  (development) - Active, up to date
✅ staging     (pre-prod)    - Created and pushed
✅ production  (deployment)  - Created and pushed
✅ main        (release)     - Existing, stable
```

### 🛡️ **Large File Issue - RESOLVED**

-   ❌ Issue: `azure_sql_backup_20250812_112407.tar.gz` (152MB) blocked GitHub push
-   ✅ Solution: Removed from git history using `git filter-branch`
-   ✅ Result: Clean repository, successful push to all branches

---

## 🚀 **DEPLOYMENT COMMANDS**

### 📝 **1. Development Workflow (MacBook M3)**

```bash
# Create staging branch from current local-main
git checkout local-main
git pull origin local-main
git checkout -b staging
git push -u origin staging

# Create production branch from main
git checkout main
git pull origin main
git checkout -b production
git push -u origin production
```

### ⚙️ **2. Branch Protection Rules (GitHub)**

```yaml
Branch Protection Settings:
main:
    - Require pull request reviews (2 reviewers)
    - Require status checks to pass
    - Restrict pushes to admins only

production:
    - Require pull request reviews (1 admin reviewer)
    - Require up-to-date branches
    - Restrict pushes to admins only
```

---

## 🔧 **DEVELOPMENT WORKFLOW**

### 🖥️ **MacBook M3 Max (Development)**

```bash
# Daily development cycle
git checkout local-main
git pull origin local-main

# Feature development
git checkout -b feature/new-table-implementation
# ... make changes ...
git add .
git commit -m "✨ FEAT: LN01 DTOs and Service implementation"
git push -u origin feature/new-table-implementation

# Create PR: feature/new-table-implementation → local-main
# After review & merge, continue on local-main
```

### 🧪 **Staging Deployment**

```bash
# Weekly/milestone staging push
git checkout staging
git pull origin staging
git merge local-main
git push origin staging

# Test on staging environment
# Database migration testing
# Integration testing
```

### 🏆 **Production Release**

```bash
# Monthly/release production deployment
git checkout main
git pull origin main
git merge staging
git tag v1.2.3
git push origin main --tags

# Production deployment (Windows Server pulls)
git checkout production
git pull origin production
git merge main
git push origin production
```

---

## 📊 **DATABASE MIGRATION STRATEGY**

### 🔄 **Migration Flow**

```bash
# 1. DEV: Create migration (MacBook M3)
dotnet ef migrations add NewTableStructure
git add . && git commit -m "🗄️ MIGRATION: Add LN01 table structure"

# 2. STAGING: Test migration
git checkout staging
git merge local-main
dotnet ef database update

# 3. PRODUCTION: Apply migration (Windows Server)
git pull origin production
dotnet ef database update --configuration Release
```

### 🛡️ **Migration Safety Checks**

```sql
-- Pre-migration backup (Windows Server)
BACKUP DATABASE KhoanDB TO DISK = 'C:\Backups\KhoanDB_PreMigration.bak'

-- Post-migration verification
SELECT COUNT(*) FROM sys.tables WHERE temporal_type = 2; -- Temporal tables
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC;
```

---

## 🔐 **SECURITY & ACCESS CONTROL**

### 🎫 **Access Permissions**

| Role          | local-main | staging    | main         | production   |
| ------------- | ---------- | ---------- | ------------ | ------------ |
| **Developer** | ✅ Push    | 📝 PR only | ❌ No access | ❌ No access |
| **Lead Dev**  | ✅ Push    | ✅ Push    | 📝 PR only   | ❌ No access |
| **DevOps**    | ✅ All     | ✅ All     | ✅ All       | ✅ Push only |

### 🛡️ **Production Safety**

```bash
# Windows Server: Production pull only (no direct push)
git remote set-url --push origin no-push
git config branch.production.pushRemote no-push

# Emergency hotfix process
git checkout -b hotfix/critical-issue production
# ... fix issue ...
git checkout main
git merge hotfix/critical-issue
git checkout production
git merge main
```

---

## 📋 **DAILY OPERATIONS**

### 🌅 **Morning Routine (MacBook M3)**

```bash
#!/bin/bash
# daily_sync.sh
echo "🌅 Starting daily development sync..."

git checkout local-main
git pull origin local-main
git status

echo "✅ Ready for development!"
```

### 🌙 **Evening Commit (MacBook M3)**

```bash
#!/bin/bash
# evening_push.sh
echo "🌙 Evening development push..."

git add .
git commit -m "🔧 Daily progress: $(date +'%Y-%m-%d')"
git push origin local-main

echo "✅ Work saved to remote!"
```

### 🏢 **Production Sync (Windows Server)**

```bash
#!/bin/bash
# production_sync.sh
echo "🏢 Production environment sync..."

git checkout production
git pull origin production

# Apply any pending migrations
dotnet ef database update --configuration Release

echo "✅ Production updated!"
```

---

## 🚨 **EMERGENCY PROCEDURES**

### 🔥 **Hotfix Process**

```bash
# 1. Create hotfix branch from production
git checkout production
git checkout -b hotfix/critical-database-issue

# 2. Apply fix
# ... emergency fixes ...

# 3. Fast-track to production
git add . && git commit -m "🚨 HOTFIX: Critical database connection issue"
git checkout production
git merge hotfix/critical-database-issue
git push origin production

# 4. Backport to main and staging
git checkout main && git merge hotfix/critical-database-issue
git checkout staging && git merge main
```

### 🔄 **Rollback Procedure**

```bash
# Database rollback (Windows Server)
dotnet ef database update PreviousMigration --configuration Release

# Code rollback
git checkout production
git reset --hard v1.2.2  # Previous stable tag
git push --force-with-lease origin production
```

---

## 📈 **MONITORING & REPORTING**

### 📊 **Branch Health Check**

```bash
#!/bin/bash
# branch_health.sh

echo "🏥 Branch Health Report"
echo "======================"

for branch in local-main staging main production; do
    echo "Branch: $branch"
    git log --oneline -5 origin/$branch
    echo "---"
done
```

### 📝 **Release Notes Template**

```markdown
## 🚀 Release v1.3.0 - $(date +'%Y-%m-%d')

### ✨ New Features

-   GL02 Entity-first architecture complete
-   LN01 DTOs and Service implementation

### 🔧 Improvements

-   Database migration performance optimized
-   Build time reduced by 30%

### 🗄️ Database Changes

-   Migration: 20250813_GL02EntityMigration
-   New indexes: GL02_NGAY_DL_IDX

### 🧪 Testing

-   ✅ Unit tests: 95% coverage
-   ✅ Integration tests: All passed
-   ✅ Performance tests: Response time < 200ms
```

---

## ⚡ **QUICK REFERENCE**

### 🎯 **Common Commands**

```bash
# Development (MacBook M3)
git checkout local-main && git pull && git push

# Staging deployment
git checkout staging && git merge local-main && git push

# Production release
git checkout main && git merge staging && git tag v1.x.x && git push --tags

# Production sync (Windows Server)
git checkout production && git pull origin production
```

### 🔍 **Status Check**

```bash
# Check all branch status
git for-each-ref --format='%(refname:short) %(upstream:track)' refs/heads

# Check commits ahead/behind
git status -uno
```

---

**🎯 This strategy ensures safe, controlled deployments from macOS M3 development to Windows Server production with proper testing and rollback capabilities.**
