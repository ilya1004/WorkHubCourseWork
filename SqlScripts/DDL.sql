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


-- Indexes
CREATE UNIQUE INDEX IX_Users_Email ON "Users" ("Email");

CREATE INDEX IX_Users_RoleId ON "Users" ("RoleId");

CREATE UNIQUE INDEX IX_FreelancerProfiles_UserId ON "FreelancerProfiles" ("UserId");

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





-- Projects Service

-- Table: Categories
CREATE TABLE "Categories" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL
);

-- Table: Projects
CREATE TABLE "Projects" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" VARCHAR(256) NOT NULL,
    "Description" TEXT NOT NULL,
    "Budget" DECIMAL(18,2) NOT NULL,
    "PaymentIntentId" VARCHAR(256),
    "EmployerUserId" UUID NOT NULL,
    "FreelancerUserId" UUID,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CategoryId" UUID REFERENCES "Categories" ("Id") ON DELETE SET NULL
);

-- Table: Lifecycles
CREATE TABLE "Lifecycles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "ApplicationsStartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ApplicationsDeadline" TIMESTAMP WITH TIME ZONE NOT NULL,
    "WorkStartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "WorkDeadline" TIMESTAMP WITH TIME ZONE NOT NULL,
    "AcceptanceStatus" VARCHAR(256) NOT NULL DEFAULT 'Pending',
    "ProjectStatus" VARCHAR(256) NOT NULL DEFAULT 'Draft',
    "ProjectId" UUID NOT NULL REFERENCES "Projects" ("Id") ON DELETE RESTRICT
);

-- Table: FreelancerApplications
CREATE TABLE "FreelancerApplications" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Status" VARCHAR(256) NOT NULL DEFAULT 'Pending',
    "CvId" UUID NOT NULL,
    "FreelancerUserId" UUID NOT NULL,
    "ProjectId" UUID REFERENCES "Projects" ("Id") ON DELETE RESTRICT
);

-- Table: Reports
CREATE TABLE "Reports" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Description" TEXT,
    "Status" VARCHAR(256) NOT NULL DEFAULT 'Sent',
    "ProjectId" UUID NOT NULL REFERENCES "Projects" ("Id") ON DELETE RESTRICT,
    "ReporterUserId" UUID NOT NULL,
    "ReviewerUserId" UUID
);

-- Table: StarredProjects
CREATE TABLE "StarredProjects" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "ProjectId" UUID NOT NULL REFERENCES "Projects" ("Id") ON DELETE RESTRICT,
    "FreelancerUserId" UUID NOT NULL
);


-- Indexes

CREATE UNIQUE INDEX IX_Categories_Name ON "Categories" ("Name");

CREATE INDEX IX_Projects_EmployerUserId ON "Projects" ("EmployerUserId");
CREATE INDEX IX_Projects_FreelancerUserId ON "Projects" ("FreelancerUserId") WHERE "FreelancerUserId" IS NOT NULL;
CREATE INDEX IX_Projects_CategoryId ON "Projects" ("CategoryId");

CREATE INDEX IX_FreelancerApplications_ProjectId ON "FreelancerApplications" ("ProjectId");
CREATE INDEX IX_FreelancerApplications_FreelancerUserId ON "FreelancerApplications" ("FreelancerUserId");

CREATE INDEX IX_Reports_ProjectId ON "Reports" ("ProjectId");
CREATE INDEX IX_Reports_ReporterUserId ON "Reports" ("ReporterUserId");
CREATE INDEX IX_Reports_ReviewerUserId ON "Reports" ("ReviewerUserId") WHERE "ReviewerUserId" IS NOT NULL;

CREATE INDEX IX_StarredProjects_FreelancerUserId ON "StarredProjects" ("FreelancerUserId");


-- Views

CREATE OR REPLACE VIEW "ProjectInfo" AS
SELECT 
    p."Id",
    p."Title",
    p."Description",
    p."Budget",
    p."PaymentIntentId",
    p."EmployerUserId",
    p."FreelancerUserId",
    p."CategoryId",
    c."Name" AS "CategoryName",
    p."IsActive",

    l."CreatedAt",
    l."UpdatedAt",
    l."ApplicationsStartDate",
    l."ApplicationsDeadline",
    l."WorkStartDate",
    l."WorkDeadline",
    l."AcceptanceStatus",
    l."ProjectStatus"

FROM "Projects" p
INNER JOIN "Lifecycles" l ON l."ProjectId" = p."Id";
INNER JOIN "Categories" c ON p."CategoryId" = c."Id";