using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public struct Stats
{
  private int health;
  private int block;
  private int insight;
  
  private int burn;
  private int attackDebuff;
  private int ignoreBlockNextAttacks;
  
  public delegate void StatsChangedEventHandler();

  public event StatsChangedEventHandler OnStatChange;
  
  public int Health {
    get { return health; }
    set {
      if (health != value) {
        health = value;
        OnStatChange?.Invoke();
      }
    }
  }

  public int Block {
    get { return block; }
    set {
      if (block != value) {
        block = value;
        OnStatChange?.Invoke();
      }
    }
  }
  
  public int Insight {
    get { return insight; }
    set {
      if (insight != value) {
        insight = value;
        OnStatChange?.Invoke();
      }
    }
  }
  
  public int Burn {
    get { return burn; }
    set {
      if (burn != value) {
        burn = value;
        OnStatChange?.Invoke();
      }
    }
  }
  
  public int AttackDebuff {
    get { return attackDebuff; }
    set {
      if (attackDebuff != value) {
        attackDebuff = value;
        OnStatChange?.Invoke();
      }
    }
  }
  
  public int IgnoreBlockOnNext {
    get { return ignoreBlockNextAttacks; }
    set {
      if (ignoreBlockNextAttacks != value) {
        ignoreBlockNextAttacks = value;
        OnStatChange?.Invoke();
      }
    }
  }
  
}
