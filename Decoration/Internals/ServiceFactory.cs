using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Tkheikkila.Microsoft.Extensions.DependencyInjection.Decoration.Internals;

public delegate object ServiceFactory(IServiceProvider provider, Type[] genericArguments);

public static class ServiceFactoryExtensions
{
	public static Expression CreateServiceFactoryExpression(this Type injectedService, ParameterExpression providerExpression, ParameterExpression genericArgumentsExpression)
	{
		if (injectedService.IsTypeDefinition)
		{
			CreateServiceFactoryExpression((provider, genericArguments) => provider.GetRequiredService(injectedService.MakeGenericType(genericArguments)), providerExpression, genericArgumentsExpression);
		}

		return Expression.Convert(
			CreateServiceFactoryExpression((provider, _) => provider.GetRequiredService(injectedService), providerExpression, genericArgumentsExpression),
			injectedService
		);
	}

	// Helper to simplify expression creation
	private static Expression CreateServiceFactoryExpression(Expression<ServiceFactory> factory, ParameterExpression provider, ParameterExpression genericArgumentsExpression)
	{
		var result = factory.Body;

		result = result.Replace(factory.Parameters[0], provider);
		result = result.Replace(factory.Parameters[1], genericArgumentsExpression);

		return result;
	}
}

public static class ExpressionExtensions
{
	[return: NotNullIfNotNull(nameof(expression))]
	public static Expression? Replace(this Expression? expression, Expression oldExpression, Expression newExpression)
	{
		return new ExpressionReplaceVisitor(oldExpression, newExpression).Visit(expression);
	}

	private sealed class ExpressionReplaceVisitor : ExpressionVisitor
	{
		private readonly Expression _oldExpression;
		private readonly Expression _newExpression;

		public ExpressionReplaceVisitor(Expression oldExpression, Expression newExpression)
		{
			_oldExpression = oldExpression;
			_newExpression = newExpression;
		}

		public override Expression? Visit(Expression? node)
		{
			return node == _oldExpression
				? _newExpression
				: base.Visit(node);
		}
	}
}
