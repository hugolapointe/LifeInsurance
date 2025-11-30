using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class HighRiskRuleTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    [DataTestMethod]
    [DataRow(30, false, 100, 1.75, ChronicDisease.Diabetes)]  // Obésité + Diabète
    [DataRow(30, false, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension | ChronicDisease.Dyslipidemia)]  // Triade métabolique
    [DataRow(80, false, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension)]  // Âge + Diabète + Hypertension
    [DataRow(80, true, 70, 1.75, ChronicDisease.Hypertension)]   // Fumeur + Hypertension + Âge
    [DataRow(80, false, 100, 1.75, ChronicDisease.Hypertension)] // Obésité + Hypertension + Âge
    public void Evaluate_Should_RejectApplicant_When_HasDangerousCombination(
       int age, bool isSmoker, double weightKg, double heightM, ChronicDisease diseases) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));

        // Assert
        Assert.IsFalse(result.IsEligible);
        Assert.AreEqual(
            "Combination of risk factors creates an unacceptably high risk profile",
            result.RejectionReason);
    }
}
