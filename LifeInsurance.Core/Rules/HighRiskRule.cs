using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;


public class HighRiskRule : EligibilityRule {
    private const string HighRiskCombinationReason =
      "Combination of risk factors creates an unacceptably high risk profile";

    public override bool IsEligible(Applicant applicant) {
        return !HasHighRiskCombination(applicant);
    }

    public override string GetRejectionReason(Applicant applicant) {
        if (HasHighRiskCombination(applicant)) {
            return HighRiskCombinationReason;
        }

        return string.Empty;
    }

    private static bool HasHighRiskCombination(Applicant applicant) {
        // Obesity-based combinations
        if (applicant.IsObese && applicant.HasDiabetes)
            return true;
        
        if (applicant.IsObese && applicant.HasHypertension && applicant.IsElderly)
            return true;
        
        // Diabetes + Hypertension combinations
        if (applicant.HasDiabetes && applicant.HasHypertension) {
            if (applicant.HasDyslipidemia || applicant.IsElderly || applicant.IsSmoker)
                return true;
        }
        
        // Smoking + multiple factors
        if (applicant.IsSmoker && applicant.HasHypertension && applicant.IsElderly)
            return true;

        return false;
    }
}
