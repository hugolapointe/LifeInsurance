using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class AgeRuleTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    [DataTestMethod]
    [DataRow(17, "Applicant must be at least 18 years old")]
    [DataRow(100, "Applicant must be under 100 years old")]
    public void Evaluate_Should_RejectApplicant_When_AgeIsInvalid(int age, string rejectionReason) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, false, 70m, 1.75m));

        // Assert
        Assert.IsFalse(result.IsEligible);
        Assert.AreEqual(rejectionReason, result.RejectionReason);
    }

    [DataTestMethod]
    [DataRow(18, 0.0)]   // Min age
    [DataRow(50, 0.0)]   // Standard
    [DataRow(75, 0.0)]   // Max standard
    [DataRow(76, 0.15)]  // Elderly (15%)
    [DataRow(99, 0.15)]  // Near max
    public void Evaluate_Should_ApplySurcharge_When_AgeVaries(int age, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, false, 70m, 1.75m));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
