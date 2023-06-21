using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Recording
{
    public Recording(string name)
    {
        this.name = name;
    }

    public string name = "";
    public uint pieces = 0;
    public uint time = 0;
    public List<Command> commands = new List<Command>();

    private int lastBrickID = 0;

    /// <summary>
    /// Returns an available guaranteed unique brick ID
    /// </summary>
    /// <returns>The brick ID</returns>
    public int GenerateBrickID()
    {
        return lastBrickID++;
    }

    public void AddCommand(Command c)
    {
        if(c.type == Command.CommandType.RequireBrick)
        {
            pieces++;
        }
        commands.Add(c);
    }


    public static string ToJson(Recording r)
    {
        return JsonUtility.ToJson(r);
    }

    public static Recording FromJson(string jsonString)
    {
        return JsonUtility.FromJson<Recording>(jsonString);
    }


}