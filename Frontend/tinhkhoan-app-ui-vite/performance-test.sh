#!/bin/bash

# 🚀 Performance Test Script for TinhKhoanApp
# Tests the optimized APIs and compares performance before/after optimization

BASE_URL="http://localhost:5000/api"
OUTPUT_DIR="./performance-results"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Create output directory
mkdir -p "$OUTPUT_DIR"

echo -e "${BLUE}🚀 Starting Performance Tests - $TIMESTAMP${NC}"
echo "=================================="

# Function to make HTTP request and measure time
measure_request() {
    local url="$1"
    local description="$2"
    local method="${3:-GET}"
    local data="${4:-}"
    
    echo -e "\n${YELLOW}Testing: $description${NC}"
    echo "URL: $url"
    
    if [ "$method" = "POST" ]; then
        response_time=$(curl -w "%{time_total}" -s -o /dev/null -X POST -H "Content-Type: application/json" -d "$data" "$url")
    else
        response_time=$(curl -w "%{time_total}" -s -o /dev/null "$url")
    fi
    
    # Convert to milliseconds
    response_time_ms=$(echo "$response_time * 1000" | bc)
    
    echo "Response time: ${response_time_ms}ms"
    
    # Color code the result
    if (( $(echo "$response_time_ms < 100" | bc -l) )); then
        echo -e "${GREEN}✅ Excellent performance (<100ms)${NC}"
    elif (( $(echo "$response_time_ms < 500" | bc -l) )); then
        echo -e "${GREEN}✅ Good performance (<500ms)${NC}"
    elif (( $(echo "$response_time_ms < 1000" | bc -l) )); then
        echo -e "${YELLOW}⚠️  Fair performance (<1s)${NC}"
    else
        echo -e "${RED}❌ Poor performance (>1s)${NC}"
    fi
    
    # Log to file
    echo "$TIMESTAMP,$description,$url,$response_time_ms" >> "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv"
    
    return 0
}

# Initialize CSV log
echo "timestamp,test_description,url,response_time_ms" > "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv"

echo -e "\n${BLUE}📊 Testing Original APIs${NC}"
echo "========================"

# Test original APIs
measure_request "$BASE_URL/rawdata" "Get All Raw Data Imports (Original)"
measure_request "$BASE_URL/DataImport" "Get All Data Imports (Original)"
measure_request "$BASE_URL/rawdata/table/LN01" "Get LN01 Table Data (Original)"
measure_request "$BASE_URL/rawdata/by-date/LN01/20250531" "Get LN01 by Date (Original)"

echo -e "\n${BLUE}⚡ Testing Optimized APIs${NC}"
echo "========================="

# Test optimized APIs
measure_request "$BASE_URL/rawdata/optimized/imports?page=1&pageSize=50" "Get Optimized Imports (Paginated)"
measure_request "$BASE_URL/rawdata/optimized/imports?page=1&pageSize=100" "Get Optimized Imports (Larger Page)"
measure_request "$BASE_URL/rawdata/optimized/records/all?offset=0&limit=50" "Get Optimized Records (Virtual Scroll)"
measure_request "$BASE_URL/rawdata/optimized/scd?tableName=LN01_History&page=1&pageSize=50" "Get Optimized SCD Records"
measure_request "$BASE_URL/rawdata/optimized/dashboard-stats" "Get Dashboard Stats (Cached)"

echo -e "\n${BLUE}🔍 Testing Search and Filtering${NC}"
echo "================================"

# Test search and filtering
measure_request "$BASE_URL/rawdata/optimized/search?searchTerm=LN01&page=1&pageSize=50" "Advanced Search (LN01)"
measure_request "$BASE_URL/rawdata/optimized/imports?searchTerm=test&sortBy=ImportDate&sortOrder=desc" "Filtered Imports Search"

echo -e "\n${BLUE}📈 Testing Performance Stats${NC}"
echo "============================="

# Test performance monitoring
measure_request "$BASE_URL/rawdata/optimized/performance-stats?timeRange=24h" "Get Performance Stats (24h)"
measure_request "$BASE_URL/rawdata/optimized/performance-stats?timeRange=7d" "Get Performance Stats (7d)"

echo -e "\n${BLUE}💾 Testing Cache Operations${NC}"
echo "============================"

# Test cache refresh
measure_request "$BASE_URL/rawdata/optimized/refresh-cache" "Refresh Cache" "POST" '{"cacheKey":"dashboard-stats"}'

echo -e "\n${BLUE}🧪 Load Testing (Multiple Requests)${NC}"
echo "===================================="

# Load test - multiple concurrent requests
echo "Running 10 concurrent requests to optimized imports..."
start_time=$(date +%s.%N)

for i in {1..10}; do
    curl -s -o /dev/null "$BASE_URL/rawdata/optimized/imports?page=$i&pageSize=25" &
done
wait

end_time=$(date +%s.%N)
total_time=$(echo "$end_time - $start_time" | bc)
total_time_ms=$(echo "$total_time * 1000" | bc)

echo "Total time for 10 concurrent requests: ${total_time_ms}ms"
avg_time=$(echo "$total_time_ms / 10" | bc)
echo "Average time per request: ${avg_time}ms"

echo "$TIMESTAMP,Load Test (10 concurrent),Multiple,$total_time_ms" >> "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv"

echo -e "\n${BLUE}📊 Generating Performance Report${NC}"
echo "=================================="

# Generate performance report
REPORT_FILE="$OUTPUT_DIR/performance_report_$TIMESTAMP.html"

cat > "$REPORT_FILE" << EOF
<!DOCTYPE html>
<html>
<head>
    <title>Performance Test Report - $TIMESTAMP</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        .header { background: #1890ff; color: white; padding: 20px; border-radius: 8px; }
        .summary { background: #f8f9fa; padding: 15px; border-radius: 8px; margin: 20px 0; }
        .test-results { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .test-results th, .test-results td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        .test-results th { background-color: #f2f2f2; }
        .excellent { background-color: #f6ffed; color: #52c41a; }
        .good { background-color: #f6ffed; color: #52c41a; }
        .fair { background-color: #fff7e6; color: #fa8c16; }
        .poor { background-color: #fff2f0; color: #f5222d; }
        .chart { width: 100%; height: 300px; background: #f8f9fa; border: 1px solid #ddd; margin: 20px 0; }
    </style>
</head>
<body>
    <div class="header">
        <h1>🚀 TinhKhoanApp Performance Test Report</h1>
        <p>Generated: $TIMESTAMP</p>
    </div>
    
    <div class="summary">
        <h2>📋 Test Summary</h2>
        <p><strong>Test Date:</strong> $(date)</p>
        <p><strong>Base URL:</strong> $BASE_URL</p>
        <p><strong>Total Tests:</strong> $(wc -l < "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | awk '{print $1-1}')</p>
    </div>
    
    <h2>📊 Test Results</h2>
    <table class="test-results">
        <thead>
            <tr>
                <th>Test Description</th>
                <th>Response Time (ms)</th>
                <th>Performance Rating</th>
                <th>URL</th>
            </tr>
        </thead>
        <tbody>
EOF

# Add test results to HTML
tail -n +2 "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | while IFS=',' read -r timestamp description url response_time; do
    if (( $(echo "$response_time < 100" | bc -l) )); then
        rating="excellent"
        rating_text="Excellent"
    elif (( $(echo "$response_time < 500" | bc -l) )); then
        rating="good"
        rating_text="Good"
    elif (( $(echo "$response_time < 1000" | bc -l) )); then
        rating="fair"
        rating_text="Fair"
    else
        rating="poor"
        rating_text="Poor"
    fi
    
    echo "<tr class=\"$rating\">" >> "$REPORT_FILE"
    echo "<td>$description</td>" >> "$REPORT_FILE"
    echo "<td>${response_time}ms</td>" >> "$REPORT_FILE"
    echo "<td>$rating_text</td>" >> "$REPORT_FILE"
    echo "<td><small>$url</small></td>" >> "$REPORT_FILE"
    echo "</tr>" >> "$REPORT_FILE"
done

cat >> "$REPORT_FILE" << EOF
        </tbody>
    </table>
    
    <h2>🎯 Performance Recommendations</h2>
    <div class="summary">
        <h3>✅ Optimizations Applied</h3>
        <ul>
            <li>Database indexing for frequently queried columns</li>
            <li>API response caching for dashboard statistics</li>
            <li>Pagination for large datasets</li>
            <li>Virtual scrolling support for frontend</li>
            <li>Optimized SQL queries with proper JOINs</li>
        </ul>
        
        <h3>🔍 Further Improvements</h3>
        <ul>
            <li>Implement Redis caching for production environments</li>
            <li>Add database query profiling and monitoring</li>
            <li>Consider implementing GraphQL for flexible data fetching</li>
            <li>Add CDN for static assets</li>
            <li>Implement database connection pooling</li>
        </ul>
    </div>
    
    <h2>📈 Performance Trends</h2>
    <div class="chart">
        <p style="text-align: center; line-height: 300px; color: #666;">
            Chart visualization would be implemented here<br>
            (Response times over different API endpoints)
        </p>
    </div>
    
    <footer style="margin-top: 40px; padding: 20px; background: #f8f9fa; border-radius: 8px;">
        <p><strong>Report generated by TinhKhoanApp Performance Testing Tool</strong></p>
        <p>For more details, check the CSV log: performance_log_$TIMESTAMP.csv</p>
    </footer>
</body>
</html>
EOF

echo -e "\n${GREEN}✅ Performance testing completed!${NC}"
echo "=================================="
echo "📁 Results saved to: $OUTPUT_DIR/"
echo "📊 HTML Report: $REPORT_FILE"
echo "📈 CSV Log: $OUTPUT_DIR/performance_log_$TIMESTAMP.csv"

# Calculate and display summary statistics
echo -e "\n${BLUE}📊 Summary Statistics${NC}"
echo "===================="

total_tests=$(tail -n +2 "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | wc -l)
avg_response=$(tail -n +2 "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | cut -d',' -f4 | awk '{sum+=$1} END {print sum/NR}')
min_response=$(tail -n +2 "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | cut -d',' -f4 | sort -n | head -1)
max_response=$(tail -n +2 "$OUTPUT_DIR/performance_log_$TIMESTAMP.csv" | cut -d',' -f4 | sort -n | tail -1)

echo "Total tests: $total_tests"
echo "Average response time: ${avg_response}ms"
echo "Fastest response: ${min_response}ms"
echo "Slowest response: ${max_response}ms"

# Performance rating
if (( $(echo "$avg_response < 200" | bc -l) )); then
    echo -e "${GREEN}🎉 Overall Performance: EXCELLENT${NC}"
elif (( $(echo "$avg_response < 500" | bc -l) )); then
    echo -e "${GREEN}✅ Overall Performance: GOOD${NC}"
elif (( $(echo "$avg_response < 1000" | bc -l) )); then
    echo -e "${YELLOW}⚠️  Overall Performance: FAIR${NC}"
else
    echo -e "${RED}❌ Overall Performance: NEEDS IMPROVEMENT${NC}"
fi

echo -e "\n${BLUE}🔗 Next Steps:${NC}"
echo "1. Review the HTML report for detailed analysis"
echo "2. Monitor performance in production environment"
echo "3. Consider implementing additional optimizations based on results"
echo "4. Set up automated performance monitoring"

exit 0
