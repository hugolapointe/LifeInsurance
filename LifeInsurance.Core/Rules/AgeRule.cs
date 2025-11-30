using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public class AgeRule : EligibilityRule {
    // Règles métier pour l'âge
    private const int MinimumAge = 18;
    private const int MaximumAge = 100;

    private const decimal ElderlySurchargeFactor = 0.15m;

    // Raisons de rejet
    private const string BelowMinimumAgeReason = "Applicant must be at least 18 years old";
    private const string AboveMaximumAgeReason = "Applicant must be under 100 years old";

    public override bool IsEligible(Applicant applicant) {
        return applicant.Age >= MinimumAge
            && applicant.Age < MaximumAge;
    }

    public override decimal CalculateSurchargeFactor(Applicant applicant) {
        if (applicant.IsElderly) {
            return ElderlySurchargeFactor;
        }

        return base.CalculateSurchargeFactor(applicant);
    }

    public override string GetRejectionReason(Applicant applicant) {
        if (applicant.Age < MinimumAge) {
            return BelowMinimumAgeReason;
        }

        if (applicant.Age >= MaximumAge) {
            return AboveMaximumAgeReason;
        }

        return base.GetRejectionReason(applicant);
    }
}
