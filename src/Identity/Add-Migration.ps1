param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

$migrationProject = Join-Path $PSScriptRoot "./DbMigration/"
$startupProject = Join-Path $PSScriptRoot "./DbMigration/"
$outputDir = "Migrations"

Write-Host "Adding migration '$Name'..."

dotnet ef migrations add $Name `
    --context IdentityDbContext `
    --project $migrationProject `
    --startup-project $startupProject `
    --output-dir $outputDir

Write-Host "Done."
