using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public class BmiRule : EligibilityRule {

    private const decimal UnderweightSurchargeFactor = 0.10m;
    private const decimal ObesitySurchargeFactor = 0.15m;


    public override decimal CalculateSurchargeFactor(Applicant applicant) {

        if (applicant.IsUnderweight) {
            return UnderweightSurchargeFactor;
        }

        if (applicant.IsObese) {
            return ObesitySurchargeFactor;
        }

        return base.CalculateSurchargeFactor(applicant);
    }
}
