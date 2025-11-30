using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class SmokerRuleTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    [DataTestMethod]
    [DataRow(false, 0.0)]   // Non-smoker
    [DataRow(true, 0.10)]   // Smoker (10%)
    public void Evaluate_Should_ApplySurcharge_When_SmokerStatusVaries(bool isSmoker, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(30, isSmoker, 70m, 1.75m));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
