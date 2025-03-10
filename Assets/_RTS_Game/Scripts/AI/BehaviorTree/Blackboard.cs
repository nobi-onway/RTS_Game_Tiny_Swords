using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    public static readonly string CLASS_TARGET = "Target";

    public Blackboard()
    {
        data = new Dictionary<string, object>();
    }

    public void SetValue(string key, object value)
    {
        data[key] = value;
    }

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }

        return default(T);
    }
}