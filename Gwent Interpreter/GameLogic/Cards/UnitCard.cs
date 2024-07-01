using System.Collections;
using System.Collections.Generic;

public class UnitCard : Card, IEffect
{
    double initialDamage = 0;
    double damageOnField = 0;
    double damageOnCount = 0;
    Effect effect = Effects.VoidEffect;

    public Level Level { get; private set; }
    public double InitialDamage { get => initialDamage; }
    public double DamageOnField { get => damageOnField; }
    public double Damage { get => damageOnCount; set => damageOnCount = value; }

    public UnitCard(string name, Faction faction, CardType cardType, List<Zone> availableRange, VisualInfo info, List<Card> currentPosition, double initialDamage, Effect effect) : base(name, faction, cardType, availableRange, info, currentPosition)
    {
        this.initialDamage = this.damageOnField = this.damageOnCount = initialDamage;
        AssignEffect(effect);
    }

    public void AssignEffect(Effect effect)
    {
        if (effect.Equals(null)) return;

        this.effect = effect;
    }

    public bool Effect(Context context)
    {
        try
        {
            return effect.Invoke(context);
        }
        catch (System.NullReferenceException)
        {
            return Effects.VoidEffect(context);
        }
    }

    public void ResetDamage() //when a weathercard affects the damage of this card, the value will only be changed until it
                              //has been summed to the total damage of the player at the moment it's being calculated, 
                              //then it will return to the initial value
    {
        this.Damage = this.damageOnField;
    }

    public void InitializeDamage()
    {
        this.damageOnField = this.initialDamage;
        ResetDamage();
    }

    public void ModifyOnFieldDamage(double newDamage, bool modifyCurrentDamageAsWell = true)
    {
        // damageOnField - damage = modification suffered on board 
        if (modifyCurrentDamageAsWell) this.Damage = (newDamage - (this.damageOnField - this.Damage) > 0 ? newDamage - (this.damageOnField - this.Damage) : 0);

        this.damageOnField = newDamage;
    }
}