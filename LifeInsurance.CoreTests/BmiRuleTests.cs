using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class BmiRuleTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    [DataTestMethod]
    [DataRow(50.0, 1.75, 0.10)]   // BMI 16.3 - Underweight (10%)
    [DataRow(55.0, 1.75, 0.10)]   // BMI 18.0 - Underweight (10%)
    [DataRow(65.0, 1.75, 0.0)]    // BMI 21.2 - Normal
    [DataRow(70.0, 1.75, 0.0)]    // BMI 22.9 - Normal
    [DataRow(75.0, 1.75, 0.0)]    // BMI 24.5 - Normal
    [DataRow(92.0, 1.75, 0.15)]   // BMI 30.0 - Obese (15%)
    [DataRow(110.0, 1.75, 0.15)]  // BMI 35.9 - Obese (15%)
    public void Evaluate_Should_ApplySurcharge_When_BmiVaries(double weightKg, double heightM, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(30, false, (decimal)weightKg, (decimal)heightM));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
