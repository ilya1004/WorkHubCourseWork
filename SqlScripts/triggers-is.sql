-- Универсальная функция логирования
CREATE OR REPLACE FUNCTION log_table_changes()
RETURNS TRIGGER AS $$
DECLARE
    table_name TEXT := TG_TABLE_NAME;
    operation  TEXT := TG_OP;
    row_data   TEXT;
BEGIN
    IF operation = 'INSERT' THEN
        row_data := row_to_json(NEW)::TEXT;
    ELSIF operation = 'DELETE' THEN
        row_data := row_to_json(OLD)::TEXT;
    ELSE
        row_data := format('OLD: %s | NEW: %s', row_to_json(OLD)::TEXT, row_to_json(NEW)::TEXT);
    END IF;

    INSERT INTO "SystemLogs" ("Source", "Message")
    VALUES (
        'table_' || table_name,
        format(
            'Operation: %s | Table: %s | RowId: %s | Data: %s',
            operation,
            table_name,
            COALESCE(NEW."Id"::TEXT, OLD."Id"::TEXT),
            row_data
        )
    );

    RETURN NULL;
END;
$$ LANGUAGE plpgsql;


-- Roles
CREATE TRIGGER trg_log_roles_insert
    AFTER INSERT ON "Roles"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_roles_delete
    AFTER DELETE ON "Roles"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();

-- EmployerIndustries
CREATE TRIGGER trg_log_employerindustries_insert
    AFTER INSERT ON "EmployerIndustries"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_employerindustries_delete
    AFTER DELETE ON "EmployerIndustries"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();

-- Users
CREATE TRIGGER trg_log_users_insert
    AFTER INSERT ON "Users"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_users_delete
    AFTER DELETE ON "Users"
    FOR EACH ROW
    EXECUTE FUNCTION log_table_changes();


-- FreelancerProfiles
CREATE TRIGGER trg_log_freelancerprofiles_insert
    AFTER INSERT ON "FreelancerProfiles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_freelancerprofiles_delete
    AFTER DELETE ON "FreelancerProfiles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- EmployerProfiles
CREATE TRIGGER trg_log_employerprofiles_insert
    AFTER INSERT ON "EmployerProfiles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_employerprofiles_delete
    AFTER DELETE ON "EmployerProfiles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- Cvs
CREATE TRIGGER trg_log_cvs_insert
    AFTER INSERT ON "Cvs"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_cvs_delete
    AFTER DELETE ON "Cvs"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- CvWorkExperiences
CREATE TRIGGER trg_log_cvworkexperiences_insert
    AFTER INSERT ON "CvWorkExperiences"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_cvworkexperiences_delete
    AFTER DELETE ON "CvWorkExperiences"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- CvSkills
CREATE TRIGGER trg_log_cvskills_insert
    AFTER INSERT ON "CvSkills"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_cvskills_delete
    AFTER DELETE ON "CvSkills"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- CvLanguages
CREATE TRIGGER trg_log_cvlanguages_insert
    AFTER INSERT ON "CvLanguages"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_cvlanguages_delete
    AFTER DELETE ON "CvLanguages"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();