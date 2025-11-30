namespace LifeInsurance.Core.Models;

[Flags]
public enum ChronicDisease {
    None = 0,
    Diabetes = 1,
    Hypertension = 2,
    Dyslipidemia = 4,
    HeartDisease = 8,
    Cancer = 16,
    KidneyDisease = 32
}
