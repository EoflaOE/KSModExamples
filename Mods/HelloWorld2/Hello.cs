/*
 * Copyright (c) EoflaOE and its companies. All rights reserved.
 * 
 * Name: Hello.cs
 * Description: Command entry point for "hello"
 * KS Version: 0.0.22
 * 
 * History:
 * 
 * | Author   | Date      | Description
 * +----------+-----------+--------------
 * | EoflaOE  | 6/13/2022 | Initial release
 */

using KS.ConsoleBase;
using KS.Misc.Writers.ConsoleWriters;
using KS.Shell.ShellBase;

namespace HelloWorld2
{
    internal class Hello : CommandExecutor, ICommand
    {
        public override void Execute(string StringArgs, string[] ListArgs, string[] ListArgsOnly, string[] ListSwitchesOnly)
        {
            TextWriterColor.Write("Hello World! From the \"hello\" command!", true, ColorTools.ColTypes.Neutral);
        }
    }
}
