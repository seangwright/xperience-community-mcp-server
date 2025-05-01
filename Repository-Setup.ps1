## Delete me after project setup!

param (
    [string]$ProjectName
)

# Validate that the project name is provided
if (-not $ProjectName) {
    Write-Host "Please provide a valid Project Name. Example: Kentico.Xperience.Lucene"
    exit 1
}

$searchText = "Kentico.Xperience.RepoTemplate"
$replaceText = "$ProjectName"

$files = Get-ChildItem -Path "./" `
 -Recurse:$true | Where-Object {
    @(".json", ".yml", ".props", ".md") -contains $_.Extension
}

foreach ($file in $files) {
    # Read the file content
    $content = Get-Content -Path $file.FullName

    # Check if the file contains the search text
    if ($content -like "*Kentico.Xperience.RepoTemplate*") {
        Write-Host "Processing file: $($file.FullName)"
        
        # Replace the text in the file content
        $newContent = $content -replace $searchText, $replaceText

        # Write the updated content back to the file
        Set-Content -Path $file.FullName -Value $newContent

        Write-Host "Replaced text in file: $($file.FullName)"
    }
}

# Define project directories
$srcProjectPath = Join-Path "./src" $ProjectName
$testProjectPath = Join-Path "./tests" "$ProjectName.Tests"
$examplesProjectPath = Join-Path "./examples" "DancingGoat"

dotnet new classlib `
    -n $ProjectName `
    -o $srcProjectPath `
    --no-restore
Write-Host "Created class library project: $srcProjectPath"

dotnet new nunit `
    -n "$ProjectName.Tests" `
    -o $testProjectPath `
    --no-restore
Write-Host "Created NUnit test project: $testProjectPath"

dotnet new kentico-xperience-sample-mvc -n DancingGoat -o $examplesProjectPath --no-restore --allow-scripts Yes
Write-Host "Created Dancing Goat sample application: $examplesProjectPath"

dotnet add "$testProjectPath/$ProjectName.Tests.csproj" `
    reference $srcProjectPath
Write-Host "Added reference from test project to class library project."

dotnet add "$examplesProjectPath/DancingGoat.csproj" `
    reference $srcProjectPath
Write-Host "Added reference from Dancing Goat project to class library project."

dotnet new sln -n "$ProjectName"
dotnet sln add $srcProjectPath
dotnet sln add $testProjectPath
dotnet sln add $examplesProjectPath

Write-Host "Project setup complete."
