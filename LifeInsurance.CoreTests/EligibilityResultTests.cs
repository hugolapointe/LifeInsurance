using LifeInsurance.Core.Models;

namespace LifeInsurance.CoreTests;

[TestClass]
public class EligibilityResultTests {
    [TestMethod]
    public void Eligible_Should_CreateValidResult_When_ValidSurcharge() {
        // Arrange & Act
        var noSurcharge = EligibilityResult.Eligible();
        var withSurcharge = EligibilityResult.Eligible(surchargeFactor: 0.25m);

        // Assert
        Assert.IsTrue(noSurcharge.IsEligible);
        Assert.AreEqual(0m, noSurcharge.SurchargeFactor);
        Assert.IsNull(noSurcharge.RejectionReason);

        Assert.IsTrue(withSurcharge.IsEligible);
        Assert.AreEqual(0.25m, withSurcharge.SurchargeFactor);
        Assert.IsNull(withSurcharge.RejectionReason);
    }

    [TestMethod]
    public void Eligible_Should_ThrowException_When_NegativeSurcharge() {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(
            () => EligibilityResult.Eligible(surchargeFactor: -0.10m)
        );
    }

    [TestMethod]
    public void NotEligible_Should_CreateValidResult_When_ReasonProvided() {
        // Arrange & Act
        var result = EligibilityResult.NotEligible("Too young");

        // Assert
        Assert.IsFalse(result.IsEligible);
        Assert.AreEqual(0m, result.SurchargeFactor);
        Assert.AreEqual("Too young", result.RejectionReason);
    }

    [TestMethod]
    public void NotEligible_Should_ThrowException_When_ReasonIsInvalid() {
        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => EligibilityResult.NotEligible(null!));
        Assert.ThrowsException<ArgumentException>(() => EligibilityResult.NotEligible(string.Empty));
        Assert.ThrowsException<ArgumentException>(() => EligibilityResult.NotEligible("   "));
    }
}
