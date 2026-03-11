#!/bin/bash
set -e

echo "Applying migrations..."
./efbundle --connection "$ConnectionStrings__DefaultConnection"
echo "Migrations applied successfully."

echo "Starting application..."
exec dotnet Server.API.dll