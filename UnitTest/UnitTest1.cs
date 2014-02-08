using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
	[TestClass]
	public class UnitTest1
	{
		public UnitTest1()
		{
			this.Init();
		}

		protected TestModel Init()
		{
			return new TestModel();
		}

		[TestMethod]
		public void TestCustomValidatorInt()
		{
			var model = this.Init();
			model.Id = 10;
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(tm => tm.Id).CustomValidator(v => v > 5, "Must greater than 5.").End();
			Assert.IsTrue(validator.IsValid(), "Custom validator on int failed");
		}

		[TestMethod]
		public void TestStringRequiredValidation_NotNull()
		{
			var model = this.Init();
			model.Str = DateTime.Now.ToString();
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Str).Required().End();
			Assert.IsTrue(validator.IsValid(), "string required validation failed when string is not null.");
		}

		[TestMethod]
		public void TestStringRequiredValidation_IsNull()
		{
			var model = this.Init();
			model.Str = null;
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Str).Required().End();
			Assert.IsFalse(validator.IsValid(), "string required validation failed when string is null.");
		}

		[TestMethod]
		public void TestStringValidation_MaxLength()
		{
			var model = this.Init();
			model.Str = "abcdefg";
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Str).MaxLength(10).End();
			Assert.IsTrue(validator.IsValid(), "string max length validation failed.");
		}

		[TestMethod]
		public void TestStringValidation_MaxLength_2()
		{
			var model = this.Init();
			model.Str = "abcdefghijklmnopqrst";
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Str).MaxLength(10).End();
			Assert.IsFalse(validator.IsValid(), "string max length validation failed.");
		}

		[TestMethod]
		public void TestStringValidation_Regex()
		{
			var model = this.Init();
			model.Str = "sn12341";
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Str).MatchRegex(@"sn\d{5,6}", System.Text.RegularExpressions.RegexOptions.Singleline, "Invalid str format").End();
			Assert.IsTrue(validator.IsValid(), "Regex validation failed.");
			model.Str = "sa12345";
			Assert.IsFalse(validator.IsValid(true), "Regex validation failed.");
		}

		[TestMethod]
		public void TestComplexRule() {
			var model = this.Init();
			model.Id = 100;
			model.Str = "abcdefg123dasd";
			var validator = ModelValidator.ModelValidator.CreateRule(model).RuleFor(q => q.Id).Required().RuleFor(q => q.Str).MatchRegex(".{10}").When(q => q.Id > 100).End();
			Assert.IsTrue(validator.IsValid(), "");
		}
	}

	public class TestModel
	{
		public int Id { get; set; }
		public string Str { get; set; }
	}
}
