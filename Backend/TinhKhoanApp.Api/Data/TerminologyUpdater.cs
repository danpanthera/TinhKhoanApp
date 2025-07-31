using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Data
{
    /// <summary>
    /// Cập nhật và chuẩn hóa terminology trong database
    /// KTNV -> KTNQ, "Kinh tế Nội vụ" -> "Kế toán và Ngân quỹ"
    /// </summary>
    public static class TerminologyUpdater
    {
        public static void UpdateTerminology(ApplicationDbContext context)
        {
            try
            {
                Console.WriteLine("🔄 Bắt đầu chuẩn hóa terminology...");

                UpdateTableTerminology(context);
                UpdateIndicatorTerminology(context);

                context.SaveChanges();
                Console.WriteLine("✅ Hoàn thành chuẩn hóa terminology!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi chuẩn hóa terminology: {ex.Message}");
            }
        }

        private static void UpdateTableTerminology(ApplicationDbContext context)
        {
            try
            {
                var tables = context.KpiAssignmentTables
                    .Where(t => t.TableName.Contains("KTNV") ||
                               (t.Description != null && t.Description.Contains("Kinh tế Nội vụ")) ||
                               (t.Description != null && t.Description.Contains("Hạch kiểm")) ||
                               t.TableName.Contains("phụ trách Kinh tế") ||
                               (t.Description != null && t.Description.Contains("phụ trách Kinh tế")) ||
                               // Thêm các bảng KPI cụ thể cần cập nhật
                               t.TableName.Contains("Trưởng phòng KTNV") ||
                               t.TableName.Contains("Phó phòng KTNV"))
                    .ToList();

                if (tables.Count == 0)
                {
                    Console.WriteLine("✅ Không có bảng KPI nào cần chuẩn hóa terminology");
                    return;
                }

                int changesCount = 0;
                foreach (var table in tables)
                {
                    bool changed = false;

                    // 1. KTNV -> KTNQ (chuẩn hóa mã phòng ban)
                    if (table.TableName.Contains("KTNV"))
                    {
                        table.TableName = table.TableName.Replace("KTNV", "KTNQ");
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật TableName: KTNV -> KTNQ trong '{table.TableName}'");
                    }

                    // 2. Kinh tế Nội vụ -> Kế toán & Ngân quỹ (chuẩn hóa tên phòng ban)
                    if (table.Description?.Contains("Kinh tế Nội vụ") == true)
                    {
                        string oldDesc = table.Description;
                        table.Description = table.Description.Replace("Kinh tế Nội vụ", "Kế toán & Ngân quỹ");
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật Description: '{oldDesc}' -> '{table.Description}'");
                    }

                    // 3. Hạch kiểm -> Hậu kiểm
                    if (table.Description?.Contains("Hạch kiểm") == true)
                    {
                        string oldDesc = table.Description;
                        table.Description = table.Description.Replace("Hạch kiểm", "Hậu kiểm");
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật Description: '{oldDesc}' -> '{table.Description}'");
                    }

                    // 4. Chuẩn hóa: phụ trách Kinh tế -> Phụ trách Kế toán
                    if (table.TableName.Contains("phụ trách Kinh tế") || table.TableName.Contains("Phó giám đốc CNL2 phụ trách Kinh tế"))
                    {
                        string oldTableName = table.TableName;
                        table.TableName = table.TableName.Replace("phụ trách Kinh tế", "Phụ trách Kế toán")
                                                       .Replace("Phó giám đốc CNL2 phụ trách Kinh tế", "Phó giám đốc CNL2 Phụ trách Kế toán");
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật TableName: '{oldTableName}' -> '{table.TableName}'");
                    }

                    // 5. Cập nhật các bảng KPI cụ thể: KTNV -> KTNQ và TableType
                    if (table.TableName == "Trưởng phòng KTNV CNL1")
                    {
                        table.TableName = "Trưởng phòng KTNQ CNL1";
                        table.TableType = KpiTableType.TruongphongKtnqCnl1;
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật bảng KPI: Trưởng phòng KTNV CNL1 -> Trưởng phòng KTNQ CNL1");
                    }
                    else if (table.TableName == "Phó phòng KTNV CNL1")
                    {
                        table.TableName = "Phó phòng KTNQ CNL1";
                        table.TableType = KpiTableType.PhophongKtnqCnl1;
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật bảng KPI: Phó phòng KTNV CNL1 -> Phó phòng KTNQ CNL1");
                    }
                    else if (table.TableName == "Trưởng phòng KTNV CNL2")
                    {
                        table.TableName = "Trưởng phòng KTNQ CNL2";
                        table.TableType = KpiTableType.TruongphongKtnqCnl2;
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật bảng KPI: Trưởng phòng KTNV CNL2 -> Trưởng phòng KTNQ CNL2");
                    }
                    else if (table.TableName == "Phó phòng KTNV CNL2")
                    {
                        table.TableName = "Phó phòng KTNQ CNL2";
                        table.TableType = KpiTableType.PhophongKtnqCnl2;
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật bảng KPI: Phó phòng KTNV CNL2 -> Phó phòng KTNQ CNL2");
                    }

                    // 6. Chuẩn hóa: phụ trách Kinh tế trong Description -> Phụ trách Kế toán
                    if (table.Description?.Contains("phụ trách Kinh tế") == true || table.Description?.Contains("Phó giám đốc Chi nhánh loại 2 phụ trách Kinh tế") == true)
                    {
                        string oldDesc = table.Description;
                        table.Description = table.Description.Replace("phụ trách Kinh tế", "Phụ trách Kế toán")
                                                          .Replace("Phó giám đốc Chi nhánh loại 2 phụ trách Kinh tế", "Phó giám đốc Chi nhánh loại 2 Phụ trách Kế toán");
                        changed = true;
                        Console.WriteLine($"📋 Cập nhật Description: '{oldDesc}' -> '{table.Description}'");
                    }

                    // Special cases
                    if (table.TableName == "TQ/HK/KTNB")
                    {
                        if (table.Description?.Contains("Thủ quỹ/Hạch kiểm/Kinh tế Nội bộ") == true)
                        {
                            table.TableName = "TQ/HK/KTNQ";
                            table.Description = table.Description.Replace("Thủ quỹ/Hạch kiểm/Kinh tế Nội bộ", "Thủ quỹ/Hậu kiểm/Kế toán Nội bộ");
                            changed = true;
                            Console.WriteLine($"📋 Cập nhật đặc biệt: TQ/HK/KTNB -> TQ/HK/KTNQ");
                        }
                    }

                    if (changed)
                    {
                        changesCount++;
                    }
                }

                if (changesCount > 0)
                {
                    Console.WriteLine($"✅ Đã chuẩn hóa terminology cho {changesCount} bảng KPI");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi chuẩn hóa terminology cho bảng KPI: {ex.Message}");
            }
        }

        private static void UpdateIndicatorTerminology(ApplicationDbContext context)
        {
            try
            {
                var indicators = context.KpiIndicators
                    .Where(i => i.IndicatorName.Contains("Kinh tế Nội vụ") ||
                           i.IndicatorName.Contains("Hạch kiểm") ||
                           i.IndicatorName.Contains("phụ trách Kinh tế"))
                    .ToList();

                if (indicators.Count == 0)
                {
                    Console.WriteLine("✅ Không có indicators cần cập nhật terminology");
                    return;
                }

                int changesCount = 0;
                foreach (var indicator in indicators)
                {
                    bool changed = false;
                    string oldName = indicator.IndicatorName;

                    // Chuẩn hóa: Kinh tế Nội vụ -> Kế toán & Ngân quỹ trong indicator names
                    if (indicator.IndicatorName.Contains("Kinh tế Nội vụ"))
                    {
                        indicator.IndicatorName = indicator.IndicatorName.Replace("Kinh tế Nội vụ", "Kế toán & Ngân quỹ");
                        changed = true;
                    }

                    if (indicator.IndicatorName.Contains("Hạch kiểm"))
                    {
                        indicator.IndicatorName = indicator.IndicatorName.Replace("Hạch kiểm", "Hậu kiểm");
                        changed = true;
                    }

                    if (indicator.IndicatorName.Contains("phụ trách Kinh tế"))
                    {
                        indicator.IndicatorName = indicator.IndicatorName.Replace("phụ trách Kinh tế", "Phụ trách Kế toán");
                        changed = true;
                    }

                    if (changed)
                    {
                        changesCount++;
                        Console.WriteLine($"📊 Cập nhật Indicator: '{oldName}' -> '{indicator.IndicatorName}'");
                    }
                }

                if (changesCount > 0)
                {
                    Console.WriteLine($"✅ Đã chuẩn hóa terminology cho {changesCount} indicators");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi chuẩn hóa terminology cho indicators: {ex.Message}");
            }
        }
    }
}
