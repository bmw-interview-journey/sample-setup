#!/usr/bin/env bash

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to extract target framework from csproj files
get_target_framework() {
    local csproj_file="$1"
    if [[ -f "$csproj_file" ]]; then
        # Extract TargetFramework value using grep and sed
        local framework=$(grep -oP '<TargetFramework>\K[^<]*' "$csproj_file" 2>/dev/null || echo "")
        if [[ -z "$framework" ]]; then
            # Try alternative pattern for TargetFrameworks (plural)
            framework=$(grep -oP '<TargetFrameworks>\K[^<]*' "$csproj_file" 2>/dev/null | cut -d';' -f1 || echo "")
        fi
        echo "$framework"
    fi
}

# Function to convert framework version to SDK version
framework_to_sdk_version() {
    local framework="$1"
    case "$framework" in
        net9.0) echo "9.0" ;;
        net8.0) echo "8.0" ;;
        net7.0) echo "7.0" ;;
        net6.0) echo "6.0" ;;
        net5.0) echo "5.0" ;;
        netcoreapp3.1) echo "3.1" ;;
        netcoreapp3.0) echo "3.0" ;;
        netcoreapp2.2) echo "2.2" ;;
        netcoreapp2.1) echo "2.1" ;;
        *) echo "unknown" ;;
    esac
}

print_status "=== .NET Interview Setup Checker ==="
echo

# Check if .NET CLI is installed
print_status "Checking for .NET CLI..."
if ! command -v dotnet &> /dev/null; then
    print_error ".NET CLI not found. Please install it from https://dotnet.microsoft.com/download"
    exit 1
fi
print_success ".NET CLI is installed"

# Show installed .NET version
print_status "Installed .NET CLI version:"
dotnet --version
echo

# Find solution file
SOLUTION_FILE=""
if [[ -f "InterviewSetup.sln" ]]; then
    SOLUTION_FILE="InterviewSetup.sln"
elif [[ -f "InterviewSetup/InterviewSetup.sln" ]]; then
    SOLUTION_FILE="InterviewSetup/InterviewSetup.sln"
else
    print_error "Could not find InterviewSetup.sln file"
    exit 1
fi

print_status "Found solution file: $SOLUTION_FILE"

# Find project files and detect required SDK version
print_status "Detecting required .NET SDK version from project files..."

REQUIRED_FRAMEWORKS=()
PROJECT_FILES=($(find . -name "*.csproj" 2>/dev/null))

if [[ ${#PROJECT_FILES[@]} -eq 0 ]]; then
    print_error "No .csproj files found in the project"
    exit 1
fi

for proj_file in "${PROJECT_FILES[@]}"; do
    framework=$(get_target_framework "$proj_file")
    if [[ -n "$framework" ]]; then
        REQUIRED_FRAMEWORKS+=("$framework")
        print_status "Found project: $(basename "$proj_file") -> Target Framework: $framework"
    fi
done

if [[ ${#REQUIRED_FRAMEWORKS[@]} -eq 0 ]]; then
    print_error "Could not detect target framework from project files"
    exit 1
fi

# Get unique frameworks and convert to SDK versions
UNIQUE_FRAMEWORKS=($(printf "%s\n" "${REQUIRED_FRAMEWORKS[@]}" | sort -u))
REQUIRED_SDK_VERSIONS=()

for framework in "${UNIQUE_FRAMEWORKS[@]}"; do
    sdk_version=$(framework_to_sdk_version "$framework")
    if [[ "$sdk_version" != "unknown" ]]; then
        REQUIRED_SDK_VERSIONS+=("$sdk_version")
        print_status "Required .NET SDK version for $framework: $sdk_version"
    else
        print_warning "Unknown framework version: $framework"
    fi
done

# Check if required SDK versions are installed
print_status "Checking installed .NET SDKs..."
INSTALLED_SDKS=$(dotnet --list-sdks 2>/dev/null || echo "")

if [[ -z "$INSTALLED_SDKS" ]]; then
    print_error "No .NET SDKs found installed"
    exit 1
fi

echo "Installed SDKs:"
echo "$INSTALLED_SDKS"
echo

# Verify each required SDK version is installed
ALL_SDKS_FOUND=true
for required_version in "${REQUIRED_SDK_VERSIONS[@]}"; do
    if echo "$INSTALLED_SDKS" | grep -q "^$required_version\."; then
        print_success "Required .NET SDK $required_version is installed"
    else
        print_error "Required .NET SDK $required_version is NOT installed"
        ALL_SDKS_FOUND=false
    fi
done

if [[ "$ALL_SDKS_FOUND" != true ]]; then
    print_error "Missing required .NET SDK versions. Please install them from https://dotnet.microsoft.com/download"
    exit 1
fi

echo

# Restore NuGet packages
print_status "Restoring NuGet packages..."
if dotnet restore "$SOLUTION_FILE"; then
    print_success "NuGet packages restored successfully"
else
    print_error "Failed to restore NuGet packages"
    exit 1
fi

echo

# Build the solution
print_status "Building the solution..."
if dotnet build "$SOLUTION_FILE" --no-restore; then
    print_success "Solution built successfully"
else
    print_error "Failed to build the solution"
    exit 1
fi

echo

# Run unit tests
print_status "Running unit tests..."
if dotnet test "$SOLUTION_FILE" --no-build --no-restore --verbosity minimal; then
    print_success "All unit tests passed"
else
    print_error "Some unit tests failed"
    exit 1
fi

echo
print_success "=== All checks passed! Your development environment is ready. ==="
