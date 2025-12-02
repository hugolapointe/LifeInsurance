using LifeInsurance.Core;
using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;


[TestClass]
public class EligibilityServiceTests {
    private EligibilityService Service = null!;

    [TestInitialize]
    public void Setup() {
        Service = new EligibilityService();
    }

    #region Smoker + BMI Combinations

    [DataTestMethod]
    [DataRow(30, true, 70, 1.75, 0.10)]   // Smoker only
    [DataRow(30, false, 50, 1.75, 0.10)]  // Underweight only
    [DataRow(30, false, 92, 1.75, 0.15)]  // Obese only
    [DataRow(30, true, 50, 1.75, 0.20)]   // Smoker + Underweight
    public void Evaluate_Should_CombineSurcharges_When_SmokerAndBmi(
        int age, bool isSmoker, double weightKg, double heightM, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    #endregion

    #region Age + Smoker + BMI Combinations

    [DataTestMethod]
    [DataRow(80, true, 70, 1.75, 0.25)]   // Age + Smoker
    [DataRow(80, false, 50, 1.75, 0.25)]  // Age + Underweight
    [DataRow(80, true, 50, 1.75, 0.35)]   // Age + Smoker + Underweight
    public void Evaluate_Should_CombineSurcharges_When_AgeWithSmokerAndBmi(
        int age, bool isSmoker, double weightKg, double heightM, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    #endregion

    #region Age/Smoker/BMI + Diseases Combinations

    [DataTestMethod]
    // Age + Diseases
    [DataRow(80, false, 70, 1.75, ChronicDisease.Diabetes, 0.35)]      // Age + Diabetes (sans hypertension)
    [DataRow(80, false, 70, 1.75, ChronicDisease.Hypertension, 0.30)]  // Age + Hypertension (sans diabète)
    [DataRow(80, false, 70, 1.75, ChronicDisease.Dyslipidemia, 0.25)]  // Age + Dyslipidemia
    // Smoker + Diseases
    [DataRow(30, true, 70, 1.75, ChronicDisease.Diabetes, 0.30)]     // Smoker + Diabetes (jeune, sans hypertension)
    [DataRow(30, true, 70, 1.75, ChronicDisease.Hypertension, 0.25)]   // Smoker + Hypertension (jeune, sans diabète)
    // BMI + Diseases
    [DataRow(30, false, 50, 1.75, ChronicDisease.Diabetes, 0.30)]      // Underweight + Diabetes (pas obésité)
    [DataRow(30, false, 50, 1.75, ChronicDisease.Hypertension, 0.25)]  // Underweight + Hypertension
    public void Evaluate_Should_CombineSurcharges_When_LifestyleWithDiseases(
        int age, bool isSmoker, double weightKg, double heightM, ChronicDisease diseases, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    #endregion

    #region Complex Safe Combinations

    [DataTestMethod]
    // Safe complex combinations (no dangerous patterns)
    [DataRow(80, true, 50, 1.75, ChronicDisease.Diabetes, 0.55)]       // Age + Smoker + Underweight + Diabetes (pas obèse, pas hypertension)
    [DataRow(30, false, 92, 1.75, ChronicDisease.Hypertension, 0.30)]  // Obese + Hypertension (jeune, pas fumeur, pas diabète)
    [DataRow(40, true, 70, 1.75, ChronicDisease.Dyslipidemia, 0.20)]   // Smoker + Dyslipidemia (pas diabète, pas hypertension)
    public void Evaluate_Should_CombineSurcharges_When_ComplexButSafeCombinations(
        int age, bool isSmoker, double weightKg, double heightM, ChronicDisease diseases, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));

        // Assert
        Assert.IsTrue(result.IsEligible, 
  $"Expected eligible but got: {result.RejectionReason}");
   Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    #endregion
}