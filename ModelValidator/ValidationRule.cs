using System;
using System.Linq.Expressions;

namespace ModelValidator
{
	public class ValidationRule<TBase>
	{
		public Expression<Func<bool>> ValidationMethod { get; set; }
		public string ErrorMessage { get; set; }
		public string PropertyName { get; set; }
		public object PropertyValue { get; set; }

		private Func<bool> compiledMethod = null;

		private bool? _validationResult;

		public Expression<Func<TBase, bool>> PreConditions { get; private set; }

		public void AddPreCondition(Expression<Func<TBase, bool>> condition, bool and = true)
		{
			if (this.PreConditions == null)
			{
				this.PreConditions = condition;
			}
			else
			{
				var exp = and ? Expression.And(this.PreConditions, condition) : Expression.Or(this.PreConditions, condition);
				this.PreConditions = Expression.Lambda<Func<TBase, bool>>(exp);
			}
		}

		public bool IsValid { get; private set; }

		public bool Validate(TBase model, bool shouldForceReevaluate = false)
		{
			if (this.PreConditions != null)
			{
				if (!this.PreConditions.Compile()(model)) // return true if condition of this rule cannot fulfill
				{
					this.IsValid = true;
					return true;
				}
			}
			this.compiledMethod = this.compiledMethod ?? this.ValidationMethod.Compile();
			if (shouldForceReevaluate || !this._validationResult.HasValue)
			{
				this._validationResult = this.compiledMethod();
			}
			this.IsValid = this._validationResult.Value;
			return this._validationResult.Value;
		}
	}
}
