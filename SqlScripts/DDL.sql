-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Identity Service

-- Table: Roles
CREATE TABLE "Roles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "NormalizedName" VARCHAR(256) NOT NULL
);

-- Table: EmployerIndustries
CREATE TABLE "EmployerIndustries" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "NormalizedName" VARCHAR(256) NOT NULL
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
    "RoleId" UUID NOT NULL REFERENCES "Roles" ("Id") ON DELETE RESTRICT
);

-- Table: FreelancerProfiles
CREATE TABLE "FreelancerProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "FirstName" VARCHAR(256) NOT NULL,
    "LastName" VARCHAR(256) NOT NULL,
    "About" TEXT,
    "StripeAccountId" VARCHAR(256),
    "UserId" UUID NOT NULL REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

-- Table: EmployerProfiles
CREATE TABLE "EmployerProfiles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CompanyName" VARCHAR(256) NOT NULL,
    "About" TEXT,
    "IndustryId" UUID REFERENCES "EmployerIndustries" ("Id") ON DELETE RESTRICT,
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
    "NormalizedName" VARCHAR(256) NOT NULL,
    "ExperienceInYears" INTEGER,
    "CvId" UUID NOT NULL REFERENCES "Cvs" ("Id") ON DELETE RESTRICT
);

-- Table: CvLanguages
CREATE TABLE "CvLanguages" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "NormalizedName" VARCHAR(256) NOT NULL,
    "Level" VARCHAR(256) NOT NULL,
    "CvId" UUID NOT NULL REFERENCES "Cvs" ("Id") ON DELETE RESTRICT
);

-- Indexes
CREATE UNIQUE INDEX IX_Users_Email ON "Users" ("Email");

CREATE INDEX IX_Users_RoleId ON "Users" ("RoleId");

CREATE UNIQUE INDEX IX_FreelancerProfiles_UserId ON "FreelancerProfiles" ("UserId");

CREATE UNIQUE INDEX IX_EmployerProfiles_UserId ON "EmployerProfiles" ("UserId");

CREATE INDEX IX_EmployerProfiles_IndustryId ON "EmployerProfiles" ("IndustryId");

CREATE UNIQUE INDEX IX_Roles_NormalizedName ON "Roles" ("NormalizedName");

CREATE UNIQUE INDEX IX_EmployerIndustries_NormalizedName ON "EmployerIndustries" ("NormalizedName");

CREATE INDEX IX_Cvs_FreelancerUserId ON "Cvs" ("FreelancerUserId");

CREATE INDEX IX_CvWorkExperiences_CvId ON "CvWorkExperiences" ("CvId");

CREATE INDEX IX_CvSkills_CvId ON "CvSkills" ("CvId");

CREATE INDEX IX_CvLanguages_CvId ON "CvLanguages" ("CvId");



-- Projects Service

-- Table: Categories
CREATE TABLE "Categories" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(256) NOT NULL,
    "NormalizedName" VARCHAR(256) NOT NULL
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
    "CategoryId" UUID REFERENCES "Categories" ("Id") ON DELETE RESTRICT
);

-- Table: Lifecycles
CREATE TABLE "Lifecycles" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ApplicationsStartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ApplicationsDeadline" TIMESTAMP WITH TIME ZONE NOT NULL,
    "WorkStartDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "WorkDeadline" TIMESTAMP WITH TIME ZONE NOT NULL,
    "AcceptanceStatus" VARCHAR(256) NOT NULL DEFAULT 'Pending',
    "Status" VARCHAR(256) NOT NULL DEFAULT 'Draft',
    "ProjectId" UUID NOT NULL REFERENCES "Projects" ("Id") ON DELETE RESTRICT
);

-- Table: FreelancerApplications
CREATE TABLE "FreelancerApplications" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Status" VARCHAR(256) NOT NULL DEFAULT 'Pending',
    "ProjectId" UUID NOT NULL,
    "FreelancerUserId" UUID NOT NULL,
    "CvId" UUID REFERENCES "Projects" ("Id") ON DELETE RESTRICT
);

-- Table: Reports
CREATE TABLE "Reports" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Description" TEXT,
    "Status" VARCHAR(256) NOT NULL DEFAULT 'Submitted',
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

CREATE UNIQUE INDEX IX_Categories_NormalizedName ON "Categories" ("NormalizedName");

CREATE INDEX IX_Projects_EmployerUserId ON "Projects" ("EmployerUserId");
CREATE INDEX IX_Projects_FreelancerUserId ON "Projects" ("FreelancerUserId") WHERE "FreelancerUserId" IS NOT NULL;
CREATE INDEX IX_Projects_CategoryId ON "Projects" ("CategoryId");

CREATE INDEX IX_FreelancerApplications_ProjectId ON "FreelancerApplications" ("ProjectId");
CREATE INDEX IX_FreelancerApplications_FreelancerUserId ON "FreelancerApplications" ("FreelancerUserId");

CREATE INDEX IX_Reports_ProjectId ON "Reports" ("ProjectId");
CREATE INDEX IX_Reports_ReporterUserId ON "Reports" ("ReporterUserId");
CREATE INDEX IX_Reports_ReviewerUserId ON "Reports" ("ReviewerUserId") WHERE "ReviewerUserId" IS NOT NULL;

CREATE INDEX IX_StarredProjects_FreelancerUserId ON "StarredProjects" ("FreelancerUserId");
