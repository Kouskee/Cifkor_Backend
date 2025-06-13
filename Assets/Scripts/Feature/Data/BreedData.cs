using System;

[Serializable]
public class BreedData
{
    public string id;
    public string type;

    public BreedAttributes attributes;
    public BreedRelationships relationships;
}

[Serializable]
public class BreedsListResponse
{
    public BreedData[] data;
    public Links links;
}

[Serializable]
public class BreedSingleResponse
{
    public BreedData data;
    public Links links;
}

[Serializable]
public class BreedAttributes
{
    public string name;
    public string description;
    public LifeSpan life;
    public WeightRange male_weight;
    public WeightRange female_weight;
    public bool hypoallergenic;
}

[Serializable]
public class LifeSpan
{
    public int max;
    public int min;
}

[Serializable]
public class WeightRange
{
    public int max;
    public int min;
}

[Serializable]
public class BreedRelationships
{
    public GroupReference group;
}

[Serializable]
public class GroupReference
{
    public GroupData data;
}

[Serializable]
public class GroupData
{
    public string id;
    public string type;
}