using System.Collections.Generic;

[System.Serializable]
public class LoadingPhrases
{
  [System.Serializable]
  public class Categories
  {
    public List<string> scientific;
    public List<string> curious;
    public List<string> motivational;
  }

  public Categories categories;
}
