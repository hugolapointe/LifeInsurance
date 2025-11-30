using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public abstract class EligibilityRule {

    public virtual bool IsEligible(Applicant applicant) => true;


    public virtual decimal CalculateSurchargeFactor(Applicant applicant) => 0m;


    public virtual string GetRejectionReason(Applicant applicant) => string.Empty;
}
