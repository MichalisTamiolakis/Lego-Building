using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Recording
{
    public enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        VeryHard = 3
    }

    public Recording(string name)
    {
        this.name = name;
    }

    public string name = "";
    public uint pieces = 0;
    public uint time = 0;
    public Difficulty difficulty = Difficulty.Easy;
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
        if(c.type == Command.CommandType.MoveBrick)
        {
            if(c.frames.Count > 0)
                time += Convert.ToUInt32(Mathf.RoundToInt(c.frames[c.frames.Count - 1].timestamp));
        }

        commands.Add(c);
    }

    public void RemoveCommand(int commandIndex)
    {
        Debug.Assert(commandIndex < commands.Count);

        Command c = commands[commandIndex];
        if (c.type == Command.CommandType.RequireBrick)
        {
            pieces--;
        }
        if (c.type == Command.CommandType.MoveBrick)
        {
            if (c.frames.Count > 0)
                time -= Convert.ToUInt32(Mathf.RoundToInt(c.frames[c.frames.Count - 1].timestamp));
        }

        commands.RemoveAt(commandIndex);
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