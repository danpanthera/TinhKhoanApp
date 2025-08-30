#!/bin/bash

# 🚀 Agribank Lai Châu Dashboard Quick Start Script
# This script starts both frontend and backend for dashboard demo

echo "🏦 Starting Agribank Lai Châu Dashboard System..."
echo "=================================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to check if port is in use
check_port() {
    if lsof -Pi :$1 -sTCP:LISTEN -t >/dev/null ; then
        return 0
    else
        return 1
    fi
}

# Function to start backend
start_backend() {
    echo -e "${BLUE}🔧 Starting Backend (ASP.NET Core)...${NC}"
    cd Backend/KhoanApp.Api
    
    if check_port 5055; then
        echo -e "${YELLOW}⚠️  Backend already running on port 5055${NC}"
    else
        echo -e "${GREEN}✅ Starting backend on https://localhost:5055${NC}"
        dotnet run --urls "https://localhost:5055" &
        BACKEND_PID=$!
        echo "Backend PID: $BACKEND_PID"
    fi
    
    cd ../../
}

# Function to start frontend
start_frontend() {
    echo -e "${BLUE}🎨 Starting Frontend (Vue 3 + Vite)...${NC}"
    cd Frontend/KhoanUI
    
    # Try different ports
    FRONTEND_PORT=3000
    for port in 3000 3001 3002 3003; do
        if ! check_port $port; then
            FRONTEND_PORT=$port
            break
        fi
    done
    
    if check_port $FRONTEND_PORT && [ $FRONTEND_PORT -eq 3000 ]; then
        echo -e "${YELLOW}⚠️  All common ports busy, trying port 3004${NC}"
        FRONTEND_PORT=3004
    fi
    
    echo -e "${GREEN}✅ Starting frontend on http://localhost:$FRONTEND_PORT${NC}"
    npm run dev -- --port $FRONTEND_PORT &
    FRONTEND_PID=$!
    echo "Frontend PID: $FRONTEND_PID"
    
    cd ../../
}

# Function to open demo page
open_demo() {
    echo -e "${BLUE}🌐 Opening Demo Page...${NC}"
    
    # Check operating system and open browser accordingly
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        open "file://$(pwd)/dashboard-demo.html"
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Linux
        xdg-open "file://$(pwd)/dashboard-demo.html"
    elif [[ "$OSTYPE" == "cygwin" ]] || [[ "$OSTYPE" == "msys" ]]; then
        # Windows
        start "file://$(pwd)/dashboard-demo.html"
    fi
}

# Function to show URLs
show_urls() {
    echo ""
    echo -e "${GREEN}🎉 Dashboard System Started Successfully!${NC}"
    echo "=============================================="
    echo -e "${BLUE}📱 Frontend:${NC} http://localhost:$FRONTEND_PORT"
    echo -e "${BLUE}🏪 Backend API:${NC} https://localhost:5055"
    echo -e "${BLUE}📊 Dashboard:${NC} http://localhost:$FRONTEND_PORT/dashboard/business-plan"
    echo -e "${BLUE}🎯 Target Assignment:${NC} http://localhost:$FRONTEND_PORT/dashboard/target-assignment"
    echo -e "${BLUE}🧮 Calculation:${NC} http://localhost:$FRONTEND_PORT/dashboard/calculation"
    echo -e "${BLUE}🎪 Demo Page:${NC} file://$(pwd)/dashboard-demo.html"
    echo ""
    echo -e "${YELLOW}💡 Tips:${NC}"
    echo "  • Use Ctrl+C to stop all services"
    echo "  • Demo page shows static preview"
    echo "  • Main dashboard has interactive features"
    echo "  • Login with demo credentials if needed"
    echo ""
}

# Function to cleanup on exit
cleanup() {
    echo ""
    echo -e "${RED}🛑 Stopping all services...${NC}"
    
    if [ ! -z "$BACKEND_PID" ]; then
        kill $BACKEND_PID 2>/dev/null
        echo "Stopped backend (PID: $BACKEND_PID)"
    fi
    
    if [ ! -z "$FRONTEND_PID" ]; then
        kill $FRONTEND_PID 2>/dev/null
        echo "Stopped frontend (PID: $FRONTEND_PID)"
    fi
    
    # Kill any remaining processes on our ports
    for port in 3000 3001 3002 3003 3004 5055; do
        if check_port $port; then
            echo "Cleaning up port $port..."
            lsof -ti :$port | xargs kill -9 2>/dev/null
        fi
    done
    
    echo -e "${GREEN}✅ Cleanup completed${NC}"
    exit 0
}

# Trap Ctrl+C and call cleanup
trap cleanup INT

# Main execution
echo -e "${BLUE}🔍 Checking prerequisites...${NC}"

# Check if we're in the right directory
if [ ! -d "Frontend/KhoanUI" ] || [ ! -d "Backend/KhoanApp.Api" ]; then
    echo -e "${RED}❌ Error: Please run this script from the project root directory${NC}"
    echo "Current directory: $(pwd)"
    echo "Expected structure:"
    echo "  Frontend/KhoanUI/"
    echo "  Backend/KhoanApp.Api/"
    exit 1
fi

# Check Node.js
if ! command -v node &> /dev/null; then
    echo -e "${RED}❌ Node.js not found. Please install Node.js 16+ first.${NC}"
    exit 1
fi

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK not found. Please install .NET 6+ SDK first.${NC}"
    exit 1
fi

# Check npm dependencies
if [ ! -d "Frontend/KhoanUI/node_modules" ]; then
    echo -e "${YELLOW}📦 Installing npm dependencies...${NC}"
    cd Frontend/KhoanUI
    npm install
    cd ../../
fi

echo -e "${GREEN}✅ Prerequisites check passed${NC}"
echo ""

# Start services
start_backend
sleep 3  # Give backend time to start

start_frontend
sleep 3  # Give frontend time to start

# Open demo page
open_demo

# Show URLs and info
show_urls

# Keep script running
echo -e "${BLUE}📡 Services are running... Press Ctrl+C to stop all services${NC}"
echo ""

# Wait for Ctrl+C
while true; do
    sleep 1
done
