using Stride.Core.CompilerServices.Analyzers;
using Xunit;

namespace Stride.Core.CompilerServices.Tests.AnalyzerTests;

public class STRDIAG003_Test
{
    [Fact]
    public async Task Error_On_Datamember_With_private_InaccessibleMember_On_Property()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] private int Value { get; set; }");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }

    [Fact]
    public async Task Error_On_Datamember_With_private_InaccessibleMember_On_Field()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] private int Value = 0;");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }

    [Fact]
    public async Task Error_On_Datamember_With_protected_InaccessibleMember_On_Property()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] protected int Value { get; set; }");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }

    [Fact]
    public async Task Error_On_Datamember_With_protected_InaccessibleMember_On_Field()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] protected int Value = 0;");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }

    [Fact]
    public async Task Error_On_Datamember_With_private_protected_InaccessibleMember_On_Property()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] private protected int Value { get; set; }");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }

    [Fact]
    public async Task Error_On_Datamember_With_private_protected_InaccessibleMember_On_Field()
    {
        string sourceCode = string.Format(ClassTemplates.BasicClassTemplate, "[DataMember] private protected int Value = 0;");
        await TestHelper.ExpectDiagnosticsErrorAsync(sourceCode, STRDIAG003InaccessibleMember.DiagnosticId);
    }
}
