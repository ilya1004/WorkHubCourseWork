-- CREATE DATABASE IF NOT EXISTS "ProjectsServiceDb";

-- \c ProjectsServiceDb
-- GRANT ALL PRIVILEGES ON DATABASE "ProjectsServiceDb" TO postgres;



-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

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


-- Table: Logs
CREATE TABLE SystemLogs (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "LogTimestamp" TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "Source" VARCHAR(255),
    "Message" TEXT NOT NULL
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
INNER JOIN "Lifecycles" l ON l."ProjectId" = p."Id"
INNER JOIN "Categories" c ON p."CategoryId" = c."Id";