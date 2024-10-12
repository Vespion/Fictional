CREATE TABLE IF NOT EXISTS Tags
(
	Id        INTEGER PRIMARY KEY,
	Name      TEXT    NOT NULL,
	Shorthand TEXT,
	Colour    INTEGER,
	Hidden    BOOLEAN NOT NULL DEFAULT FALSE CHECK (Hidden IN (TRUE, FALSE))
);

CREATE UNIQUE INDEX IF NOT EXISTS Tags_Name_Index ON Tags (Name);

CREATE VIRTUAL TABLE IF NOT EXISTS TagSearch USING fts5
(
	Name,
	Shorthand,
	content = Tags,
	content_rowid = Id,
	tokenize = "porter unicode61 remove_diacritics 2"
);

-- Triggers to keep the FTS index up to date.
CREATE TRIGGER TagSearch_AfterInsert
	AFTER INSERT
	ON Tags
BEGIN
	INSERT INTO TagSearch(rowid, Name, Shorthand) VALUES (new.rowid, new.Name, new.Shorthand);
END;
CREATE TRIGGER TagSearch_AfterDelete
	AFTER DELETE
	ON Tags
BEGIN
	INSERT INTO TagSearch(TagSearch, rowid, Name, Shorthand) VALUES ('delete', old.rowid, old.Name, old.Shorthand);
END;
CREATE TRIGGER TagSearch_AfterUpdate
	AFTER UPDATE
	ON Tags
BEGIN
	INSERT INTO TagSearch(TagSearch, rowid, Name, Shorthand) VALUES ('delete', old.rowid, old.Name, old.Shorthand);
	INSERT INTO TagSearch(rowid, Name, Shorthand) VALUES (new.rowid, new.Name, new.Shorthand);
END;

CREATE TABLE IF NOT EXISTS TagAliasLinks
(
	CanonicalTagId INTEGER NOT NULL,
	AliasTagId     INTEGER NOT NULL,
	PRIMARY KEY (CanonicalTagId, AliasTagId),
	FOREIGN KEY (CanonicalTagId) REFERENCES Tags (Id) ON DELETE CASCADE,
	FOREIGN KEY (AliasTagId) REFERENCES Tags (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS TagGraphLinks
(
	ParentTagId INTEGER NOT NULL,
	ChildTagId  INTEGER NOT NULL,
	PRIMARY KEY (ParentTagId, ChildTagId),
	FOREIGN KEY (ParentTagId) REFERENCES Tags (Id) ON DELETE CASCADE,
	FOREIGN KEY (ChildTagId) REFERENCES Tags (Id) ON DELETE CASCADE
);