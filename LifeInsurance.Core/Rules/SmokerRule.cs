using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public class SmokerRule : EligibilityRule {

    private const decimal SmokerSurchargeFactor = 0.10m;


    public override decimal CalculateSurchargeFactor(Applicant applicant) {
        if (applicant.IsSmoker) {
            return SmokerSurchargeFactor;
        }

        return base.CalculateSurchargeFactor(applicant);
    }
}
