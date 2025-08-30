#!/bin/bash
# 🚀 KhoanApp - Full Stack Launcher
# Usage: ./launch_app.sh (from project root)

echo "🚀 KhoanApp - Full Stack Startup"
echo "======================================"

# Kiểm tra project root
if [ ! -d "Backend" ] || [ ! -d "Frontend" ]; then
    echo "❌ Error: Not in project root directory!"
    echo "💡 Please run this script from KhoanApp root directory"
    exit 1
fi

echo "🎯 Choose startup option:"
echo "1) Start Backend only"
echo "2) Start Frontend only" 
echo "3) Start Both (Full Stack)"
echo "4) Exit"
echo ""
read -p "Enter your choice (1-4): " choice

case $choice in
    1)
        echo "🚀 Starting Backend..."
        cd Backend/KhoanApp.Api
        chmod +x start_backend.sh
        ./start_backend.sh
        ;;
    2)
        echo "🎨 Starting Frontend..."
        cd Frontend/KhoanUI
        chmod +x start_frontend.sh
        ./start_frontend.sh
        ;;
    3)
        echo "🚀 Starting Full Stack Application..."
        
        # Start Backend in background
        echo "🔧 Starting Backend..."
        cd Backend/KhoanApp.Api
        chmod +x start_backend.sh
        ./start_backend.sh &
        BACKEND_PID=$!
        
        # Wait a bit for backend to start
        sleep 5
        
        # Start Frontend
        echo "🎨 Starting Frontend..."
        cd ../../Frontend/KhoanUI
        chmod +x start_frontend.sh
        ./start_frontend.sh
        
        # Kill backend when frontend stops
        kill $BACKEND_PID 2>/dev/null
        ;;
    4)
        echo "👋 Goodbye!"
        exit 0
        ;;
    *)
        echo "❌ Invalid choice!"
        exit 1
        ;;
esac
