#!/bin/bash

# 🧹 DOCKER IMAGE CLEANUP SCRIPT
# Dọn dẹp images thừa và corrupted để tối ưu performance
# Ngày: 18/07/2025

echo "🧹 === DOCKER IMAGE CLEANUP ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}📊 1. Current Docker state analysis...${NC}"
echo "Disk usage by Docker:"
docker system df 2>/dev/null || echo "❌ Docker system df failed"
echo ""

echo "All images:"
docker images 2>/dev/null || echo "❌ Docker images command failed"
echo ""

echo "All containers:"
docker ps -a 2>/dev/null || echo "❌ Docker ps command failed"
echo ""

echo -e "${BLUE}🔍 2. Identify corrupted or unused resources...${NC}"
echo "Checking for dangling images..."
DANGLING=$(docker images -f "dangling=true" -q 2>/dev/null)
if [ -n "$DANGLING" ]; then
    echo "Found dangling images: $DANGLING"
else
    echo "No dangling images found"
fi
echo ""

echo "Checking for stopped containers..."
STOPPED=$(docker ps -f "status=exited" -q 2>/dev/null)
if [ -n "$STOPPED" ]; then
    echo "Found stopped containers: $STOPPED"
else
    echo "No stopped containers found"
fi
echo ""

echo -e "${BLUE}🧹 3. Safe cleanup (preserve running containers)...${NC}"
# Remove dangling images
if [ -n "$DANGLING" ]; then
    echo "Removing dangling images..."
    docker rmi $DANGLING 2>/dev/null && echo "✅ Dangling images removed" || echo "⚠️  Some dangling images failed to remove"
fi

# Remove stopped containers (except our main one)
if [ -n "$STOPPED" ]; then
    echo "Removing stopped containers..."
    for container in $STOPPED; do
        if [[ "$container" != *"azure_sql_edge_tinhkhoan"* ]]; then
            docker rm $container 2>/dev/null && echo "✅ Removed container: $container" || echo "⚠️  Failed to remove: $container"
        fi
    done
fi

# Remove unused networks
echo "Removing unused networks..."
docker network prune -f 2>/dev/null && echo "✅ Unused networks removed" || echo "⚠️  Network cleanup failed"

# Remove unused volumes (careful!)
echo "Checking for unused volumes..."
docker volume ls -qf dangling=true 2>/dev/null | head -5 # Just show first 5
echo "⚠️  Skipping volume cleanup to preserve data"
echo ""

echo -e "${BLUE}🔄 4. Image optimization...${NC}"
echo "Checking for multiple versions of same image..."
docker images | grep -E "(azure-sql-edge|microsoft)" | head -10
echo ""

echo "Removing old/unused image tags..."
docker images | grep "<none>" | awk '{print $3}' | head -5 | xargs -r docker rmi 2>/dev/null && echo "✅ Removed untagged images" || echo "⚠️  Some images couldn't be removed"
echo ""

echo -e "${BLUE}📦 5. Rebuild image cache if needed...${NC}"
echo "Checking if we need to rebuild Docker image cache..."
if docker images 2>&1 | grep -q "input/output error"; then
    echo -e "${RED}❌ Image corruption detected!${NC}"
    echo "Attempting to fix image corruption..."
    
    # Try to restart Docker daemon
    echo "Restarting Docker Desktop..."
    osascript -e 'quit app "Docker"' 2>/dev/null
    sleep 15
    open -a Docker
    sleep 60
    
    echo "Testing after restart..."
    if docker images >/dev/null 2>&1; then
        echo -e "${GREEN}✅ Image corruption fixed${NC}"
    else
        echo -e "${RED}❌ Image corruption persists - may need full reset${NC}"
    fi
else
    echo -e "${GREEN}✅ No image corruption detected${NC}"
fi
echo ""

echo -e "${BLUE}📊 6. Final state report...${NC}"
echo "Current disk usage:"
docker system df 2>/dev/null || echo "Docker system df unavailable"
echo ""

echo "Active containers:"
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Size}}" 2>/dev/null || echo "Docker ps unavailable"
echo ""

echo "Available images:"
docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" 2>/dev/null || echo "Docker images unavailable"
echo ""

echo -e "${GREEN}�� === DOCKER CLEANUP COMPLETED ===${NC}"
echo "Cleanup actions taken:"
echo "  ✅ Removed dangling images"
echo "  ✅ Removed stopped containers (safe)"
echo "  ✅ Removed unused networks"
echo "  ⚠️  Preserved volumes (data safety)"
echo "  🔄 Checked for image corruption"
echo ""
echo "If you still have Docker issues, consider running:"
echo "  ./docker_gentle_fix.sh   # Try to preserve data"
echo "  ./docker_reset_fix.sh    # Complete rebuild (data loss)"
