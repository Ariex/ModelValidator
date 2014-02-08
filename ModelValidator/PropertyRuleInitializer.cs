using System;
using System.Linq.Expressions;

namespace ModelValidator
{
	public class PropertyRuleInitializer<TBase, TProp>
	{
		private PropertyRuleCollection<TBase> propRuleCollector;
		private System.Linq.Expressions.Expression<Func<TBase, TProp>> propSelector;
		private ValidationRule<TBase> currentRule;

		private TProp getPropValue()
		{
			//Console.WriteLine(this.propSelector.Compile()(this.propRuleCollector.GetBaseObject()));
			return this.propSelector.Compile()(this.propRuleCollector.GetBaseObject());
		}

		public PropertyRuleInitializer(PropertyRuleCollection<TBase> initializer, System.Linq.Expressions.Expression<Func<TBase, TProp>> propSelector)
		{
			this.propRuleCollector = initializer;
			this.propSelector = propSelector;
		}

		public PropertyRuleInitializer<TBase, TProp> MaxLength(int maxLength, string errorMsg = null)
		{
			Expression<Func<bool>> exp = () =>
				this.getPropValue().ToString().Length <= maxLength;
			this.currentRule = this.propRuleCollector.AddValidationRule(new ValidationRule<TBase>()
			{
				ValidationMethod = exp,
				ErrorMessage = errorMsg ?? "Reach max length",
				PropertyValue = this.getPropValue(),
				PropertyName = (this.propSelector.Body as MemberExpression).Member.Name
			});
			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> When(Expression<Func<TBase, bool>> whenCondition)
		{
			if (this.currentRule == null)
			{
				throw new Exception("A rule must be set before setup rule conditions.");
			}
			this.currentRule.AddPreCondition(whenCondition);
			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> And(Expression<Func<bool>> condition)
		{
			if (this.currentRule == null)
			{
				throw new Exception("A rule must be set before setup rule conditions.");
			}

			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> MinLength(int minLength, string errorMsg = null)
		{
			Expression<Func<bool>> exp = () => this.getPropValue().ToString().Length >= minLength;
			this.currentRule = this.propRuleCollector.AddValidationRule(new ValidationRule<TBase>()
			{
				ValidationMethod = exp,
				ErrorMessage = errorMsg ?? "Reach min length",
				PropertyValue = this.getPropValue(),
				PropertyName = (this.propSelector.Body as MemberExpression).Member.Name
			});
			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> MatchRegex(string regex, System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Singleline, string errorMsg = null)
		{
			var reg = new System.Text.RegularExpressions.Regex(regex, options);
			Expression<Func<bool>> exp = () => 
				reg.IsMatch(
				this.getPropValue().ToString()
				);
			this.currentRule = this.propRuleCollector.AddValidationRule(new ValidationRule<TBase>()
			{
				ValidationMethod = exp,
				ErrorMessage = errorMsg ?? string.Format("Does not match \"{0}\" with option: {1}", regex, options.ToString()),
				PropertyValue = this.getPropValue(),
				PropertyName = (this.propSelector.Body as MemberExpression).Member.Name
			});
			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> CustomValidator(Func<TProp, bool> validator, string errorMsg)
		{
			Expression<Func<bool>> exp = () => validator(this.getPropValue());
			this.currentRule = this.propRuleCollector.AddValidationRule(new ValidationRule<TBase>()
			{
				ValidationMethod = exp,
				ErrorMessage = errorMsg,
				PropertyName = (this.propSelector.Body as MemberExpression).Member.Name,
				PropertyValue = this.getPropValue()
			});
			return this;
		}

		public PropertyRuleInitializer<TBase, TProp> Required(string errorMsg = "{0} is required")
		{
			this.currentRule = this.propRuleCollector.AddValidationRule(new ValidationRule<TBase>()
			{
				PropertyName = (this.propSelector.Body as MemberExpression).Member.Name,
				PropertyValue = this.getPropValue(),
				ValidationMethod = () => !System.Collections.Generic.EqualityComparer<TProp>.Default.Equals(this.getPropValue(), default(TProp))
			});
			this.currentRule.ErrorMessage = string.Format(errorMsg, this.currentRule.PropertyName, this.currentRule.PropertyValue);
			return this;
		}

		public PropertyRuleCollection<TBase> End()
		{
			return this.propRuleCollector;//.End();
		}

		public PropertyRuleInitializer<TBase, TProperty> RuleFor<TProperty>(System.Linq.Expressions.Expression<Func<TBase, TProperty>> propSelector)
		{
			return this.propRuleCollector.RuleFor<TProperty>(propSelector);
		}
	}
}
