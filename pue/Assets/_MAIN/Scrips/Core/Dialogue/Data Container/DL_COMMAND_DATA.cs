using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DL_COMMAND_DATA
{
    private List<Command> commands;
    private char COMMANDSPLITTER_ID = ',';
    private char ARGUMENTCONTAINER_ID = '(';
    private string WAITCOMMAND_ID = "[wait]";
    public List<Command> GetCommands() {  return commands; }
    public class Command
    {
        string name;
        string[] arguments;
        bool waitForComplete;

        public void SetName(string name) { this.name=name; }
        public string GetName() {  return name; }
        public void SetArguments(string[] arguments) { this.arguments=arguments;}
        public string[] GetArguments() { return arguments;}
        public bool GetWait() { return waitForComplete; }
        public void SetWait(bool wait) { waitForComplete = wait; }
    }

    public DL_COMMAND_DATA(string rawCommands)
    { 
        commands =RipCommands(rawCommands); 
    }
    private List<Command> RipCommands(string rawCommands)
    {
        string[] data =rawCommands.Split(COMMANDSPLITTER_ID,System.StringSplitOptions.RemoveEmptyEntries);
        List<Command> result = new List<Command>();
        foreach (string cmd in data) 
        {
            Command command=new Command();
            int index =cmd.IndexOf(ARGUMENTCONTAINER_ID);
            command.SetName(cmd.Substring(0, index).Trim());
            if(command.GetName().ToLower().StartsWith(WAITCOMMAND_ID))
            { 
                command.SetName(command.GetName().Substring(WAITCOMMAND_ID.Length));
                command.SetWait(true);
            }
            else
            {
                command.SetWait(false);
            }
            command.SetArguments(GetArgs(cmd.Substring(index+1, cmd.Length-index-2)));
            result.Add(command);
        }
        return result;
    }
    private string[] GetArgs(string args)
    {
        List<string> argList = new List<string>();
        StringBuilder currentArgs = new StringBuilder();
        bool inQuotes =false;

        for(int i=0;i <args.Length;++i) 
        {
            if (args[i]== '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if(!inQuotes && args[i] ==' ')
            {
                argList.Add(currentArgs.ToString());
                currentArgs.Clear();
                continue;
            }
            currentArgs.Append(args[i]);
        }
        if(currentArgs.Length > 0) 
        {
            argList.Add(currentArgs.ToString());
        }
        return argList.ToArray();
    }
}
