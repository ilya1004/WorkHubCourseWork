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


-- Categories
CREATE TRIGGER trg_log_categories_insert
    AFTER INSERT ON "Categories"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_categories_delete
    AFTER DELETE ON "Categories"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- Projects
CREATE TRIGGER trg_log_projects_insert
    AFTER INSERT ON "Projects"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_projects_delete
    AFTER DELETE ON "Projects"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- Lifecycles
CREATE TRIGGER trg_log_lifecycles_insert
    AFTER INSERT ON "Lifecycles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_lifecycles_delete
    AFTER DELETE ON "Lifecycles"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- FreelancerApplications
CREATE TRIGGER trg_log_freelancerapplications_insert
    AFTER INSERT ON "FreelancerApplications"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_freelancerapplications_delete
    AFTER DELETE ON "FreelancerApplications"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- Reports
CREATE TRIGGER trg_log_reports_insert
    AFTER INSERT ON "Reports"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_reports_delete
    AFTER DELETE ON "Reports"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

-- StarredProjects
CREATE TRIGGER trg_log_starredprojects_insert
    AFTER INSERT ON "StarredProjects"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();

CREATE TRIGGER trg_log_starredprojects_delete
    AFTER DELETE ON "StarredProjects"
    FOR EACH ROW EXECUTE FUNCTION log_table_changes();