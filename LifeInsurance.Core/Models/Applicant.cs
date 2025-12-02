namespace LifeInsurance.Core.Models;

public class Applicant {
    public int Age { get; init; }
    public bool IsSmoker { get; init; }
    public decimal WeightKg { get; init; }
    public decimal HeightM { get; init; }
    public ChronicDisease Diseases { get; init; }

    public decimal BMI => WeightKg / (HeightM * HeightM);
    public bool IsUnderweight => BMI < 18.5m;
    public bool IsObese => BMI >= 30m;
    public bool IsElderly => Age > 75;
    public bool HasDiabetes => Diseases.HasFlag(ChronicDisease.Diabetes);
    public bool HasHypertension => Diseases.HasFlag(ChronicDisease.Hypertension);
    public bool HasDyslipidemia => Diseases.HasFlag(ChronicDisease.Dyslipidemia);

    public Applicant(
        int age, bool isSmoker, decimal weightKg, decimal heightM, 
        ChronicDisease diseases = ChronicDisease.None) {

        if (age < 0) {
            throw new ArgumentException("Age cannot be negative", nameof(age));
        }

        if (weightKg <= 0) {
            throw new ArgumentException("Weight must be positive", nameof(weightKg));
        }

        if (heightM <= 0) {
            throw new ArgumentException("Height must be positive", nameof(heightM));
        }

        Age = age;
        IsSmoker = isSmoker;
        WeightKg = weightKg;
        HeightM = heightM;
        Diseases = diseases;
    }
}
