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
  
  public delegate void StatsChangedEventHandler();

  public event StatsChangedEventHandler OnChange;
  
  public int Health {
    get { return health; }
    set {
      if (health != value) {
        health = value;
        OnChange?.Invoke();
      }
    }
  }

  public int Block {
    get { return block; }
    set {
      if (block != value) {
        block = value;
        OnChange?.Invoke();
      }
    }
  }
  
  public int Insight {
    get { return insight; }
    set {
      if (insight != value) {
        insight = value;
        OnChange?.Invoke();
      }
    }
  }
  
  public int Burn {
    get { return burn; }
    set {
      if (burn != value) {
        burn = value;
        OnChange?.Invoke();
      }
    }
  }
  
  public int AttackDebuff {
    get { return attackDebuff; }
    set {
      if (attackDebuff != value) {
        attackDebuff = value;
        OnChange?.Invoke();
      }
    }
  }
}
