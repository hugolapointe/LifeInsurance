using LifeInsurance.Core.Models;
using LifeInsurance.Core.Rules;

namespace LifeInsurance.Core;

public class EligibilityService {
    private readonly List<EligibilityRule> Rules = [
            new AgeRule(),
            new ChronicDiseaseRule(),
            new SmokerRule(),
            new BmiRule(),
            new HighRiskRule()
        ];

    public EligibilityResult Evaluate(Applicant applicant) {
        var firstRejection = Rules
            .Where(rule => !rule.IsEligible(applicant))
            .Select(rule => rule.GetRejectionReason(applicant))
            .FirstOrDefault(reason => !string.IsNullOrWhiteSpace(reason));

        if (firstRejection is not null) {
            return EligibilityResult.NotEligible(firstRejection);
        }

        var totalSurcharge = Rules.Sum(rule => rule.CalculateSurchargeFactor(applicant));

        return EligibilityResult.Eligible(totalSurcharge);
    }
}
