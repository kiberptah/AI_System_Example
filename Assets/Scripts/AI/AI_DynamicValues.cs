using System.Collections.Generic;


[System.Serializable]
public class AI_DynamicValues 
{
    public Dictionary<string, float> floatValues = new Dictionary<string, float>();
    public Dictionary<string, int> intValues = new Dictionary<string, int>();
    public Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
    public Dictionary<string, string> stringValues = new Dictionary<string, string>();

    public void ClearAll()
    {
        floatValues.Clear();
        intValues.Clear();
        boolValues.Clear();
        stringValues.Clear();
    }
}
