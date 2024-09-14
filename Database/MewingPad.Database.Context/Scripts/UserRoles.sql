CREATE ROLE mewingpad_admin
    SUPERUSER
    CREATEDB
    CREATEROLE
    LOGIN
    PASSWORD '$@$admin$@$';

GRANT ALL PRIVILEGES
	ON ALL TABLES IN SCHEMA public
	TO mewingpad_admin;

CREATE ROLE mewingpad_user
    NOSUPERUSER
    NOCREATEDB
    NOCREATEROLE
	LOGIN
    PASSWORD '$@$user$@$'
	CONNECTION LIMIT -1;

GRANT SELECT, INSERT ON public."Users" TO mewingpad_user;
GRANT SELECT, INSERT ON public."Reports" TO mewingpad_user;
GRANT SELECT, INSERT, UPDATE ON public."Scores" TO mewingpad_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON public."Playlists" TO mewingpad_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON public."PlaylistsAudiotracks" TO mewingpad_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON public."TagsAudiotracks" TO mewingpad_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON public."Commentaries" TO mewingpad_user;

CREATE ROLE mewingpad_guest
    NOSUPERUSER
    NOCREATEDB
    NOCREATEROLE
	LOGIN
    PASSWORD '$@$guest$@$'
	CONNECTION LIMIT -1;

GRANT SELECT ON public."Scores" TO mewingpad_guest;
GRANT SELECT ON public."Audiotracks" TO mewingpad_guest;
GRANT SELECT ON public."Tags" TO mewingpad_guest;
GRANT SELECT ON public."TagsAudiotracks" TO mewingpad_guest;
GRANT SELECT ON public."Commentaries" TO mewingpad_guest;
GRANT SELECT ON public."Users" TO mewingpad_guest;