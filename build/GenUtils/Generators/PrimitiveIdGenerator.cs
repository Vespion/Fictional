using Fictional.Build.GenUtils.Emitters.PrimitiveId;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGeneratorUtils;

namespace Fictional.Build.GenUtils.Generators;

/// <summary>
///     Generates the implementation of the "IPrimitiveId{TId, TValue}" interface for primitive ID types.
/// </summary>
[Generator]
public class PrimitiveIdGenerator : IIncrementalGenerator
{
	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var provider = context.SyntaxProvider
			.CreateSyntaxProvider(static (node, _) => node is RecordDeclarationSyntax, Transform)
			.Where(static m => m.targetFile is not null);

		context.RegisterSourceOutput(provider,
			static (spc, source) => Execute(source.targetFile!, source.typeName, spc));
	}

	private static (TypeDesc? targetFile, string typeName) Transform(GeneratorSyntaxContext syntaxContext,
		CancellationToken token)
	{
		var recordDeclaration = (RecordDeclarationSyntax)syntaxContext.Node;

		foreach (var baseTypeSyntax in recordDeclaration.BaseList?.Types ?? [])
		{
			if (!baseTypeSyntax.Type.IsKind(SyntaxKind.GenericName)) continue;

			var symbol = syntaxContext.SemanticModel.GetSymbolInfo(baseTypeSyntax.Type).Symbol;
			if (symbol?.Name != "IPrimitiveId") continue;

			var recordSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(recordDeclaration);

			if (recordSymbol == null) return (null, string.Empty);

			return (TypeDesc.Create(
				recordSymbol.Name,
				recordSymbol.ContainingNamespace.ToDisplayString(),
				recordSymbol.TypeKind,
				isRecord: recordSymbol.IsRecord,
				isPartial: true,
				isReadOnly: recordSymbol.IsReadOnly,
				accessibility: recordSymbol.DeclaredAccessibility
			), ((INamedTypeSymbol)symbol).TypeArguments[1].Name);
		}

		return (null, string.Empty);
	}

	private static void Execute(TypeDesc targetDesc, string targetType, SourceProductionContext spc)
	{
		var options = new SourceFileEmitterOptions
			{ AssemblyName = typeof(PrimitiveIdGenerator).Assembly.GetName(), UseFileScopedNamespace = true };

		DefaultSourceCodeEmitter targetEmitter;
		switch (targetType)
		{
			case "Guid":
				targetEmitter = new GuidPrimitiveEmitter();
				break;
			case "UInt64":
				targetEmitter = new UInt64PrimitiveEmitter();
				break;
			case "Int64":
				targetEmitter = new Int64PrimitiveEmitter();
				break;
			case "String":
				targetEmitter = new StringPrimitiveEmitter();
				break;
			default:
				return;
		}

		var sourceEmitter = new DefaultSourceFileEmitter(options)
		{
			SourceCodeEmitters =
			[
				targetEmitter
			]
		};

		var sourceBuilder = new SourceBuilder()
			.Register(sourceEmitter, DefaultGenerationSpec.CreateFrom(targetDesc));

		foreach (var sourceFile in sourceBuilder.SourceFiles)
			spc.AddSource(sourceFile.Key, sourceFile.Value.ToSourceText());
	}
}