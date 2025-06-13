using System;

[Serializable]
public class BreedModel
{
    public string id;
    public string name;
    public string description;

    public BreedModel(string id, string name, string description = "")
    {
        this.id = id;
        this.name = name;
        this.description = description;
    }
}