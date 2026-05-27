# deploy.ps1
Write-Host "Building project..." -ForegroundColor Yellow
ng build --configuration production

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build done!" -ForegroundColor Green
Write-Host "Opening dist folder..." -ForegroundColor Yellow

explorer "dist\lounamaro-ui\browser"

Write-Host "=======================================" -ForegroundColor Yellow
Write-Host "  Drag browser folder to Netlify:" -ForegroundColor Yellow
Write-Host "  https://app.netlify.com/sites/lunamaro/deploys" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Yellow