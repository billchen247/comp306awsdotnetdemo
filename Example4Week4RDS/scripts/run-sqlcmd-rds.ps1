Param(
    [Parameter(Mandatory=$true)] [string]$Server,
    [Parameter(Mandatory=$true)] [string]$User,
    [Parameter(Mandatory=$false)] [string]$File = "sql/create_tododb.sql"
)


# Ensure sqlcmd is available
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    Write-Error "sqlcmd not found. Install the SQL Server command-line tools (sqlcmd) or use SSMS/Azure Data Studio."
    exit 1
}

# Prompt for password securely
$secure = Read-Host -Prompt "Enter DB password" -AsSecureString
$ptr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure)
$password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($ptr)

try {
    Write-Host "Running script against server: $Server"
    # Use -N to encrypt and -C to trust server certificate if necessary for RDS
    sqlcmd -S "$Server,1433" -U $User -P $password -i $File -N -C
} finally {
    # Clean up sensitive memory
    [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr)
}

# Script notes:
# - This helper prompts for the password and avoids writing the password on the
#   command line or embedding it in the script. On CI systems, pass credentials
#   from secured secrets instead of interactive prompting.
# - The -N and -C flags enable encryption and trust server certificate which are
#   commonly needed for AWS RDS SQL Server endpoints. Remove -C if you prefer to
#   validate server certificates.
