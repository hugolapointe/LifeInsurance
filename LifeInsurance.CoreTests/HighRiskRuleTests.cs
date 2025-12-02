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
    // 1. Syndrome métabolique : Obésité + Diabète
    [DataRow(30, false, 100, 1.75, ChronicDisease.Diabetes)]
    // 2. Triade métabolique : Diabète + Hypertension + Dyslipidémie
    [DataRow(30, false, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension | ChronicDisease.Dyslipidemia)]
    // 3. Âge + Diabète + Hypertension
    [DataRow(80, false, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension)]
    // 4. Fumeur + Hypertension + Âge
    [DataRow(80, true, 70, 1.75, ChronicDisease.Hypertension)]
    // 5. Obésité + Hypertension + Âge
    [DataRow(80, false, 100, 1.75, ChronicDisease.Hypertension)]
    // 6. Fumeur + Obésité + Diabète
    [DataRow(30, true, 100, 1.75, ChronicDisease.Diabetes)]
    // 7. Fumeur + Diabète + Hypertension (jeune)
    [DataRow(40, true, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension)]
    // 8. Obésité + Diabète + Hypertension
    [DataRow(50, false, 100, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension)]
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

    [DataTestMethod]
    // Combinaisons sûres (pas de rejet)
    [DataRow(30, false, 70, 1.75, ChronicDisease.Diabetes)] // Diabète seul (jeune, poids normal)
    [DataRow(30, false, 70, 1.75, ChronicDisease.Hypertension)]          // Hypertension seule
    [DataRow(30, true, 70, 1.75, ChronicDisease.None)]  // Fumeur seul
    [DataRow(30, false, 100, 1.75, ChronicDisease.None)]  // Obésité seule
    [DataRow(80, false, 70, 1.75, ChronicDisease.None)]      // Âge seul
    [DataRow(30, false, 70, 1.75, ChronicDisease.Diabetes | ChronicDisease.Hypertension)]  // 2 maladies sans fumeur/obésité
    [DataRow(80, false, 70, 1.75, ChronicDisease.Diabetes)]      // Âge + Diabète (sans hypertension)
    [DataRow(30, true, 70, 1.75, ChronicDisease.Hypertension)]           // Fumeur + Hypertension (jeune)
    public void Evaluate_Should_AcceptApplicant_When_CombinationIsSafe(
        int age, bool isSmoker, double weightKg, double heightM, ChronicDisease diseases) {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));

        // Assert
        Assert.IsTrue(result.IsEligible, 
      $"Expected eligible but got rejection: {result.RejectionReason}");
    }
}
