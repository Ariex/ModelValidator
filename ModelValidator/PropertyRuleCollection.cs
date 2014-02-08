using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelValidator
{
	public class PropertyRuleCollection<TBase>
	{
		private ModelValidator<TBase> validatorBuilder;
		public List<ValidationRule<TBase>> ValidationRules = new List<ValidationRule<TBase>>();
		private TBase validateObject;

		public ValidationRule<TBase> AddValidationRule(ValidationRule<TBase> rule)
		{
			this.ValidationRules.Add(rule);
			return rule;
		}

		public PropertyRuleCollection(ModelValidator<TBase> validator, TBase validateObject)
		{
			this.validatorBuilder = validator;
			this.validateObject = validateObject;
		}

		public TBase GetBaseObject()
		{
			return this.validateObject;
		}

		private bool? _isvalid = null;

		public bool IsValid(bool shouldForceReevaluate = false, bool shouldStopWhenFailed = true)
		{
			if (shouldForceReevaluate || !this._isvalid.HasValue)
			{
				this._isvalid = shouldStopWhenFailed ? !this.ValidationRules.Any(r => r.Validate(this.validateObject, shouldForceReevaluate) == false) : this.ValidationRules.Aggregate(true, (a, b) => a || b.Validate(this.validateObject, shouldForceReevaluate));
			}
			return this._isvalid.Value;
		}

		public List<ValidationRule<TBase>> ValidationSummary
		{
			get
			{
				return this.ValidationRules;
			}
		}

		public ModelValidator<TBase> End()
		{
			return this.validatorBuilder;
		}

		public PropertyRuleInitializer<TBase, TProperty> RuleFor<TProperty>(System.Linq.Expressions.Expression<Func<TBase, TProperty>> expression)
		{
			var propRule = new PropertyRuleInitializer<TBase, TProperty>(this, expression);
			return propRule;
		}
	}
}
