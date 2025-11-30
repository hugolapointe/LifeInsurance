namespace LifeInsurance.Core.Models;

public class EligibilityResult {
    public bool IsEligible { get; }
    public decimal SurchargeFactor { get; }
    public string? RejectionReason { get; }

    private EligibilityResult(bool isEligible, decimal surchargeFactor, string? rejectionReason) {
        IsEligible = isEligible;
        SurchargeFactor = surchargeFactor;
        RejectionReason = rejectionReason;
    }

    public static EligibilityResult Eligible(decimal surchargeFactor = 0m) {
        if (surchargeFactor < 0) {
            throw new ArgumentException("Surcharge factor cannot be negative", nameof(surchargeFactor));
        }

        return new EligibilityResult(
            isEligible: true,
            surchargeFactor: surchargeFactor,
            rejectionReason: null
        );
    }

    public static EligibilityResult NotEligible(string rejectionReason) {
        if (string.IsNullOrWhiteSpace(rejectionReason)) {
            throw new ArgumentException("Rejection reason is required for non-eligible applicants", nameof(rejectionReason));
        }

        return new EligibilityResult(
            isEligible: false,
            surchargeFactor: 0m,
            rejectionReason: rejectionReason
        );
    }
}
