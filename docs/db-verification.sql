-- db-verification.sql
-- Use this script to validate that ASP.NET Core Identity tables
-- were created and seeded correctly in MariaDB/MySQL.

-- 1) List all tables in the currently selected database.
-- Confirms that Identity tables (AspNetUsers, AspNetRoles, etc.) exist.
SELECT
    TABLE_NAME
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
ORDER BY TABLE_NAME;

-- 2) Check AspNetUsers table structure.
-- Shows columns, types, nullability, defaults, and key flags.
SELECT
    COLUMN_NAME,
    COLUMN_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    COLUMN_KEY,
    EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'AspNetUsers'
ORDER BY ORDINAL_POSITION;

-- 3) Check AspNetRoles table structure.
-- Verifies role table columns and key metadata.
SELECT
    COLUMN_NAME,
    COLUMN_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    COLUMN_KEY,
    EXTRA
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'AspNetRoles'
ORDER BY ORDINAL_POSITION;

-- 4) Verify seeded admin user.
-- Returns admin user details if the seed process created it.
SELECT
    Id,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    LockoutEnabled,
    AccessFailedCount
FROM AspNetUsers
WHERE NormalizedEmail = 'ADMIN@BREVET.LOCAL'
   OR Email = 'admin@brevet.local';

-- Optional: verify the seeded admin is assigned the Admin role.
SELECT
    u.Email,
    r.Name AS RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON ur.UserId = u.Id
INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE (u.NormalizedEmail = 'ADMIN@BREVET.LOCAL' OR u.Email = 'admin@brevet.local')
  AND (r.NormalizedName = 'ADMIN' OR r.Name = 'Admin');

-- 5) Check foreign key relationships for Identity tables.
-- Lists FK constraints and the referenced table/column pairs.
SELECT
    kcu.CONSTRAINT_NAME,
    kcu.TABLE_NAME,
    kcu.COLUMN_NAME,
    kcu.REFERENCED_TABLE_NAME,
    kcu.REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE kcu
WHERE kcu.TABLE_SCHEMA = DATABASE()
  AND kcu.REFERENCED_TABLE_NAME IS NOT NULL
  AND (
      kcu.TABLE_NAME LIKE 'AspNet%'
      OR kcu.REFERENCED_TABLE_NAME LIKE 'AspNet%'
  )
ORDER BY kcu.TABLE_NAME, kcu.CONSTRAINT_NAME, kcu.ORDINAL_POSITION;
