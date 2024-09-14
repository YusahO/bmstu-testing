CREATE OR REPLACE FUNCTION update_mean_score_trigger_func()
RETURNS TRIGGER AS
$$
BEGIN
    IF (TG_OP = 'DELETE') THEN
        UPDATE "Audiotracks"
        SET mean_score = (SELECT COALESCE(AVG(value), 0)
                          FROM "Scores"
                          WHERE audiotrack_id = OLD.audiotrack_id)
        WHERE id = OLD.audiotrack_id;
    ELSE
        UPDATE "Audiotracks"
        SET mean_score = (SELECT COALESCE(AVG(value), 0)
                          FROM "Scores"
                          WHERE audiotrack_id = NEW.audiotrack_id)
        WHERE id = NEW.audiotrack_id;
    END IF;
    RETURN NULL;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER update_mean_score_trigger
AFTER 
    INSERT OR 
    UPDATE OF value OR 
    DELETE ON "Scores"
FOR EACH ROW
EXECUTE PROCEDURE update_mean_score_trigger_func();
