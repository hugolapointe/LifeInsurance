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
    [DataRow(30, false, 0.0, 1.75)]    // Invalid weight
    [DataRow(30, false, 70.0, -1.75)]  // Invalid height
    [DataRow(30, false, 70.0, 0.0)]    // Invalid height
    public void Constructor_Should_ThrowException_When_ParametersInvalid(
        int age, bool isSmoker, double weightKg, double heightM) {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM)
        );
    }

    [DataTestMethod]
    [DataRow(70.0, 1.75, 22.86)]   // Normal
    [DataRow(50.0, 1.75, 16.33)]   // Underweight
    [DataRow(100.0, 1.75, 32.65)]  // Obese
    public void BMI_Should_CalculateCorrectly(double weightKg, double heightM, double expectedBmi) {
        // Arrange & Act
        var applicant = new Applicant(30, false, (decimal)weightKg, (decimal)heightM);

        // Assert
        Assert.AreEqual((decimal)expectedBmi, applicant.BMI, 0.01m);
    }
}
