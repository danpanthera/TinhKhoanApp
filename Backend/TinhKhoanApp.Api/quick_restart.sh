#!/bin/bash
# Quick restart script

echo "🔄 QUICK RESTART - NO HANGING!"
echo "==============================="

# Cleanup first
./emergency_cleanup.sh

# Wait a moment
sleep 2

# Start everything
./start_full_app.sh
