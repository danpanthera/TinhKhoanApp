#!/bin/bash
# master_control.sh
# Script điều khiển chính cho TinhKhoanApp - KHÔNG BỊ TREO!

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m'

show_menu() {
    echo -e "${PURPLE}🎮 TinhKhoanApp Master Control${NC}"
    echo -e "${PURPLE}=============================${NC}"
    echo -e "${GREEN}1. 🚀 Start Full App (No Hang)${NC}"
    echo -e "${GREEN}2. 🔍 Check Status${NC}"
    echo -e "${GREEN}3. 🚨 Emergency Stop${NC}"
    echo -e "${GREEN}4. 🔄 Restart All${NC}"
    echo -e "${GREEN}5. 🧪 Test GL01 Import${NC}"
    echo -e "${GREEN}6. 🔧 Fix Warnings${NC}"
    echo -e "${GREEN}7. 📊 Show Logs${NC}"
    echo -e "${GREEN}8. 🏥 Health Check${NC}"
    echo -e "${RED}9. 🚪 Exit${NC}"
    echo -e "${PURPLE}=============================${NC}"
}

start_app() {
    echo -e "${YELLOW}🚀 Starting TinhKhoanApp...${NC}"
    ./start_full_app.sh
}

check_status() {
    echo -e "${YELLOW}🔍 Checking status...${NC}"
    ./check_status.sh
}

emergency_stop() {
    echo -e "${RED}🚨 Emergency stop...${NC}"
    ./emergency_stop.sh
}

restart_all() {
    echo -e "${YELLOW}🔄 Restarting all services...${NC}"
    ./emergency_stop.sh
    sleep 3
    ./start_full_app.sh
}

test_gl01() {
    echo -e "${YELLOW}🧪 Testing GL01 import...${NC}"
    curl -s http://localhost:5055/health && echo " - Backend OK" || echo " - Backend not responding"
    curl -s http://localhost:5055/api/direct-import/test && echo " - GL01 endpoint OK" || echo " - GL01 endpoint issue"
}

fix_warnings() {
    echo -e "${YELLOW}🔧 Fixing warnings...${NC}"
    ./fix_warnings.sh
}

show_logs() {
    echo -e "${YELLOW}📊 Recent logs:${NC}"
    echo -e "${BLUE}Backend log:${NC}"
    tail -10 backend.log 2>/dev/null || echo "No backend log"
    echo -e "${BLUE}Frontend log:${NC}"
    tail -10 frontend_startup.log 2>/dev/null || echo "No frontend log"
}

health_check() {
    echo -e "${YELLOW}🏥 Health check...${NC}"

    # Database
    if docker ps --filter "name=azure_sql_edge_tinhkhoan" --format "{{.Names}}" | grep -w "azure_sql_edge_tinhkhoan" > /dev/null; then
        echo -e "${GREEN}✅ Database: Healthy${NC}"
    else
        echo -e "${RED}❌ Database: Unhealthy${NC}"
    fi

    # Backend
    if curl -s --max-time 3 http://localhost:5055/health > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Backend: Healthy (http://localhost:5055)${NC}"
    else
        echo -e "${RED}❌ Backend: Unhealthy${NC}"
    fi

    # Frontend
    if curl -s --max-time 3 http://localhost:3000 > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Frontend: Healthy (http://localhost:3000)${NC}"
    else
        echo -e "${RED}❌ Frontend: Unhealthy${NC}"
    fi
}

# Main loop
while true; do
    show_menu
    echo -n "Choose option [1-9]: "
    read choice

    case $choice in
        1) start_app ;;
        2) check_status ;;
        3) emergency_stop ;;
        4) restart_all ;;
        5) test_gl01 ;;
        6) fix_warnings ;;
        7) show_logs ;;
        8) health_check ;;
        9) echo -e "${GREEN}👋 Goodbye!${NC}"; exit 0 ;;
        *) echo -e "${RED}Invalid option!${NC}" ;;
    esac

    echo -e "\n${YELLOW}Press Enter to continue...${NC}"
    read
    clear
done
