START TRANSACTION;
CREATE TABLE "Assignments" (
    "Id" uuid NOT NULL,
    "ConcurrencyToken" uuid NOT NULL,
    "CaseId" uuid NOT NULL,
    "TaskCode" character varying(150) NOT NULL,
    "TargetType" integer NOT NULL,
    "Status" integer NOT NULL,
    "TargetRoleId" uuid,
    "TargetCenterId" uuid,
    "TargetUserRoleId" uuid,
    "TargetUserId" uuid,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    "ClaimedAtUtc" timestamp with time zone,
    "CompletedAtUtc" timestamp with time zone,
    "ClaimedByUserRoleId" uuid,
    CONSTRAINT "PK_Assignments" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_Assignments_CaseId" ON "Assignments" ("CaseId");

CREATE INDEX "IX_Assignments_TargetType_TargetRoleId_TargetCenterId_Status" ON "Assignments" ("TargetType", "TargetRoleId", "TargetCenterId", "Status");

CREATE INDEX "IX_Assignments_TargetUserId_Status" ON "Assignments" ("TargetUserId", "Status");

CREATE INDEX "IX_Assignments_TargetUserRoleId_Status" ON "Assignments" ("TargetUserRoleId", "Status");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260926235504_AddAssignments', '10.0.12');

COMMIT;

START TRANSACTION;
ALTER TABLE "Cases" ADD "CenterId" uuid NOT NULL;

CREATE INDEX "IX_Cases_CenterId" ON "Cases" ("CenterId");

ALTER TABLE "Cases" ADD CONSTRAINT "FK_Cases_Centers_CenterId" FOREIGN KEY ("CenterId") REFERENCES "Centers" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260927135007_AddCenterIdToCase', '10.0.12');

COMMIT;

START TRANSACTION;
CREATE TABLE "OutboxMessages" (
    "Id" uuid NOT NULL,
    "Type" character varying(200) NOT NULL,
    "Payload" jsonb NOT NULL,
    "OccurredAtUtc" timestamp with time zone NOT NULL,
    "ProcessedAtUtc" timestamp with time zone,
    "Error" character varying(4000),
    CONSTRAINT "PK_OutboxMessages" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_OutboxMessages_ProcessedAtUtc_OccurredAtUtc" ON "OutboxMessages" ("ProcessedAtUtc", "OccurredAtUtc");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260927214647_AddOutboxMessages', '10.0.12');

COMMIT;

START TRANSACTION;
INSERT INTO "Roles" ("Id", "Code", "IsActive", "IsSystem", "Name", "Portal", "ScopeType")
VALUES ('20000000-0000-0000-0000-000000000005', 'organization-admin', TRUE, TRUE, 'Organization Administrator', 1, 1);

INSERT INTO "RolePermissions" ("Id", "PermissionId", "RoleId")
VALUES ('30000000-0000-0000-0000-000000000003', '10000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000005');
INSERT INTO "RolePermissions" ("Id", "PermissionId", "RoleId")
VALUES ('30000000-0000-0000-0000-000000000004', '10000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000005');
INSERT INTO "RolePermissions" ("Id", "PermissionId", "RoleId")
VALUES ('30000000-0000-0000-0000-000000000005', '10000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000005');
INSERT INTO "RolePermissions" ("Id", "PermissionId", "RoleId")
VALUES ('30000000-0000-0000-0000-000000000006', '10000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000005');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260928000738_AddOrganizationAdminRole', '10.0.12');

COMMIT;

