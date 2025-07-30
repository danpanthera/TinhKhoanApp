#!/bin/bash

# Màu sắc cho output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== TIỆN ÍCH HỖ TRỢ PHÁT TRIỂN TINHKHOANAPP ===${NC}"

show_menu() {
    echo -e "\n${GREEN}Chọn một tùy chọn:${NC}"
    echo -e "${YELLOW}1.${NC} Chạy dev server frontend"
    echo -e "${YELLOW}2.${NC} Chạy dev server backend"
    echo -e "${YELLOW}3.${NC} Build frontend"
    echo -e "${YELLOW}4.${NC} Build backend"
    echo -e "${YELLOW}5.${NC} Kiểm tra chất lượng code"
    echo -e "${YELLOW}6.${NC} Hiển thị hướng dẫn phát triển component"
    echo -e "${YELLOW}7.${NC} Kiểm tra type errors"
    echo -e "${YELLOW}8.${NC} Sửa lỗi định dạng code"
    echo -e "${YELLOW}9.${NC} Clean & reinstall frontend"
    echo -e "${YELLOW}10.${NC} Restore packages backend"
    echo -e "${YELLOW}11.${NC} Tìm kiếm trong codebase"
    echo -e "${YELLOW}12.${NC} Hiển thị CODE_QUALITY_GUIDE.md"
    echo -e "${YELLOW}13.${NC} Hiển thị DEVELOPER_GUIDE.md"
    echo -e "${YELLOW}0.${NC} Thoát"
    echo -e "\nNhập lựa chọn của bạn: "
}

run_option() {
    case $1 in
        1)
            echo -e "${BLUE}Đang khởi động dev server frontend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            ./start_frontend.sh
            ;;
        2)
            echo -e "${BLUE}Đang khởi động dev server backend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
            ./start_backend.sh
            ;;
        3)
            echo -e "${BLUE}Đang build frontend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            npm run build
            ;;
        4)
            echo -e "${BLUE}Đang build backend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
            dotnet build
            ;;
        5)
            echo -e "${BLUE}Đang kiểm tra chất lượng code...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            ./code-quality-report.sh
            ;;
        6)
            echo -e "${BLUE}Hiển thị hướng dẫn phát triển component...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            ./component-guide.sh
            ;;
        7)
            echo -e "${BLUE}Đang kiểm tra type errors...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            npm run type-check
            ;;
        8)
            echo -e "${BLUE}Đang sửa lỗi định dạng code...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            npm run lint -- --fix
            ;;
        9)
            echo -e "${BLUE}Đang clean & reinstall frontend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            npm run clean-all
            npm install
            ;;
        10)
            echo -e "${BLUE}Đang restore packages backend...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
            dotnet restore
            ;;
        11)
            echo -e "${BLUE}Tìm kiếm trong codebase${NC}"
            read -p "Nhập từ khóa cần tìm: " search_term
            echo -e "${BLUE}Đang tìm kiếm '$search_term' trong codebase...${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp
            grep -r "$search_term" --include="*.vue" --include="*.js" --include="*.ts" --include="*.cs" --color=always .
            ;;
        12)
            echo -e "${BLUE}Hiển thị CODE_QUALITY_GUIDE.md${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            cat CODE_QUALITY_GUIDE.md | less
            ;;
        13)
            echo -e "${BLUE}Hiển thị DEVELOPER_GUIDE.md${NC}"
            cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite
            cat DEVELOPER_GUIDE.md | less
            ;;
        0)
            echo -e "${GREEN}Cảm ơn bạn đã sử dụng tiện ích!${NC}"
            exit 0
            ;;
        *)
            echo -e "${RED}Lựa chọn không hợp lệ!${NC}"
            ;;
    esac
}

# Main program
if [[ $1 ]]; then
    run_option $1
else
    while true; do
        show_menu
        read choice
        run_option $choice
        echo -e "\n${YELLOW}Nhấn Enter để tiếp tục...${NC}"
        read
    done
fi
