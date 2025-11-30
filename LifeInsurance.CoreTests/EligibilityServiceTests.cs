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
    [DataRow(30, true, 100, 1.75, 0.25)]  // Smoker + Obese
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
    [DataRow(80, false, 100, 1.75, 0.30)] // Age + Obese
    [DataRow(80, true, 50, 1.75, 0.35)]   // Age + Smoker + Underweight
    [DataRow(80, true, 100, 1.75, 0.40)]  // Age + Smoker + Obese
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
    [DataRow(80, false, 70, 1.75, ChronicDisease.Diabetes, 0.35)]      // Age + Diabetes
    [DataRow(80, false, 70, 1.75, ChronicDisease.Hypertension, 0.30)]  // Age + Hypertension
    [DataRow(80, false, 70, 1.75, ChronicDisease.Dyslipidemia, 0.25)]  // Age + Dyslipidemia
    // Smoker + Diseases
    [DataRow(30, true, 70, 1.75, ChronicDisease.Diabetes, 0.30)]       // Smoker + Diabetes
    [DataRow(30, true, 70, 1.75, ChronicDisease.Hypertension, 0.25)]   // Smoker + Hypertension
    // BMI + Diseases
    [DataRow(30, false, 50, 1.75, ChronicDisease.Diabetes, 0.30)]      // Underweight + Diabetes
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

    #region Complex 4+ Factors Combinations

    [DataTestMethod]
    [DataRow(80, true, 50, 1.75, ChronicDisease.Diabetes, 0.55)]       // Age + Smoker + Underweight + Diabetes
    [DataRow(30, true, 100, 1.75, ChronicDisease.Hypertension, 0.40)]  // Smoker + Obese + Hypertension (young)
    public void Evaluate_Should_CombineSurcharges_When_ComplexMultipleFactors(
        int age, bool isSmoker, double weightKg, double heightM, ChronicDisease diseases, double expectedSurcharge) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));

        // Assert
        Assert.IsTrue(result.IsEligible);
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }

    #endregion
}