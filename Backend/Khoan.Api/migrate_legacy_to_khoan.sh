#!/usr/bin/env bash
set -euo pipefail

# Always operate from the directory where this script lives
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"
# Migrate legacy TinhKhoanApp.Api source into Khoan.Api (Option 2)
# Steps:
# 1. Copy required directories
# 2. Replace namespaces TinhKhoanApp.Api -> Khoan.Api
# 3. Remove legacy Program* files
# 4. Add minimal README marker

LEGACY_ROOT="../TinhKhoanApp.Api"   # Relative to Backend/Khoan.Api
TARGET_ROOT="."                     # Current = Backend/Khoan.Api

DIRS=( Data Models DTOs Repositories Services Extensions Filters Middleware Converters HealthChecks Helpers Utils Controllers )

echo "[INFO] Starting legacy source migration (Option 2)"
echo "[INFO] Legacy root: $(realpath "$LEGACY_ROOT" 2>/dev/null || echo "$LEGACY_ROOT (not found)")"
echo "[INFO] Target root: $(pwd)"

for d in "${DIRS[@]}"; do
  SRC="$LEGACY_ROOT/$d"
  if [ -d "$SRC" ]; then
    echo "[COPY] $d -> $TARGET_ROOT/$d"
    mkdir -p "$TARGET_ROOT/$d"
    rsync -a --delete \
      --exclude '*TEMP_DISABLED*' \
      --exclude 'bin' \
      --exclude 'obj' \
      --exclude '*.mdf' \
      --exclude '*.ldf' \
      "$SRC/" "$TARGET_ROOT/$d/" || true
  else
    echo "[WARN] Missing in legacy: $d (expected at $SRC)"
  fi
done

echo "[INFO] Namespace replacement in copied C# files (TinhKhoanApp.Api -> Khoan.Api)"

# Namespace replacement
find $TARGET_ROOT -maxdepth 6 -type f -name '*.cs' -print0 | xargs -0 sed -i '' 's/TinhKhoanApp.Api/Khoan.Api/g'

# Remove legacy program variants if accidentally copied
rm -f Program.clean.cs Program.minimal.cs Program.cs.orig 2>/dev/null || true

# Marker README
cat > SOURCE_MIGRATION_README.md <<EOF
This folder now contains migrated source from legacy TinhKhoanApp.Api (Option 2 consolidation).
Next steps after successful build & tests:
1. Remove TinhKhoanApp.Api project from solution.
2. Delete legacy folder after final verification.
3. Update docs & scripts to reference Khoan.Api only.
EOF

echo "[DONE] Migration script completed local copy + namespace update. Now run: dotnet build ../Backend.sln"
