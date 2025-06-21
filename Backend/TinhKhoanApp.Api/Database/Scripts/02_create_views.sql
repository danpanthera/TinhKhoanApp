-- =====================================================
-- Views for Current Data Access
-- Tạo views để truy cập nhanh dữ liệu hiện tại
-- =====================================================

-- 1. View cho dữ liệu LN01 hiện tại
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'LN01_Current')
EXEC('CREATE VIEW LN01_Current AS
SELECT 
    SourceID,
    MANDT, BUKRS, LAND1, WAERS, SPRAS, 
    KTOPL, WAABW, PERIV, KOKFI, RCOMP,
    ADRNR, STCEG, FIKRS, XFMCO, XFMCB,
    XFMCA, TXJCD,
    ValidFrom, ModifiedDate, VersionNumber
FROM LN01_History
WHERE IsCurrent = 1');

-- 2. View cho dữ liệu GL01 hiện tại
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'GL01_Current')
EXEC('CREATE VIEW GL01_Current AS
SELECT 
    SourceID,
    MANDT, BUKRS, GJAHR, BELNR, BUZEI,
    AUGDT, AUGCP, AUGBL, BSCHL, KOART,
    UMSKZ, UMSKS, ZUMSK, SHKZG, GSBER,
    PARGB, MWSKZ, QSSKZ, DMBTR, WRBTR,
    KZBTR, PSWBT, PSWSL, TXBHW, TXBFW,
    MWSTS, MWSTV, HWBAS, FWBAS, HWSTE,
    FWSTE, STBLG, STGRD, VALUT, ZUONR,
    SGTXT, ZINKZ, VBUND, BEWAR, ALTKT,
    VORGN, FDLEV, FDGRP, HKONT, KUNNR,
    LIFNR, FILKD, XBILK, GVTYP, HZUON,
    ZFBDT, ZTERM, ZBD1T, ZBD2T, ZBD3T,
    ZBD1P, ZBD2P, SKFBT, SKNTO, WSKTO,
    ZLSCH, ZLSPR, ZBFIX, HBKID, BVTYP,
    NEBTR, MWART, DMBE2, DMBE3, PPRCT,
    XREF1, XREF2, KOST1, KOST2, VBEL2,
    POSN2, KKBER, EMPFB,
    ValidFrom, ModifiedDate, VersionNumber
FROM GL01_History
WHERE IsCurrent = 1');

-- 3. View cho dữ liệu DP01 hiện tại
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'DP01_Current')
EXEC('CREATE VIEW DP01_Current AS
SELECT 
    SourceID,
    MANDT, KUNNR, LAND1, NAME1, NAME2,
    NAME3, NAME4, ORT01, ORT02, PFACH,
    PSTL2, PSTLZ, REGIO, SORTL, STRAS,
    ADRNR, MCOD1, MCOD2, MCOD3, ANRED,
    AUFSD, BAHNE, BAHNS, BBBNR, BBSNR,
    BEGRU, BRSCH, BUBKZ, DATLT, ERDAT,
    ERNAM, EXABL, FAKSD, FISKN, KNAZK,
    KNRZA, KONZS, KTOKD, KUKLA, LIFNR,
    LIFSD, LOCCO, LOEVM, NAME1_GP, NIELS,
    NNSNR, PARAU, REVDB, RPMKR, SORTL2,
    SPERR, SPRAS, STCD1, STCD2, STCD3,
    STCD4, STCD5, STCEG, STKZA, STKZU,
    TELBX, TELF1, TELF2, TELFX, TELTX,
    TELX1, XCPDK, XZEMP, VBUND, FISKN_GP,
    DEAR1, DEAR2, DEAR3, DEAR4, DEAR5,
    GFORM, BRAN1, BRAN2, BRAN3, BRAN4,
    BRAN5, EKONT, UMSAT, UMJAH, UWAER,
    JMZAH, JMJAH, KATR1, KATR2, KATR3,
    KATR4, KATR5, KATR6, KATR7, KATR8,
    KATR9, KATR10, STKZN, UMSA1, TXJCD,
    PERIV, ABRVW, INSPBYDEBI, INSPATDEBI,
    KTOCD, PFORT, WERKS, DTAMS, DTAWS,
    DUEFL, HZUOR, SPERZ, ETIKG,
    ValidFrom, ModifiedDate, VersionNumber
FROM DP01_History
WHERE IsCurrent = 1');

-- 4. View thống kê import
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'ImportStatistics')
EXEC('CREATE VIEW ImportStatistics AS
SELECT 
    TableName,
    COUNT(*) as TotalImports,
    SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) as SuccessfulImports,
    SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) as FailedImports,
    SUM(CASE WHEN Status = 'PROCESSING' THEN 1 ELSE 0 END) as ProcessingImports,
    MAX(ImportDate) as LastImportDate,
    SUM(TotalRecords) as TotalRecordsProcessed,
    SUM(NewRecords) as TotalNewRecords,
    SUM(UpdatedRecords) as TotalUpdatedRecords,
    SUM(DeletedRecords) as TotalDeletedRecords,
    ROUND(AVG(Duration), 2) as AvgDurationSeconds,
    ROUND(
        CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) AS FLOAT) * 100.0 / 
        COUNT(*), 2
    ) as SuccessRate
FROM ImportLog
GROUP BY TableName;

-- 5. View import gần đây
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'RecentImports')
EXEC('CREATE VIEW RecentImports AS
SELECT 
    il.LogID,
    il.BatchId,
    il.TableName,
    il.ImportDate,
    il.Status,
    il.TotalRecords,
    il.ProcessedRecords,
    il.NewRecords,
    il.UpdatedRecords,
    il.DeletedRecords,
    il.Duration,
    il.ErrorMessage,
    il.CreatedBy,
    CASE 
        WHEN il.NewRecords + il.UpdatedRecords + il.DeletedRecords > 0 
        THEN 1 ELSE 0 
    END as HasChanges
FROM ImportLog il
ORDER BY il.ImportDate DESC
OFFSET 0 ROWS FETCH NEXT 50 ROWS ONLY;');
