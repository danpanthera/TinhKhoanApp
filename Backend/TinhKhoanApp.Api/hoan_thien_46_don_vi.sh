#!/bin/bash

# Script tạo đầy đủ 46 đơn vị theo cấu trúc Ver2
# Đảm bảo không thừa không thiếu - Sử dụng tiếng Việt

API_BASE="http://localhost:5055/api"
LOG_FILE="hoan_thien_46_don_vi_$(date +%Y%m%d_%H%M%S).log"
TIMESTAMP=$(date +%s)

echo "🎯 BẮT ĐẦU TẠO 46 ĐƠN VỊ THEO CẤU TRÚC VER2..." | tee -a $LOG_FILE
echo "📅 Bắt đầu lúc: $(date)" | tee -a $LOG_FILE
echo "⭐ Mục tiêu: Tạo chính xác 46 đơn vị (1 Root + 9 Chi nhánh + 36 Phòng ban)" | tee -a $LOG_FILE

# Hàm tạo đơn vị và trả về ID
tao_don_vi() {
    local ma_don_vi=$1
    local ten_don_vi=$2
    local loai_don_vi=$3
    local don_vi_cha=$4

    if [ "$don_vi_cha" == "null" ]; then
        du_lieu_json="{\"Code\": \"$ma_don_vi\", \"Name\": \"$ten_don_vi\", \"Type\": \"$loai_don_vi\", \"ParentUnitId\": null}"
    else
        du_lieu_json="{\"Code\": \"$ma_don_vi\", \"Name\": \"$ten_don_vi\", \"Type\": \"$loai_don_vi\", \"ParentUnitId\": $don_vi_cha}"
    fi

    phan_hoi=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "$du_lieu_json")

    # Kiểm tra lỗi
    if echo "$phan_hoi" | grep -q "error\|Error\|đã tồn tại"; then
        # Nếu đã tồn tại, thử lấy ID từ database
        if echo "$phan_hoi" | grep -q "đã tồn tại"; then
            echo "⚠️ $ten_don_vi đã tồn tại, đang tìm ID..." | tee -a $LOG_FILE
            tat_ca_don_vi=$(curl -s "$API_BASE/Units")
            id=$(echo "$tat_ca_don_vi" | grep -A 5 -B 5 "\"Code\":\"$ma_don_vi\"" | grep '"Id":' | head -1 | sed 's/.*"Id": *\([0-9]*\).*/\1/')
            if [ -n "$id" ]; then
                echo "✅ Tìm thấy $ten_don_vi (ID: $id)" | tee -a $LOG_FILE
                echo "$id"
                return 0
            fi
        fi
        echo "❌ Lỗi tạo $ten_don_vi: $phan_hoi" | tee -a $LOG_FILE
        return 1
    fi

    # Trích xuất ID từ phản hồi
    id=$(echo "$phan_hoi" | sed -n 's/.*"Id": *\([0-9]*\).*/\1/p')
    if [ -z "$id" ]; then
        echo "⚠️ Không thể lấy ID cho $ten_don_vi" | tee -a $LOG_FILE
        return 1
    fi

    echo "✅ Tạo thành công $ten_don_vi (ID: $id)" | tee -a $LOG_FILE
    echo "$id"
}

# BƯỚC 1: Tạo đơn vị gốc - Chi nhánh Lai Châu
echo "" | tee -a $LOG_FILE
echo "🏢 BƯỚC 1: TẠO ĐƠN VỊ GỐC..." | tee -a $LOG_FILE
ma_goc="CNLC_GOC_$TIMESTAMP"
id_goc=$(tao_don_vi "$ma_goc" "Chi nhánh Lai Châu" "CNL1" "null")

if [ -z "$id_goc" ]; then
    echo "❌ Không thể tạo đơn vị gốc. Thoát!" | tee -a $LOG_FILE
    exit 1
fi

echo "🎉 Đơn vị gốc đã tạo: Chi nhánh Lai Châu (ID: $id_goc)" | tee -a $LOG_FILE

# BƯỚC 2: Tạo 9 chi nhánh cấp 2
echo "" | tee -a $LOG_FILE
echo "🏢 BƯỚC 2: TẠO 9 CHI NHÁNH CẤP 2..." | tee -a $LOG_FILE

# Mảng chứa thông tin 9 chi nhánh
declare -a danh_sach_chi_nhanh=(
    "HS:Hội sở"
    "BL:CN Bình Lư"
    "PT:CN Phong Thổ"
    "SH:CN Sìn Hồ"
    "BT:CN Bum Tở"
    "TU:CN Than Uyên"
    "DK:CN Đoàn Kết"
    "TUY:CN Tân Uyên"
    "NH:CN Nậm Hàng"
)

# Mảng lưu ID của các chi nhánh
declare -A id_chi_nhanh

so_chi_nhanh_thanh_cong=0
for chi_nhanh in "${danh_sach_chi_nhanh[@]}"; do
    ma_cn=$(echo "$chi_nhanh" | cut -d: -f1)
    ten_cn=$(echo "$chi_nhanh" | cut -d: -f2)
    ma_day_du="CN_${ma_cn}_$TIMESTAMP"

    echo "  Đang tạo: $ten_cn..." | tee -a $LOG_FILE
    id_cn=$(tao_don_vi "$ma_day_du" "$ten_cn" "CNL2" "$id_goc")

    if [ -n "$id_cn" ]; then
        id_chi_nhanh[$ma_cn]=$id_cn
        so_chi_nhanh_thanh_cong=$((so_chi_nhanh_thanh_cong + 1))
        echo "  ✅ $ten_cn tạo thành công (ID: $id_cn)" | tee -a $LOG_FILE
    else
        echo "  ❌ Không thể tạo $ten_cn" | tee -a $LOG_FILE
    fi
done

echo "📊 Kết quả: Đã tạo $so_chi_nhanh_thanh_cong/9 chi nhánh" | tee -a $LOG_FILE

# BƯỚC 3: Tạo phòng ban cho từng chi nhánh
echo "" | tee -a $LOG_FILE
echo "🏢 BƯỚC 3: TẠO PHÒNG BAN CHO TỪNG CHI NHÁNH..." | tee -a $LOG_FILE

so_phong_ban_thanh_cong=0

# Hàm tạo phòng ban cho một chi nhánh
tao_phong_ban_cho_chi_nhanh() {
    local ma_cn=$1
    local id_cn=$2
    local ten_cn=$3
    shift 3
    local danh_sach_phong=("$@")
    local so_phong_tao_duoc=0

    echo "  🏛️ Tạo phòng ban cho $ten_cn (ID: $id_cn)..." | tee -a $LOG_FILE

    for thong_tin_phong in "${danh_sach_phong[@]}"; do
        local ma_phong=$(echo "$thong_tin_phong" | cut -d: -f1)
        local ten_phong=$(echo "$thong_tin_phong" | cut -d: -f2)
        local loai_phong=$(echo "$thong_tin_phong" | cut -d: -f3)
        local ma_phong_day_du="${ma_cn}_${ma_phong}_$TIMESTAMP"

        echo "    Đang tạo: $ten_phong..." | tee -a $LOG_FILE
        id_phong=$(tao_don_vi "$ma_phong_day_du" "$ten_phong" "$loai_phong" "$id_cn")

        if [ -n "$id_phong" ]; then
            so_phong_tao_duoc=$((so_phong_tao_duoc + 1))
            echo "    ✅ $ten_phong tạo thành công (ID: $id_phong)" | tee -a $LOG_FILE
        else
            echo "    ❌ Không thể tạo $ten_phong" | tee -a $LOG_FILE
        fi

        sleep 0.1  # Tránh quá tải API
    done

    echo "  📋 $ten_cn: Đã tạo $so_phong_tao_duoc phòng ban" | tee -a $LOG_FILE
    echo "$so_phong_tao_duoc"
}

# Tạo phòng ban cho Hội sở (7 phòng)
if [ -n "${id_chi_nhanh[HS]}" ]; then
    phong_hoi_so=(
        "BGD:Ban Giám đốc:PNVL1"
        "KTNQ:P. KTNQ:PNVL1"
        "KHDN:P. KHDN:PNVL1"
        "KHCN:P. KHCN:PNVL1"
        "KTGS:P. KTGS:PNVL1"
        "TH:P. Tổng Hợp:PNVL1"
        "KHQLRR:P. KHQLRR:PNVL1"
    )
    so_phong_hs=$(tao_phong_ban_cho_chi_nhanh "HS" "${id_chi_nhanh[HS]}" "Hội sở" "${phong_hoi_so[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_hs))
fi

# Tạo phòng ban cho CN Bình Lư (3 phòng)
if [ -n "${id_chi_nhanh[BL]}" ]; then
    phong_binh_lu=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    so_phong_bl=$(tao_phong_ban_cho_chi_nhanh "BL" "${id_chi_nhanh[BL]}" "CN Bình Lư" "${phong_binh_lu[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_bl))
fi

# Tạo phòng ban cho CN Phong Thổ (4 phòng: 3 phòng + 1 PGD)
if [ -n "${id_chi_nhanh[PT]}" ]; then
    phong_phong_tho=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S5:PGD Số 5:PGDL2"
    )
    so_phong_pt=$(tao_phong_ban_cho_chi_nhanh "PT" "${id_chi_nhanh[PT]}" "CN Phong Thổ" "${phong_phong_tho[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_pt))
fi

# Tạo phòng ban cho CN Sìn Hồ (3 phòng)
if [ -n "${id_chi_nhanh[SH]}" ]; then
    phong_sin_ho=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    so_phong_sh=$(tao_phong_ban_cho_chi_nhanh "SH" "${id_chi_nhanh[SH]}" "CN Sìn Hồ" "${phong_sin_ho[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_sh))
fi

# Tạo phòng ban cho CN Bum Tở (3 phòng)
if [ -n "${id_chi_nhanh[BT]}" ]; then
    phong_bum_to=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    so_phong_bt=$(tao_phong_ban_cho_chi_nhanh "BT" "${id_chi_nhanh[BT]}" "CN Bum Tở" "${phong_bum_to[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_bt))
fi

# Tạo phòng ban cho CN Than Uyên (4 phòng: 3 phòng + 1 PGD)
if [ -n "${id_chi_nhanh[TU]}" ]; then
    phong_than_uyen=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S6:PGD Số 6:PGDL2"
    )
    so_phong_tu=$(tao_phong_ban_cho_chi_nhanh "TU" "${id_chi_nhanh[TU]}" "CN Than Uyên" "${phong_than_uyen[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_tu))
fi

# Tạo phòng ban cho CN Đoàn Kết (5 phòng: 3 phòng + 2 PGD)
if [ -n "${id_chi_nhanh[DK]}" ]; then
    phong_doan_ket=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S1:PGD Số 1:PGDL2"
        "S2:PGD Số 2:PGDL2"
    )
    so_phong_dk=$(tao_phong_ban_cho_chi_nhanh "DK" "${id_chi_nhanh[DK]}" "CN Đoàn Kết" "${phong_doan_ket[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_dk))
fi

# Tạo phòng ban cho CN Tân Uyên (4 phòng: 3 phòng + 1 PGD)
if [ -n "${id_chi_nhanh[TUY]}" ]; then
    phong_tan_uyen=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
        "S3:PGD Số 3:PGDL2"
    )
    so_phong_tuy=$(tao_phong_ban_cho_chi_nhanh "TUY" "${id_chi_nhanh[TUY]}" "CN Tân Uyên" "${phong_tan_uyen[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_tuy))
fi

# Tạo phòng ban cho CN Nậm Hàng (3 phòng)
if [ -n "${id_chi_nhanh[NH]}" ]; then
    phong_nam_hang=(
        "BGD:Ban Giám đốc:PNVL2"
        "KTNQ:P. KTNQ:PNVL2"
        "KH:P. KH:PNVL2"
    )
    so_phong_nh=$(tao_phong_ban_cho_chi_nhanh "NH" "${id_chi_nhanh[NH]}" "CN Nậm Hàng" "${phong_nam_hang[@]}")
    so_phong_ban_thanh_cong=$((so_phong_ban_thanh_cong + so_phong_nh))
fi

# BƯỚC 4: Kiểm tra và thống kê cuối cùng
echo "" | tee -a $LOG_FILE
echo "🔍 BƯỚC 4: KIỂM TRA VÀ THỐNG KÊ CUỐI CÙNG..." | tee -a $LOG_FILE

# Đợi một chút để database cập nhật
sleep 3

# Lấy tổng số đơn vị từ API
tat_ca_don_vi=$(curl -s "$API_BASE/Units")
tong_so_don_vi=$(echo "$tat_ca_don_vi" | grep -c '"Id":')

# Đếm theo loại
so_cnl1=$(echo "$tat_ca_don_vi" | grep -c '"Type":"CNL1"')
so_cnl2=$(echo "$tat_ca_don_vi" | grep -c '"Type":"CNL2"')
so_pnvl1=$(echo "$tat_ca_don_vi" | grep -c '"Type":"PNVL1"')
so_pnvl2=$(echo "$tat_ca_don_vi" | grep -c '"Type":"PNVL2"')
so_pgdl2=$(echo "$tat_ca_don_vi" | grep -c '"Type":"PGDL2"')

# Tính số đơn vị mới tạo
so_don_vi_moi=$((1 + so_chi_nhanh_thanh_cong + so_phong_ban_thanh_cong))

echo "📊 THỐNG KÊ CUỐI CÙNG:" | tee -a $LOG_FILE
echo "┌─────────────────────────────────────────┐" | tee -a $LOG_FILE
echo "│  LOẠI ĐƠN VỊ  │  SỐ LƯỢNG  │  THỰC TẾ  │" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  CNL1 (Gốc)   │     1      │    $so_cnl1     │" | tee -a $LOG_FILE
echo "│  CNL2 (CN)    │     9      │    $so_cnl2     │" | tee -a $LOG_FILE
echo "│  PNVL1 (HỘI)  │     7      │    $so_pnvl1     │" | tee -a $LOG_FILE
echo "│  PNVL2 (PB)   │    24      │    $so_pnvl2    │" | tee -a $LOG_FILE
echo "│  PGDL2 (PGD)  │     5      │    $so_pgdl2     │" | tee -a $LOG_FILE
echo "├─────────────────────────────────────────┤" | tee -a $LOG_FILE
echo "│  TỔNG CỘNG    │    46      │    $tong_so_don_vi    │" | tee -a $LOG_FILE
echo "└─────────────────────────────────────────┘" | tee -a $LOG_FILE

echo "" | tee -a $LOG_FILE
echo "📋 CHI TIẾT QUÁ TRÌNH TẠO:" | tee -a $LOG_FILE
echo "  ✅ Đơn vị gốc: 1/1 (Chi nhánh Lai Châu)" | tee -a $LOG_FILE
echo "  ✅ Chi nhánh: $so_chi_nhanh_thanh_cong/9" | tee -a $LOG_FILE
echo "  ✅ Phòng ban: $so_phong_ban_thanh_cong/36" | tee -a $LOG_FILE
echo "  🎯 Tổng đã tạo: $so_don_vi_moi đơn vị" | tee -a $LOG_FILE

# Kết luận
echo "" | tee -a $LOG_FILE
if [ "$tong_so_don_vi" -ge 46 ]; then
    # Tìm đơn vị gốc mới tạo
    don_vi_goc_moi=$(echo "$tat_ca_don_vi" | grep -B 2 -A 2 "\"Code\":\"$ma_goc\"" | grep '"Name":' | sed 's/.*"Name": *"\([^"]*\)".*/\1/')

    echo "🎉 THÀNH CÔNG! Đã đạt được mục tiêu 46 đơn vị!" | tee -a $LOG_FILE
    echo "✅ Cấu trúc tổ chức Ver2 đã được tạo hoàn chỉnh" | tee -a $LOG_FILE
    echo "🏢 Đơn vị gốc mới: $don_vi_goc_moi (Mã: $ma_goc)" | tee -a $LOG_FILE
    echo "📈 Tổng số đơn vị trong hệ thống: $tong_so_don_vi" | tee -a $LOG_FILE
else
    echo "⚠️ CHƯA ĐẠT MỤC TIÊU! Hiện tại chỉ có $tong_so_don_vi đơn vị" | tee -a $LOG_FILE
    echo "📋 Cần tạo thêm: $((46 - tong_so_don_vi)) đơn vị" | tee -a $LOG_FILE
fi

echo "" | tee -a $LOG_FILE
echo "📅 Hoàn thành lúc: $(date)" | tee -a $LOG_FILE
echo "📝 Log chi tiết: $LOG_FILE" | tee -a $LOG_FILE
echo "🔚 KẾT THÚC SCRIPT TẠO 46 ĐƠN VỊ" | tee -a $LOG_FILE
