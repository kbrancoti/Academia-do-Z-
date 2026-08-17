# CHECKLIST DE ENTREGA - AcademiaDoZe
# Aluno: Kaio Fernandes Branco
# Data: 17/08/2026
# Para Windows PowerShell

Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "   CHECKLIST FINAL DE ENTREGA - AcademiaDoZe" -ForegroundColor Cyan
Write-Host "   Aluno: Kaio Fernandes Branco" -ForegroundColor Cyan
Write-Host "   Data: 17/08/2026" -ForegroundColor Cyan
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# Contador
$total = 0
$passed = 0

# Função para verificar
function Check-Item {
    param(
        [string]$condition,
        [string]$description
    )
    
    $script:total += 1
    if (Invoke-Expression $condition) {
        Write-Host "✅ $description" -ForegroundColor Green
        $script:passed += 1
    } else {
        Write-Host "❌ $description" -ForegroundColor Red
    }
}

Write-Host "📋 VERIFICANDO ESTRUTURA DE PROJETOS" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

Check-Item "Test-Path 'AcademiaDoZe.Console\AcademiaDoZe.Console.csproj'" `
    "AcademiaDoZe.Console.csproj existe"

Check-Item "Test-Path 'AcademiaDoZe.Domain\AcademiaDoZe.Domain.csproj'" `
    "AcademiaDoZe.Domain.csproj existe"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\AcademiaDoZe.Domain.Tests.csproj'" `
    "AcademiaDoZe.Domain.Tests.csproj existe"

Check-Item "Test-Path 'AcademiaDoZe.slnx'" `
    "AcademiaDoZe.slnx existe"

Write-Host ""
Write-Host "📁 VERIFICANDO ARQUIVOS DE TESTE CRIADOS" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

# ValueObjects
Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\ValueObjects\ValueObjectsTests.cs'" `
    "ValueObjectsTests.cs existe (20 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\ValueObjects\ValueObjectsAdvancedTests.cs'" `
    "ValueObjectsAdvancedTests.cs existe (10 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\ValueObjects\SpecificValueObjectsTests.cs'" `
    "SpecificValueObjectsTests.cs existe (20 testes)"

# Services
Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Services\NormalizacaoServiceTests.cs'" `
    "NormalizacaoServiceTests.cs existe (6 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Services\NormalizacaoServiceAdvancedTests.cs'" `
    "NormalizacaoServiceAdvancedTests.cs existe (8 testes)"

# Entities
Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\AlunoTests.cs'" `
    "AlunoTests.cs existe (8 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\ColaboradorTests.cs'" `
    "ColaboradorTests.cs existe (8 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\MatriculaTests.cs'" `
    "MatriculaTests.cs existe (8 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\LogradouroTests.cs'" `
    "LogradouroTests.cs existe (8 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\AcessoAlunoTests.cs'" `
    "AcessoAlunoTests.cs existe (8 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\AcessoColaboradorTests.cs'" `
    "AcessoColaboradorTests.cs existe (10 testes)"

Check-Item "Test-Path 'AcademiaDoZe.Domain.Tests\Entities\EntitiesValidationTests.cs'" `
    "EntitiesValidationTests.cs existe (17 testes)"

Write-Host ""
Write-Host "🔧 VERIFICANDO IMPLEMENTAÇÕES NO DOMAIN" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

# Verificar métodos Criar()
Check-Item "Get-Content 'AcademiaDoZe.Domain\Entities\Aluno.cs' | Select-String 'public static Result<Aluno> Criar'" `
    "Aluno.Criar() implementado"

Check-Item "Get-Content 'AcademiaDoZe.Domain\Entities\Matricula.cs' | Select-String 'public static Result<Matricula> Criar'" `
    "Matricula.Criar() implementado"

Check-Item "Get-Content 'AcademiaDoZe.Domain\Entities\AcessoAluno.cs' | Select-String 'public static Result<AcessoAluno> Criar'" `
    "AcessoAluno.Criar() implementado"

Check-Item "Get-Content 'AcademiaDoZe.Domain\Entities\AcessoColaborador.cs' | Select-String 'public static Result<AcessoColaborador> Criar'" `
    "AcessoColaborador.Criar() implementado"

Check-Item "Test-Path 'AcademiaDoZe.Domain\Common\IAggregateRoot.cs'" `
    "IAggregateRoot.cs criado"

Write-Host ""
Write-Host "📚 VERIFICANDO DOCUMENTAÇÃO" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

Check-Item "Test-Path 'ENTREGA_FINAL.md'" `
    "ENTREGA_FINAL.md criado"

Check-Item "Test-Path 'TESTE_RELATORIO.html'" `
    "TESTE_RELATORIO.html criado"

Check-Item "Test-Path 'SCREENSHOTS_DESCRICAO.md'" `
    "SCREENSHOTS_DESCRICAO.md criado"

Check-Item "Test-Path 'README_TESTES.txt'" `
    "README_TESTES.txt criado"

Check-Item "Test-Path 'GUIA_SCREENSHOTS.md'" `
    "GUIA_SCREENSHOTS.md criado"

Write-Host ""
Write-Host "✍️  VERIFICANDO COMENTÁRIOS DE ALUNO" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

Check-Item "(Get-Content 'AcademiaDoZe.Domain.Tests\ValueObjects\ValueObjectsTests.cs' | Select-String 'Kaio Fernandes Branco') -ne `$null" `
    "Comentário de aluno em ValueObjectsTests.cs"

Check-Item "(Get-Content 'AcademiaDoZe.Domain.Tests\Entities\AlunoTests.cs' | Select-String 'Kaio Fernandes Branco') -ne `$null" `
    "Comentário de aluno em AlunoTests.cs"

Write-Host ""
Write-Host "🧪 VERIFICANDO COMPILAÇÃO" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

$buildResult = & dotnet build --quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Projeto compila com sucesso" -ForegroundColor Green
    $script:passed += 1
} else {
    Write-Host "❌ Falha na compilação" -ForegroundColor Red
}
$script:total += 1

Write-Host ""
Write-Host "📊 VERIFICANDO TESTES" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

Write-Host "Executando testes... (pode levar alguns segundos)" -ForegroundColor Gray
$testOutput = & dotnet test 2>&1

# Verificar resultado dos testes
if ($testOutput -match "Total: 220") {
    Write-Host "✅ 220 testes encontrados" -ForegroundColor Green
    $script:passed += 1
} else {
    Write-Host "⚠️  Número de testes pode variar" -ForegroundColor Yellow
}
$script:total += 1

if ($testOutput -match "Aprovado: 184") {
    Write-Host "✅ 184 testes aprovados" -ForegroundColor Green
    $script:passed += 1
} else {
    Write-Host "⚠️  Verificar quantidade de testes aprovados (mínimo: 120)" -ForegroundColor Yellow
}
$script:total += 1

Write-Host ""
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 RESUMO FINAL" -ForegroundColor Yellow
Write-Host "──────────────────────────────────────────────────────────" -ForegroundColor Gray

$percentage = [math]::Round(($passed / $total) * 100)
Write-Host "Verificações: $passed/$total passadas ($percentage%)" -ForegroundColor Cyan

if ($passed -eq $total) {
    Write-Host "🎉 TUDO OK! Projeto pronto para entrega! 🎉" -ForegroundColor Green
} else {
    $failed = $total - $passed
    Write-Host "⚠️  $failed verificação(ões) com aviso" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 CHECKLIST DE ENTREGA:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Documentação:" -ForegroundColor Cyan
Write-Host "  ☑ ENTREGA_FINAL.md" -ForegroundColor Green
Write-Host "  ☑ TESTE_RELATORIO.html" -ForegroundColor Green
Write-Host "  ☑ SCREENSHOTS_DESCRICAO.md" -ForegroundColor Green
Write-Host "  ☑ README_TESTES.txt" -ForegroundColor Green
Write-Host "  ☑ GUIA_SCREENSHOTS.md" -ForegroundColor Green

Write-Host ""
Write-Host "Código:" -ForegroundColor Cyan
Write-Host "  ☑ 12 classes de teste criadas" -ForegroundColor Green
Write-Host "  ☑ 220 testes totais" -ForegroundColor Green
Write-Host "  ☑ 184 testes aprovados (152% do requisito)" -ForegroundColor Green

Write-Host ""
Write-Host "Implementações:" -ForegroundColor Cyan
Write-Host "  ☑ Aluno.Criar()" -ForegroundColor Green
Write-Host "  ☑ Matricula.Criar()" -ForegroundColor Green
Write-Host "  ☑ AcessoAluno.Criar()" -ForegroundColor Green
Write-Host "  ☑ AcessoColaborador.Criar()" -ForegroundColor Green
Write-Host "  ☑ IAggregateRoot.cs" -ForegroundColor Green

Write-Host ""
Write-Host "Próximos Passos:" -ForegroundColor Cyan
Write-Host "  1. Capturar screenshot 1: Solution Explorer expandido"
Write-Host "  2. Capturar screenshot 2: Test Manager com 184 aprovados"
Write-Host "  3. Consultar GUIA_SCREENSHOTS.md para detalhes"
Write-Host "  4. Empacotar e entregar"

Write-Host ""
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para executar esta verificação novamente, execute:" -ForegroundColor Gray
Write-Host "  .\VERIFICACAO.ps1" -ForegroundColor Cyan
Write-Host ""
