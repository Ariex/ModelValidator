using System.Collections.Generic;
using System.Linq;

namespace ModelValidator
{
	public class ModelValidator
	{
		public static PropertyRuleCollection<TBase> CreateRule<TBase>(TBase obj)
		{
			return ModelValidator<TBase>.CreateRule(obj);
		}
	}

	public class ModelValidator<TBase>
	{
		private static ModelValidator<TBase> _instance = null;
		public static ModelValidator<TBase> Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ModelValidator<TBase>();
				}
				return _instance;
			}
		}

		private List<PropertyRuleCollection<TBase>> propRuleCollection = new List<PropertyRuleCollection<TBase>>();

		public static PropertyRuleCollection<TBase> CreateRule(TBase validateObject)
		{
			var validator = new ModelValidator<TBase>();
			var propRuleIniter = new PropertyRuleCollection<TBase>(validator, validateObject);
			// ModelValidator<TBase>.Instance.propRuleCollection.Add(propRuleIniter);
			return propRuleIniter;
		}

		public bool IsValid(bool shouldStopValidateWhenFailed = true)
		{
			return shouldStopValidateWhenFailed ?
				!this.propRuleCollection.Any(pr => pr.IsValid() == false) 
				: 
				this.propRuleCollection.Aggregate(true, (a, b) => a && b.IsValid());
		}

		public List<ValidationRule<TBase>> ValidationSummary
		{
			get
			{
				return this.propRuleCollection.Aggregate(new List<ValidationRule<TBase>>(), (a, b) => { a.AddRange(b.ValidationRules); return a; });
			}
		}
	}
}
