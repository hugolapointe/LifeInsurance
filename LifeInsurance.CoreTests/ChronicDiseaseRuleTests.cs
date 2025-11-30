using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class ChronicDiseaseRuleTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    [DataTestMethod]
    [DataRow(ChronicDisease.HeartDisease)]
    [DataRow(ChronicDisease.Cancer)]
    [DataRow(ChronicDisease.KidneyDisease)]
    public void Evaluate_Should_RejectApplicant_When_HasTerminalDisease(ChronicDisease disease) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(30, false, 70m, 1.75m, disease));

        // Assert
        Assert.IsFalse(result.IsEligible);
        Assert.AreEqual("Applicant has a terminal or severe chronic disease", result.RejectionReason);
    }

    [DataTestMethod]
    [DataRow(ChronicDisease.None, 0.0)]
    [DataRow(ChronicDisease.Diabetes, 0.20)]  
    [DataRow(ChronicDisease.Hypertension, 0.15)]
    [DataRow(ChronicDisease.Dyslipidemia, 0.10)]
    public void Evaluate_Should_ApplySurcharge_When_HasChronicDisease(ChronicDisease disease, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(30, false, 70m, 1.75m, disease));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    [DataTestMethod]
    [DataRow(ChronicDisease.Diabetes | ChronicDisease.Hypertension, 0.35)]
    [DataRow(ChronicDisease.Diabetes | ChronicDisease.Dyslipidemia, 0.30)]    
    [DataRow(ChronicDisease.Hypertension | ChronicDisease.Dyslipidemia, 0.25)] 
    public void Evaluate_Should_CombineSurcharges_When_HasMultipleDiseases(ChronicDisease diseases, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(30, false, 70m, 1.75m, diseases));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
