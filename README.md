# .NET Standard Postgres Data Exporter & Importer
Exporting large amounts of data where the sql file is too big to execute or takes too long. This class library generates insert statements in 500 row increments per sql file. Tables in pg_catalog & information_schema gets excluded.

## Getting started
To get you the database running just execute the following in cmd/bash:
```
docker-compose up
```