-- SQL SERVER TO POSTGRESQL SCHEMA CONVERSION
-- Generated: 2025-06-19
-- Database: TinhKhoanDB
-- Target: PostgreSQL 16

-- USAGE INSTRUCTIONS:
-- 1. Review this file before applying to PostgreSQL
-- 2. Modify data types as needed for PostgreSQL compatibility
-- 3. Run schema creation before importing data
-- 4. Apply constraints after data import to avoid conflicts

-- DATA TYPE CONVERSIONS NEEDED:
-- nvarchar(max) → TEXT
-- nvarchar(n) → VARCHAR(n)  
-- datetime2 → TIMESTAMPTZ
-- bit → BOOLEAN
-- int IDENTITY → SERIAL
-- uniqueidentifier → UUID

-- NOTES:
-- - All tables use 'dbo' schema in SQL Server
-- - Consider using 'app' schema in PostgreSQL
-- - Review foreign key constraints for circular dependencies
-- - Some indexes may need adjustment for PostgreSQL
