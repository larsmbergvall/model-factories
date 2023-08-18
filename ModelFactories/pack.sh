#!/bin/bash

PROJECT_DIR=$(pwd)
CSPROJ_FILE="$PROJECT_DIR/ModelFactories.csproj"

VERSION=$(awk -F'<Version>|<\/Version>' '/<Version>/{print $2}' "$CSPROJ_FILE")

dotnet pack -c release

mv ./bin/release/ModelFactories.Local.{$VERSION}.nupkg ../../ModelFactories.Local.{$VERSION}.nupkg
