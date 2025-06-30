using System.Collections.Generic;

[System.Serializable]
public class TipPhrases
{
  [System.Serializable]
  public class Tip
  {
    public string type;
    public string text;
  }

  public List<Tip> tips;
}
