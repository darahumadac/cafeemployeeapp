#!/bin/bash

# Run the command
echo "Setting up db ..."
/opt/mssql-tools18/bin/sqlcmd -S localhost,1433 -U SA -P "${MSSQL_SA_PASSWORD}" -l 60 -C -N \
    -i create_db_seed_data.sql -o output.txt &

# Start SQL Server
/opt/mssql/bin/sqlservr
