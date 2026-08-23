using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Silo.Source;
public class BlazorCheckAccessAuthorizedPages : ISyntaxContextReceiver
{
    public IList<INamedTypeSymbol> Pages { get; set; } = new List<INamedTypeSymbol>();
   
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is PropertyDeclarationSyntax propertyDeclarationSyntax 
            && propertyDeclarationSyntax.AttributeLists.Any())
        {
            var parent = propertyDeclarationSyntax.Parent;

            if (parent is null || parent.IsKind(SyntaxKind.ClassDeclaration) is false)
            {
                return; 
            }

            var classDeclarationSyntax = (ClassDeclarationSyntax?)parent;

            if (classDeclarationSyntax?.Modifiers.Any(k => k.IsKind(SyntaxKind.PartialKeyword)) is false)
            {
                return; 
            }

            if (classDeclarationSyntax
                .AttributeLists
                .Any(list => list.Attributes.Any(a => a.Name.NormalizeWhitespace().ToFullString().Equals("AllowAnonymous"))))
            {
                return;
            }

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

            if (classSymbol is null)
            {
                return; 
            }

            if (classSymbol.Name.Contains("SiloBasePage")
                || classSymbol.Name.Contains("Login")
                || classSymbol.Name.Contains("Home"))
            {
                return;
            }

            var assemblySymbol = classSymbol.ContainingAssembly;

            if (assemblySymbol is null)
            {
                return;
            }

            if (!classSymbol.ToString().Contains(".Pages"))
            {
                return;
            }

            Pages.Add(classSymbol);
        }
    }
}
