using LifeInsurance.Core.Models;

namespace LifeInsurance.Core.Rules;

public class ChronicDiseaseRule : EligibilityRule {
    private const ChronicDisease TerminalDiseases =
        ChronicDisease.HeartDisease |
        ChronicDisease.Cancer |
        ChronicDisease.KidneyDisease;

    private const decimal DiabetesSurchargeFactor = 0.20m;
    private const decimal HypertensionSurchargeFactor = 0.15m;
    private const decimal DyslipidemiaSurchargeFactor = 0.10m;

    private const string TerminalDiseaseReason = "Applicant has a terminal or severe chronic disease";

    public override bool IsEligible(Applicant applicant) {
        return (applicant.Diseases & TerminalDiseases) == 0;
    }

    public override decimal CalculateSurchargeFactor(Applicant applicant) {
        decimal totalSurcharge = base.CalculateSurchargeFactor(applicant);

        if (applicant.Diseases.HasFlag(ChronicDisease.Diabetes)) {
            totalSurcharge += DiabetesSurchargeFactor;
        }

        if (applicant.Diseases.HasFlag(ChronicDisease.Hypertension)) {
            totalSurcharge += HypertensionSurchargeFactor;
        }

        if (applicant.Diseases.HasFlag(ChronicDisease.Dyslipidemia)) {
            totalSurcharge += DyslipidemiaSurchargeFactor;
        }

        return totalSurcharge;
    }

    public override string GetRejectionReason(Applicant applicant) {
        if ((applicant.Diseases & TerminalDiseases) != 0) {
            return TerminalDiseaseReason;
        }

        return base.GetRejectionReason(applicant);
    }
}
