-- CREATE DATABASE IF NOT EXISTS "IdentityServiceDb";

-- \c IdentityServiceDb
-- GRANT ALL PRIVILEGES ON DATABASE "IdentityServiceDb" TO postgres;



-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Identity Service

-- Table: Roles
CREATE TABLE "Roles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL
);

-- Table: EmployerIndustries
CREATE TABLE "EmployerIndustries" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL
);

-- Table: Users
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "RegisteredAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ImageUrl" VARCHAR(512),
    "Email" VARCHAR(256) NOT NULL,
    "PasswordHash" VARCHAR(256) NOT NULL,
    "RefreshToken" VARCHAR(256),
    "RefreshTokenExpiryTime" TIMESTAMP WITH TIME ZONE,
    "IsEmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "RoleId" UUID NOT NULL REFERENCES "Roles" ("Id") ON DELETE RESTRICT
);

-- Table: FreelancerProfiles
CREATE TABLE "FreelancerProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "FirstName" VARCHAR(256) NOT NULL,
    "LastName" VARCHAR(256) NOT NULL,
    "Nickname" VARCHAR(256) NOT NULL,
    "About" TEXT,
    "StripeAccountId" VARCHAR(256),
    "UserId" UUID NOT NULL REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IX_FreelancerProfiles_UserId ON "FreelancerProfiles" ("UserId");

-- Table: EmployerProfiles
CREATE TABLE "EmployerProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CompanyName" VARCHAR(256) NOT NULL,
    "About" TEXT,
    "IndustryId" UUID REFERENCES "EmployerIndustries" ("Id") ON DELETE SET NULL,
    "StripeCustomerId" VARCHAR(256),
    "UserId" UUID NOT NULL REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

-- Table: Cvs
CREATE TABLE "Cvs" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" VARCHAR(256) NOT NULL,
    "UserSpecialization" VARCHAR(256) NOT NULL,
    "UserEducation" TEXT,
    "IsPublic" BOOLEAN NOT NULL DEFAULT FALSE,
    "FreelancerUserId" UUID NOT NULL REFERENCES "FreelancerProfiles" ("UserId") ON DELETE RESTRICT
);

-- Table: CvWorkExperiences
CREATE TABLE "CvWorkExperiences" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserSpecialization" VARCHAR(256) NOT NULL,
    "StartDate" DATE NOT NULL,
    "EndDate" DATE NOT NULL,
    "Responsibilities" TEXT,
    "CvId" UUID NOT NULL REFERENCES "Cvs" ("Id") ON DELETE RESTRICT
);

-- Table: CvSkills
CREATE TABLE "CvSkills" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "ExperienceInYears" INTEGER,
    "CvId" UUID NOT NULL REFERENCES "Cvs" ("Id") ON DELETE RESTRICT
);

-- Table: CvLanguages
CREATE TABLE "CvLanguages" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "Level" VARCHAR(256) NOT NULL,
    "CvId" UUID NOT NULL REFERENCES "Cvs" ("Id") ON DELETE RESTRICT
);


-- Table: Logs
CREATE TABLE SystemLogs (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "LogTimestamp" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "Source" VARCHAR(255),
    "Message" TEXT NOT NULL
);


-- Indexes
CREATE UNIQUE INDEX IX_Users_Email ON "Users" ("Email");

CREATE INDEX IX_Users_RoleId ON "Users" ("RoleId");

CREATE UNIQUE INDEX IX_EmployerProfiles_UserId ON "EmployerProfiles" ("UserId");

CREATE INDEX IX_EmployerProfiles_IndustryId ON "EmployerProfiles" ("IndustryId");

CREATE UNIQUE INDEX IX_Roles_Name ON "Roles" ("Name");

CREATE UNIQUE INDEX IX_EmployerIndustries_Name ON "EmployerIndustries" ("Name");

CREATE INDEX IX_Cvs_FreelancerUserId ON "Cvs" ("FreelancerUserId");

CREATE INDEX IX_CvWorkExperiences_CvId ON "CvWorkExperiences" ("CvId");

CREATE INDEX IX_CvSkills_CvId ON "CvSkills" ("CvId");

CREATE INDEX IX_CvLanguages_CvId ON "CvLanguages" ("CvId");


-- Views
CREATE OR REPLACE VIEW "EmployerUser" AS
SELECT 
    u."Id",
    u."Email",
    u."RegisteredAt",
    u."ImageUrl",
    r."Name" AS "RoleName",
    
    ep."CompanyName",
    ep."About",
    ep."StripeCustomerId",
    ei."Id" AS "IndustryId",
    ei."Name" AS "IndustryName"
FROM "Users" u
INNER JOIN "EmployerProfiles" ep ON ep."UserId" = u."Id"
INNER JOIN "Roles" r ON r."Id" = u."RoleId"
LEFT JOIN "EmployerIndustries" ei ON ei."Id" = ep."IndustryId";

CREATE OR REPLACE VIEW "FreelancerUser" AS
SELECT 
    u."Id",
    u."Email",
    u."RegisteredAt",
    u."ImageUrl",
    r."Name" AS "RoleName",
    
    fp."FirstName",
    fp."LastName",
    fp."Nickname",
    fp."About",
    fp."StripeAccountId"
FROM "Users" u
INNER JOIN "FreelancerProfiles" fp ON fp."UserId" = u."Id"
INNER JOIN "Roles" r ON r."Id" = u."RoleId";


