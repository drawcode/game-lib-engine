using System;

public class DataAttribute {
    public string uid;
    public string code;
    public string type;
    public string otype;
    public string name;
    public object val;

    public DataAttribute() {
        Reset();
    }

    public void Reset() {
        uid = "";
        code = "";
        type = "";
        name = "";
        otype = "";
        val = null;
    }
}