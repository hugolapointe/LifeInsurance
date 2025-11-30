using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public class HighRiskRule : EligibilityRule {
    private const string HighRiskCombinationReason =
    "Combination of risk factors creates an unacceptably high risk profile";

    public override bool IsEligible(Applicant applicant) {
        return !HasDangerousCombination(applicant);
    }

    public override string GetRejectionReason(Applicant applicant) {
        if (HasDangerousCombination(applicant)) {
            return HighRiskCombinationReason;
        }

        return string.Empty;
    }

    private static bool HasDangerousCombination(Applicant applicant) {
        if (applicant.IsObese && applicant.HasDiabetes)
            return true;

        if (applicant.HasDiabetes && applicant.HasHypertension && applicant.HasDyslipidemia)
            return true;

        if (applicant.IsElderly && applicant.HasDiabetes && applicant.HasHypertension)
            return true;

        if (applicant.IsSmoker && applicant.HasHypertension && applicant.IsElderly)
            return true;

        if (applicant.IsObese && applicant.HasHypertension && applicant.IsElderly)
            return true;

        return false;
    }
}
