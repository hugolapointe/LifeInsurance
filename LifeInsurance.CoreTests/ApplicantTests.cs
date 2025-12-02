using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class ApplicantTests {
    [TestMethod]
    public void Constructor_Should_CreateValidApplicant_When_ParametersValid() {
        // Arrange & Act
        var applicant = new Applicant(30, isSmoker: true, weightKg: 70m, heightM: 1.75m);

        // Assert
        Assert.AreEqual(30, applicant.Age);
        Assert.IsTrue(applicant.IsSmoker);
        Assert.AreEqual(70m, applicant.WeightKg);
        Assert.AreEqual(1.75m, applicant.HeightM);
    }

    [DataTestMethod]
    [DataRow(-1, false, 70.0, 1.75)]   // Invalid age
    [DataRow(30, false, -70.0, 1.75)]  // Invalid weight
    [DataRow(30, false, 70.0, -1.75)]  // Invalid height
    public void Constructor_Should_ThrowException_When_ParametersInvalid(
        int age, bool isSmoker, double weightKg, double heightM) {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM)
        );
    }

    [DataTestMethod]
    [DataRow(70.0, 1.75, 22.86, false, false)]   // Normal
    [DataRow(50.0, 1.75, 16.33, true, false)]    // Underweight
    [DataRow(100.0, 1.75, 32.65, false, true)]   // Obese
    public void BMI_And_WeightCategories_Should_BeCorrect(
        double weightKg, double heightM, double expectedBmi,
        bool expectedUnderweight, bool expectedObese) {
        // Arrange & Act
        var applicant = new Applicant(30, false, (decimal)weightKg, (decimal)heightM);

        // Assert
        Assert.AreEqual((decimal)expectedBmi, applicant.BMI, 0.01m);
        Assert.AreEqual(expectedUnderweight, applicant.IsUnderweight);
        Assert.AreEqual(expectedObese, applicant.IsObese);
    }

    [DataTestMethod]
    [DataRow(50, false)]
    [DataRow(75, false)]
    [DataRow(76, true)]
    public void IsElderly_Should_BeCorrect(int age, bool expectedElderly) {
        // Arrange & Act
        var applicant = new Applicant(age, false, 70m, 1.75m);

        // Assert
        Assert.AreEqual(expectedElderly, applicant.IsElderly);
    }

    [DataTestMethod]
    [DataRow(ChronicDisease.None, false, false, false)]
    [DataRow(ChronicDisease.Diabetes, true, false, false)]
    [DataRow(ChronicDisease.Hypertension, false, true, false)]
    [DataRow(ChronicDisease.Dyslipidemia, false, false, true)]
    [DataRow(ChronicDisease.Diabetes | ChronicDisease.Hypertension, true, true, false)]
    [DataRow(ChronicDisease.HeartDisease, false, false, false)]  // Terminal disease
    public void ChronicDiseaseProperties_Should_BeCorrect(
      ChronicDisease diseases, bool expectedDiabetes, bool expectedHypertension, bool expectedDyslipidemia) {
        // Arrange & Act
        var applicant = new Applicant(30, false, 70m, 1.75m, diseases);

        // Assert
        Assert.AreEqual(expectedDiabetes, applicant.HasDiabetes);
        Assert.AreEqual(expectedHypertension, applicant.HasHypertension);
        Assert.AreEqual(expectedDyslipidemia, applicant.HasDyslipidemia);
    }
}
