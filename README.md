# ?? Démonstration TDD : Système d'Évaluation d'Éligibilité d'Assurance Vie

## ?? Objectifs Pédagogiques

Cette démonstration vise à enseigner le **Test-Driven Development (TDD)** à travers un cas pratique réaliste : l'évaluation de l'éligibilité pour une assurance vie basée sur des facteurs de risque médicaux.

### Compétences développées
- ? Maîtriser le cycle Red-Green-Refactor du TDD
- ? Écrire des tests unitaires et d'intégration efficaces
- ? Appliquer les principes SOLID
- ? Utiliser des patterns de conception (Strategy, Factory Method)
- ? Organiser une suite de tests maintenable

---

## ?? Contexte Métier : Évaluation des Risques en Assurance Vie

### Qu'est-ce qu'une assurance vie ?

Une assurance vie est un contrat qui verse une prestation (capital ou rente) aux bénéficiaires en cas de décès de l'assuré. Les compagnies d'assurance évaluent le **risque de mortalité** pour déterminer :
1. **L'éligibilité** : Peut-on assurer cette personne ?
2. **La surcharge** : Quel supplément de prime appliquer ?

### Facteurs de risque évalués

#### 1. **Âge** 
**Justification médicale** :
- Le risque de mortalité augmente exponentiellement avec l'âge
- Après 75 ans, les risques cardiovasculaires, cancers et maladies dégénératives augmentent significativement

**Règles implémentées** :
- ? Rejet : < 18 ans (mineur) ou ? 100 ans (risque extrême)
- ? Surcharge 15% : 76-99 ans (risque élevé)
- ? Standard : 18-75 ans

#### 2. **Tabagisme**
**Justification médicale** :
- Augmente le risque de cancer du poumon (×15-30)
- Augmente le risque cardiovasculaire (×2-4)
- Diminue l'espérance de vie de 10-15 ans

**Règles implémentées** :
- ? Surcharge 10% : Fumeur actif

#### 3. **IMC (Indice de Masse Corporelle)**
**Justification médicale** :
- **Sous-poids** (IMC < 18.5) : Malnutrition, immunodéficience, fragilité osseuse
- **Obésité** (IMC ? 30) : Diabète de type 2, hypertension, apnée du sommeil, risque cardiovasculaire

**Règles implémentées** :
- ? Surcharge 10% : IMC < 18.5 (sous-poids)
- ? Surcharge 15% : IMC ? 30 (obésité)

#### 4. **Maladies chroniques**
**Justification médicale** :

| Maladie | Impact | Décision |
|---------|--------|----------|
| **Diabète** | Complications cardiovasculaires, rénales, cécité | Surcharge 20% |
| **Hypertension** | AVC, insuffisance cardiaque, insuffisance rénale | Surcharge 15% |
| **Dyslipidémie** | Athérosclérose, infarctus du myocarde | Surcharge 10% |
| **Maladie cardiaque** | Pronostic vital engagé | ? Rejet |
| **Cancer** | Pronostic incertain, récidive | ? Rejet |
| **Insuffisance rénale** | Dialyse, transplantation | ? Rejet |

#### 5. **Combinaisons à haut risque**
**Justification médicale** :

Certaines combinaisons créent un **risque synergique** (effet multiplicatif) :

| Combinaison | Raison scientifique | Décision |
|-------------|---------------------|----------|
| **Obésité + Diabète** | Syndrome métabolique sévère, résistance à l'insuline | ? Rejet |
| **Diabète + Hypertension + Dyslipidémie** | Triade cardiométabolique, risque cardiovasculaire majeur | ? Rejet |
| **Âge > 75 + Diabète + Hypertension** | Complications multiples, insuffisance rénale probable | ? Rejet |
| **Fumeur + Hypertension + Âge > 75** | Risque d'AVC et d'infarctus très élevé | ? Rejet |
| **Obésité + Hypertension + Âge > 75** | Insuffisance cardiaque, mobilité réduite | ? Rejet |

---

## ?? Principes du Test-Driven Development (TDD)

### Le cycle Red-Green-Refactor

```
   1. RED 2. GREEN              3. REFACTOR
   ???????????  ???????????  ???????????
   ? Écrire  ?          ? Écrire  ?     ? Nettoyer?
   ? un test ?  ??????> ? le code ?  ??????> ? le code ?
   ? qui     ?          ? minimal ?       ? sans    ?
   ? échoue  ?          ? qui     ?          ? casser  ?
   ?         ?   ? passe   ?          ? tests   ?
   ???????????          ???????????   ???????????
       ?      ?
     ?     ?
       ?????????????????????????????????????????????
         Recommencer
```

### Les 3 lois du TDD (Uncle Bob)

1. **Ne pas écrire de code de production** sans avoir écrit un test qui échoue
2. **Écrire uniquement le test suffisant** pour échouer (y compris la compilation)
3. **Écrire uniquement le code de production** suffisant pour faire passer le test

### Avantages du TDD

| Avantage | Explication |
|----------|-------------|
| **??? Confiance** | Les tests garantissent que le code fonctionne |
| **?? Design** | Force à penser l'API avant l'implémentation |
| **?? Documentation** | Les tests documentent l'utilisation du code |
| **?? Refactoring** | Permet de modifier sans crainte de casser |
| **?? Moins de bugs** | Les bugs sont détectés immédiatement |

---

## ??? Architecture du Système

### Pattern Strategy (Règles d'éligibilité)

```
EligibilityService
       ?
       ??? AgeRule
   ??? SmokerRule
  ??? BmiRule
??? ChronicDiseaseRule
 ??? HighRiskCombinationRule
```

Chaque règle implémente `EligibilityRule` :
```csharp
public abstract class EligibilityRule
{
    public abstract bool IsEligible(Applicant applicant);
    public abstract decimal CalculateSurchargeFactor(Applicant applicant);
    public abstract string GetRejectionReason(Applicant applicant);
}
```

### Pattern Factory Method (Création de résultats)

```csharp
public class EligibilityResult
{
  // Constructeur privé - force l'utilisation des factories
    private EligibilityResult(...) { }
 
    // Factory methods
    public static EligibilityResult Eligible(decimal surcharge)
    public static EligibilityResult NotEligible(string reason)
}
```

**Avantage** : Impossible de créer un résultat invalide (ex: éligible avec raison de rejet)

### Domain-Driven Design

Le modèle `Applicant` contient des **propriétés calculées** :
```csharp
public class Applicant
{
    public decimal BMI => WeightKg / (HeightM * HeightM);
    public bool IsObese => BMI >= 30m;
    public bool IsElderly => Age > 75;
 public bool HasDiabetes => Diseases.HasFlag(ChronicDisease.Diabetes);
}
```

**Principe** : L'objet du domaine "sait" ses propres caractéristiques (Tell, Don't Ask)

---

## ?? Guide Étape par Étape (Approche TDD)

### Phase 1 : Fondations (Modèles de base)

#### Étape 1.1 : Créer le modèle Applicant

**?? RED - Test**
```csharp
[TestClass]
public class ApplicantTests
{
    [TestMethod]
    public void Constructor_Should_CreateValidApplicant_When_ParametersValid()
    {
        // Arrange & Act
        var applicant = new Applicant(30, false, 70m, 1.75m);
        
 // Assert
        Assert.AreEqual(30, applicant.Age);
        Assert.IsFalse(applicant.IsSmoker);
    }
}
```

**?? GREEN - Implémentation minimale**
```csharp
public class Applicant
{
    public int Age { get; init; }
    public bool IsSmoker { get; init; }
    public decimal WeightKg { get; init; }
    public decimal HeightM { get; init; }
    
    public Applicant(int age, bool isSmoker, decimal weightKg, decimal heightM)
    {
        Age = age;
        IsSmoker = isSmoker;
        WeightKg = weightKg;
   HeightM = heightM;
    }
}
```

**?? REFACTOR**
- Ajouter validation (âge < 0, poids <= 0, taille <= 0)
- Ajouter tests pour validation

#### Étape 1.2 : Ajouter le calcul du BMI

**?? RED - Test**
```csharp
[TestMethod]
public void BMI_Should_CalculateCorrectly()
{
    var applicant = new Applicant(30, false, 70m, 1.75m);
    Assert.AreEqual(22.86m, applicant.BMI, 0.01m);
}
```

**?? GREEN**
```csharp
public decimal BMI => WeightKg / (HeightM * HeightM);
```

#### Étape 1.3 : Créer EligibilityResult avec Factory Methods

**?? RED - Tests**
```csharp
[TestMethod]
public void Eligible_Should_CreateEligibleResult()
{
  var result = EligibilityResult.Eligible(0.25m);
    
    Assert.IsTrue(result.IsEligible);
    Assert.AreEqual(0.25m, result.SurchargeFactor);
    Assert.IsNull(result.RejectionReason);
}

[TestMethod]
public void NotEligible_Should_RequireReason()
{
    Assert.ThrowsException<ArgumentException>(
        () => EligibilityResult.NotEligible(null!)
    );
}
```

**?? GREEN**
```csharp
public class EligibilityResult
{
  private EligibilityResult(bool isEligible, decimal surcharge, string? reason)
    {
     IsEligible = isEligible;
        SurchargeFactor = surcharge;
     RejectionReason = reason;
    }
    
    public static EligibilityResult Eligible(decimal surcharge = 0m)
    {
        if (surcharge < 0)
      throw new ArgumentException("Surcharge cannot be negative");
        return new EligibilityResult(true, surcharge, null);
    }
    
    public static EligibilityResult NotEligible(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason required");
        return new EligibilityResult(false, 0m, reason);
    }
}
```

---

### Phase 2 : Première Règle (AgeRule)

#### Étape 2.1 : Test de rejet pour âge invalide

**?? RED**
```csharp
[TestClass]
public class AgeRuleTests
{
    private EligibilityService Service = null!;
    
    [TestInitialize]
    public void Setup()
    {
        Service = new EligibilityService();
    }
    
    [DataTestMethod]
    [DataRow(17, "Applicant must be at least 18 years old")]
    [DataRow(100, "Applicant must be under 100 years old")]
    public void Evaluate_Should_RejectApplicant_When_AgeIsInvalid(
        int age, string expectedReason)
    {
        // Arrange & Act
        var result = Service.Evaluate(new Applicant(age, false, 70m, 1.75m));
        
        // Assert
        Assert.IsFalse(result.IsEligible);
        Assert.AreEqual(expectedReason, result.RejectionReason);
    }
}
```

**?? GREEN**
```csharp
// 1. Créer EligibilityRule abstrait
public abstract class EligibilityRule
{
    public abstract bool IsEligible(Applicant applicant);
    public abstract decimal CalculateSurchargeFactor(Applicant applicant);
    public abstract string GetRejectionReason(Applicant applicant);
}

// 2. Implémenter AgeRule
public class AgeRule : EligibilityRule
{
    private const int MinimumAge = 18;
    private const int MaximumAge = 100;
    
    public override bool IsEligible(Applicant applicant)
    {
     return applicant.Age >= MinimumAge && applicant.Age < MaximumAge;
  }
    
    public override string GetRejectionReason(Applicant applicant)
    {
        if (applicant.Age < MinimumAge)
    return "Applicant must be at least 18 years old";
      if (applicant.Age >= MaximumAge)
       return "Applicant must be under 100 years old";
        return string.Empty;
    }
    
    public override decimal CalculateSurchargeFactor(Applicant applicant)
    {
        return 0m; // À implémenter plus tard
    }
}

// 3. Créer EligibilityService
public class EligibilityService
{
    private readonly List<EligibilityRule> Rules;
    
    public EligibilityService()
    {
      Rules = [new AgeRule()];
    }
    
    public EligibilityResult Evaluate(Applicant applicant)
    {
        foreach (var rule in Rules)
        {
         if (!rule.IsEligible(applicant))
            {
   var reason = rule.GetRejectionReason(applicant);
     if (!string.IsNullOrWhiteSpace(reason))
            return EligibilityResult.NotEligible(reason);
            }
      }
        
    return EligibilityResult.Eligible();
    }
}
```

#### Étape 2.2 : Test de surcharge pour âge élevé

**?? RED**
```csharp
[DataTestMethod]
[DataRow(18, 0.0)]
[DataRow(75, 0.0)]
[DataRow(76, 0.15)]
[DataRow(99, 0.15)]
public void Evaluate_Should_ApplySurcharge_When_AgeVaries(
    int age, double expectedSurcharge)
{
    var result = Service.Evaluate(new Applicant(age, false, 70m, 1.75m));
    
    Assert.IsTrue(result.IsEligible);
    Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
}
```

**?? GREEN**
```csharp
// Dans AgeRule
private const int SurchargeAge = 75;
private const decimal AgeSurchargeFactor = 0.15m;

public override decimal CalculateSurchargeFactor(Applicant applicant)
{
    if (applicant.Age > SurchargeAge)
        return AgeSurchargeFactor;
    return 0m;
}

// Dans EligibilityService.Evaluate()
var totalSurcharge = Rules.Sum(r => r.CalculateSurchargeFactor(applicant));
return EligibilityResult.Eligible(totalSurcharge);
```

**?? REFACTOR**
- Extraire les constantes
- Utiliser LINQ au lieu de foreach
```csharp
var firstRejection = Rules
    .Where(r => !r.IsEligible(applicant))
    .Select(r => r.GetRejectionReason(applicant))
    .FirstOrDefault(reason => !string.IsNullOrWhiteSpace(reason));
```

---

### Phase 3 : Règles Simples (SmokerRule, BmiRule)

#### Étape 3.1 : SmokerRule

**?? RED**
```csharp
[TestClass]
public class SmokerRuleTests
{
    [DataTestMethod]
    [DataRow(false, 0.0)]
 [DataRow(true, 0.10)]
    public void Evaluate_Should_ApplySurcharge_When_SmokerStatusVaries(
        bool isSmoker, double expectedSurcharge)
    {
        var service = new EligibilityService();
        var result = service.Evaluate(new Applicant(30, isSmoker, 70m, 1.75m));
        
        Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
```

**?? GREEN**
```csharp
public class SmokerRule : EligibilityRule
{
    private const decimal SmokerSurchargeFactor = 0.10m;
    
    public override bool IsEligible(Applicant applicant) => true;
  
    public override decimal CalculateSurchargeFactor(Applicant applicant)
  {
     return applicant.IsSmoker ? SmokerSurchargeFactor : 0m;
    }
    
    public override string GetRejectionReason(Applicant applicant) => string.Empty;
}

// Ajouter dans EligibilityService
Rules = [new AgeRule(), new SmokerRule()];
```

#### Étape 3.2 : BmiRule

**?? RED**
```csharp
[DataTestMethod]
[DataRow(50.0, 1.75, 0.10)]  // Underweight
[DataRow(70.0, 1.75, 0.0)]   // Normal
[DataRow(92.0, 1.75, 0.15)]  // Obese
public void Evaluate_Should_ApplySurcharge_When_BmiVaries(
    double weightKg, double heightM, double expectedSurcharge)
{
    var service = new EligibilityService();
    var result = service.Evaluate(
new Applicant(30, false, (decimal)weightKg, (decimal)heightM));
 
    Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
}
```

**?? GREEN**
```csharp
public class BmiRule : EligibilityRule
{
    private const decimal UnderweightThreshold = 18.5m;
    private const decimal ObesityThreshold = 30.0m;
    private const decimal UnderweightSurcharge = 0.10m;
    private const decimal ObesitySurcharge = 0.15m;
    
    public override bool IsEligible(Applicant applicant) => true;
    
    public override decimal CalculateSurchargeFactor(Applicant applicant)
    {
    var bmi = applicant.BMI;
        
        if (bmi < UnderweightThreshold)
        return UnderweightSurcharge;
   
        if (bmi >= ObesityThreshold)
         return ObesitySurcharge;
        
        return 0m;
    }
    
    public override string GetRejectionReason(Applicant applicant) => string.Empty;
}
```

---

### Phase 4 : Maladies Chroniques

#### Étape 4.1 : Créer l'enum ChronicDisease

**?? RED**
```csharp
[TestMethod]
public void Constructor_Should_AcceptChronicDisease()
{
    var applicant = new Applicant(30, false, 70m, 1.75m, ChronicDisease.Diabetes);
    Assert.AreEqual(ChronicDisease.Diabetes, applicant.Diseases);
}
```

**?? GREEN**
```csharp
[Flags]
public enum ChronicDisease
{
    None = 0,
    Diabetes = 1,
    Hypertension = 2,
    Dyslipidemia = 4,
    HeartDisease = 8,
    Cancer = 16,
    KidneyDisease = 32
}

public class Applicant
{
    public ChronicDisease Diseases { get; init; }
    
    public Applicant(..., ChronicDisease diseases = ChronicDisease.None)
    {
        // ...
    Diseases = diseases;
    }
}
```

#### Étape 4.2 : ChronicDiseaseRule - Rejets

**?? RED**
```csharp
[DataTestMethod]
[DataRow(ChronicDisease.HeartDisease)]
[DataRow(ChronicDisease.Cancer)]
[DataRow(ChronicDisease.KidneyDisease)]
public void Evaluate_Should_RejectApplicant_When_HasTerminalDisease(
    ChronicDisease disease)
{
    var result = service.Evaluate(new Applicant(30, false, 70m, 1.75m, disease));
    
  Assert.IsFalse(result.IsEligible);
    Assert.AreEqual("Applicant has a terminal or severe chronic disease",
   result.RejectionReason);
}
```

**?? GREEN**
```csharp
public class ChronicDiseaseRule : EligibilityRule
{
    private const ChronicDisease TerminalDiseases = 
  ChronicDisease.HeartDisease | 
        ChronicDisease.Cancer | 
        ChronicDisease.KidneyDisease;
    
    public override bool IsEligible(Applicant applicant)
    {
        return (applicant.Diseases & TerminalDiseases) == 0;
    }
    
    public override string GetRejectionReason(Applicant applicant)
 {
        if ((applicant.Diseases & TerminalDiseases) != 0)
            return "Applicant has a terminal or severe chronic disease";
   return string.Empty;
    }
}
```

#### Étape 4.3 : ChronicDiseaseRule - Surcharges

**?? RED**
```csharp
[DataTestMethod]
[DataRow(ChronicDisease.Diabetes, 0.20)]
[DataRow(ChronicDisease.Hypertension, 0.15)]
[DataRow(ChronicDisease.Dyslipidemia, 0.10)]
public void Evaluate_Should_ApplySurcharge_When_HasChronicDisease(
    ChronicDisease disease, double expectedSurcharge)
{
    var result = service.Evaluate(new Applicant(30, false, 70m, 1.75m, disease));
    
    Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
}
```

**?? GREEN**
```csharp
public override decimal CalculateSurchargeFactor(Applicant applicant)
{
    decimal total = 0m;
    
if (applicant.Diseases.HasFlag(ChronicDisease.Diabetes))
        total += 0.20m;
    
    if (applicant.Diseases.HasFlag(ChronicDisease.Hypertension))
     total += 0.15m;
    
    if (applicant.Diseases.HasFlag(ChronicDisease.Dyslipidemia))
        total += 0.10m;
    
    return total;
}
```

**?? REFACTOR** - Ajouter propriétés calculées
```csharp
// Dans Applicant
public bool HasDiabetes => Diseases.HasFlag(ChronicDisease.Diabetes);
public bool HasHypertension => Diseases.HasFlag(ChronicDisease.Hypertension);
public bool HasDyslipidemia => Diseases.HasFlag(ChronicDisease.Dyslipidemia);

// Dans ChronicDiseaseRule
if (applicant.HasDiabetes) total += 0.20m;
if (applicant.HasHypertension) total += 0.15m;
if (applicant.HasDyslipidemia) total += 0.10m;
```

---

### Phase 5 : Combinaisons à Haut Risque

#### Étape 5.1 : Ajouter propriétés calculées

**?? RED**
```csharp
[TestMethod]
public void IsObese_Should_ReturnTrue_When_BmiOver30()
{
    var applicant = new Applicant(30, false, 100m, 1.75m);
    Assert.IsTrue(applicant.IsObese);
}
```

**?? GREEN**
```csharp
public bool IsObese => BMI >= 30m;
public bool IsElderly => Age > 75;
```

#### Étape 5.2 : HighRiskCombinationRule

**?? RED**
```csharp
[DataTestMethod]
[DataRow(30, false, 100, 1.75, ChronicDisease.Diabetes)]  // Obésité + Diabète
public void Evaluate_Should_RejectApplicant_When_HasDangerousCombination(...)
{
    var result = service.Evaluate(
        new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));
    
    Assert.IsFalse(result.IsEligible);
    Assert.AreEqual("Combination of risk factors creates an unacceptably high risk profile",
        result.RejectionReason);
}
```

**?? GREEN**
```csharp
public class HighRiskCombinationRule : EligibilityRule
{
    public override bool IsEligible(Applicant applicant)
    {
    // Obésité + Diabète
        if (applicant.IsObese && applicant.HasDiabetes)
       return false;
      
        // Triade métabolique
        if (applicant.HasDiabetes && 
            applicant.HasHypertension && 
            applicant.HasDyslipidemia)
      return false;
        
        // Âge + Diabète + Hypertension
        if (applicant.IsElderly && 
        applicant.HasDiabetes && 
            applicant.HasHypertension)
       return false;
        
        // Fumeur + Hypertension + Âge
  if (applicant.IsSmoker && 
       applicant.HasHypertension && 
applicant.IsElderly)
   return false;
        
        // Obésité + Hypertension + Âge
   if (applicant.IsObese && 
   applicant.HasHypertension && 
          applicant.IsElderly)
        return false;
   
        return true;
    }
    
    public override string GetRejectionReason(Applicant applicant)
    {
        if (!IsEligible(applicant))
     return "Combination of risk factors creates an unacceptably high risk profile";
        return string.Empty;
    }
    
    public override decimal CalculateSurchargeFactor(Applicant applicant) => 0m;
}
```

**?? REFACTOR** - Extraire méthode
```csharp
private bool HasDangerousCombination(Applicant applicant)
{
    if (applicant.IsObese && applicant.HasDiabetes) return true;
    if (applicant.HasDiabetes && applicant.HasHypertension && applicant.HasDyslipidemia) return true;
    if (applicant.IsElderly && applicant.HasDiabetes && applicant.HasHypertension) return true;
    if (applicant.IsSmoker && applicant.HasHypertension && applicant.IsElderly) return true;
    if (applicant.IsObese && applicant.HasHypertension && applicant.IsElderly) return true;
    return false;
}
```

---

### Phase 6 : Tests d'Intégration

#### Étape 6.1 : Créer EligibilityServiceTests

**?? RED**
```csharp
[TestClass]
public class EligibilityServiceTests
{
    [DataTestMethod]
    [DataRow(30, true, 70, 1.75, ChronicDisease.None, 0.10)]  // Smoker only
    [DataRow(30, true, 100, 1.75, ChronicDisease.None, 0.25)] // Smoker + Obese
    [DataRow(80, true, 100, 1.75, ChronicDisease.None, 0.40)] // Age + Smoker + Obese
    public void Evaluate_Should_CombineSurcharges_When_MultipleRulesApply(
     int age, bool isSmoker, double weightKg, double heightM, 
        ChronicDisease diseases, double expectedSurcharge)
    {
  var service = new EligibilityService();
        var result = service.Evaluate(
          new Applicant(age, isSmoker, (decimal)weightKg, (decimal)heightM, diseases));
 
        Assert.IsTrue(result.IsEligible);
    Assert.AreEqual((decimal)expectedSurcharge, result.SurchargeFactor);
    }
}
```

**? GREEN** - Devrait déjà passer si toutes les règles sont implémentées

---

### Phase 7 : Organisation et Refactoring Final

#### Étape 7.1 : Séparer les tests par règle

Créer des fichiers de tests séparés :
- `AgeRuleTests.cs`
- `SmokerRuleTests.cs`
- `BmiRuleTests.cs`
- `ChronicDiseaseRuleTests.cs`
- `HighRiskCombinationRuleTests.cs`
- `EligibilityServiceTests.cs` (intégration uniquement)

#### Étape 7.2 : Utiliser des régions pour organiser

```csharp
#region Smoker + BMI Combinations
// Tests ici
#endregion

#region Age + Smoker + BMI Combinations
// Tests ici
#endregion
```

---

## ?? Bonnes Pratiques Démontrées

### 1. **Naming Convention des Tests**
```
{MethodName}_Should_{ExpectedBehavior}_When_{Condition}
```

Exemple :
```csharp
Evaluate_Should_RejectApplicant_When_AgeIsInvalid
Evaluate_Should_ApplySurcharge_When_BmiVaries
```

### 2. **Arrange-Act-Assert (AAA)**
```csharp
[TestMethod]
public void ExempleTest()
{
    // Arrange - Préparer les données
    var applicant = new Applicant(30, false, 70m, 1.75m);
    
  // Act - Exécuter l'action
    var result = service.Evaluate(applicant);
    
// Assert - Vérifier le résultat
    Assert.IsTrue(result.IsEligible);
  Assert.AreEqual(0m, result.SurchargeFactor);
}
```

### 3. **Tests Paramétrés (Data-Driven)**
```csharp
[DataTestMethod]
[DataRow(17, "Applicant must be at least 18 years old")]
[DataRow(100, "Applicant must be under 100 years old")]
public void Test(int age, string expectedReason)
{
    // Un seul test, multiples scénarios
}
```

**Avantages** :
- Évite la duplication de code
- Facile d'ajouter de nouveaux cas
- Tests plus maintenables

### 4. **Factory Methods pour Invariants**
```csharp
// ? MAUVAIS - Permet états invalides
new EligibilityResult(true, 0m, "Raison de rejet") // Incohérent!

// ? BON - Garantit cohérence
EligibilityResult.Eligible(0.25m)    // Toujours cohérent
EligibilityResult.NotEligible("...") // Raison obligatoire
```

### 5. **Tell, Don't Ask**
```csharp
// ? MAUVAIS - "Demander"
if (applicant.Diseases.HasFlag(ChronicDisease.Diabetes))

// ? BON - "Dire"
if (applicant.HasDiabetes)
```

### 6. **Single Responsibility**
Chaque règle a une seule responsabilité :
- `AgeRule` : Évaluer l'âge
- `SmokerRule` : Évaluer le tabagisme
- `HighRiskCombinationRule` : Évaluer les combinaisons

### 7. **Open/Closed Principle**
Le système est :
- **Ouvert à l'extension** : Facile d'ajouter une nouvelle règle
- **Fermé à la modification** : Pas besoin de modifier `EligibilityService`

### 8. **Dependency Inversion**
`EligibilityService` dépend de l'abstraction `EligibilityRule`, pas des implémentations concrètes.

---

## ?? Métriques Finales du Projet

| Métrique | Valeur |
|----------|--------|
| **Classes de production** | 9 |
| **Règles d'éligibilité** | 5 |
| **Fichiers de tests** | 8 |
| **Tests totaux** | 63 |
| **Tests unitaires** | 44 (70%) |
| **Tests d'intégration** | 19 (30%) |
| **Couverture de code** | 100% |

---

## ?? Exercices Supplémentaires

### Exercice 1 : Ajouter une nouvelle règle
**Objectif** : Pratiquer le cycle TDD

Ajouter une règle `OccupationRule` qui :
- Rejette les professions à très haut risque (ex: démineur, acrobate)
- Applique 20% de surcharge pour professions à risque (ex: pompier, policier)

**Étapes** :
1. Écrire les tests (RED)
2. Implémenter la règle (GREEN)
3. Refactoriser (REFACTOR)
4. Ajouter au service

### Exercice 2 : Ajouter une nouvelle combinaison dangereuse
**Objectif** : Étendre HighRiskCombinationRule

Ajouter le rejet pour :
- Fumeur + Obésité + Diabète

### Exercice 3 : Modifier une surcharge
**Objectif** : Pratiquer le refactoring avec tests

Modifier la surcharge du tabagisme de 10% à 12%

**Observation** : Les tests doivent vous guider et détecter tous les endroits à modifier !

---

## ?? Ressources Complémentaires

### Livres recommandés
- **"Test Driven Development: By Example"** - Kent Beck
- **"Clean Code"** - Robert C. Martin
- **"Refactoring"** - Martin Fowler

### Concepts avancés à explorer
- **Mocking et Stubs** : Pour isoler les dépendances
- **Test Fixtures** : Données de test réutilisables
- **Mutation Testing** : Tester la qualité des tests eux-mêmes
- **Property-Based Testing** : Tests avec génération automatique de données

---

## ? Checklist de Validation

Après avoir terminé la démonstration, vérifiez que vous maîtrisez :

- [ ] Le cycle Red-Green-Refactor
- [ ] L'écriture de tests avant le code
- [ ] L'utilisation de tests paramétrés
- [ ] Le pattern Strategy pour les règles
- [ ] Le pattern Factory Method pour les résultats
- [ ] L'organisation de tests par fichier
- [ ] L'utilisation de régions pour structurer
- [ ] Les propriétés calculées (Tell, Don't Ask)
- [ ] La validation fail-fast dans les constructeurs
- [ ] Les principes SOLID (SRP, OCP, DIP)

---

## ?? Conclusion

Cette démonstration illustre comment le **TDD** permet de construire un système robuste, maintenable et bien testé. Les tests ne sont pas une charge supplémentaire, mais un **outil de design** qui guide vers un meilleur code.

**Points clés à retenir** :
1. ? **Les tests d'abord** : Ils définissent le comportement attendu
2. ? **Baby steps** : Avancer par petites étapes vérifiables
3. ? **Refactoring continu** : Améliorer le code sans crainte
4. ? **Tests = Documentation** : Ils expliquent comment utiliser le code
5. ? **Design émergent** : L'architecture émerge des tests

Le TDD demande de la discipline au début, mais devient rapidement naturel et procure une **confiance inébranlable** dans votre code.

---

**Bon apprentissage ! ??**

*Pour toute question ou suggestion d'amélioration de cette démonstration, n'hésitez pas à contribuer.*
