using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class User
{
    public string Name { get; set; }
    
    public User()
    {
    }

    public User(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }


}
