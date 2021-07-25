﻿/*
 * Copyright (c) EoflaOE and its companies. All rights reserved.
 * 
 * Name: BeepSynth.cs
 * Description: Entry point for the BeepSynth mod
 * KS Version: 0.0.16
 * 
 * History:
 * 
 * | Author   | Date      | Description
 * +----------+-----------+--------------
 * | EoflaOE  | 7/3/2021  | Initial release
 */

using Extensification.StringExts;
using KS;
using System;
using System.Collections.Generic;
using System.IO;
using KSIO = KS.Filesystem;
using Debug = KS.DebugWriters;
using TWC = KS.TextWriterColor;

namespace BeepSynth
{
    public class BeepSynth : ModParser.IScript
    {
        public Dictionary<string, CommandInfo> Commands { get; set; }
        public string Name { get; set; }
        public string ModPart { get; set; }
        public string Version { get; set; }

        public void InitEvents(string ev)
        {
        }

        public void InitEvents(string ev, params object[] Args)
        {
        }

        public void PerformCmd(CommandInfo Command, string Args = "")
        {
            //Variables
            bool RequiredArgumentsProvided = true;
            string[] eqargs = Args.SplitEncloseDoubleQuotes(" ");
            if (eqargs != null)
            {
                RequiredArgumentsProvided = eqargs?.Length >= Command.MinimumArguments;
            }
            else if (Command.ArgumentsRequired && eqargs == null)
            {
                RequiredArgumentsProvided = false;
            }

            if (Command.Command == "bsynth")
            {
                if (RequiredArgumentsProvided)
                {
                    Debug.Wdbg('I', "Success: " + TryParseSynth(Args));
                }
                else
                {
                    TWC.W("Provide a synth file.", true, ColorTools.ColTypes.Neutral);
                }
            }
        }

        public void StartMod()
        {
            Name = "BeepSynth";
            ModPart = "Main";
            Version = "0.0.15.8";
            Commands = new Dictionary<string, CommandInfo> { { "bsynth", new CommandInfo("bsynth", CommandType.ShellCommandType.Shell, "Loads the synth file and plays it.", true, 1) } };
        }

        public void StopMod()
        {
        }

        /// <summary>
        /// Tries to parse the synth file and play it back using the console PC speaker
        /// </summary>
        /// <param name="file">A file name in current shell path</param>
        /// <returns>True if successful, False if unsuccessful.</returns>
        public static bool TryParseSynth(string file)
        {
            try
            {
                file = KSIO.NeutralizePath(file);
                Debug.Wdbg('I', "Probing {0}...", file);
                if (File.Exists(file))
                {
                    // Open the stream
                    StreamReader FStream = new StreamReader(file);
                    Debug.Wdbg('I', "Opened StreamReader(file) with the length of {0}", FStream.BaseStream.Length);

                    // Read a line and parse it
                    string FStreamLine = FStream.ReadLine();
                    if (FStreamLine == "KS-BSynth")
                    {
                        //Comments are ignored in the file. Comment format: - <message>
                        Debug.Wdbg('I', "File is scripted");
                        while (!FStream.EndOfStream)
                        {
                            FStreamLine = FStream.ReadLine();
                            Debug.Wdbg('I', "Line: {0}", FStreamLine);
                            if (!FStreamLine.StartsWith("-") && !(string.IsNullOrEmpty(FStreamLine)))
                            {
                                try
                                {
                                    Debug.Wdbg('I', "Not a comment. Getting frequency and time...");
                                    int freq = Convert.ToInt32(FStreamLine.Remove(FStreamLine.IndexOf(",")));
                                    int ms = Convert.ToInt32(FStreamLine.Substring(FStreamLine.IndexOf(",") + 1));
                                    Debug.Wdbg('I', "Got frequency {0} Hz and time {1} ms", freq, ms);
                                    Console.Beep(freq, ms);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Wdbg('E', "Not a comment and not a synth line. ({0}) {1}", FStreamLine, ex.Message);
                                    TWC.W("Failed to probe a synth line: {0}", true, ColorTools.ColTypes.Error, ex.Message);
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        Debug.Wdbg('E', "File is not scripted");
                        TWC.W("The file isn't a scripted synth file.", true, ColorTools.ColTypes.Error);
                    }
                }
                else
                {
                    Debug.Wdbg('E', "File doesn't exist");
                    TWC.W("Scripted file {0} does not exist.".FormatString(file), true, ColorTools.ColTypes.Error);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}