using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TPScript : MonoBehaviour 
{
	public QuaverScript Quaver;

    private bool IsSubmitValid(string[] split)
    {
        ushort j, max = Quaver.init.select.difficulty == 3 ? Quaver.init.select.perColumn ? (ushort)80 : (ushort)320 : (ushort)200;
        for (int i = 1; i < split.Length; i++)
            if (!ushort.TryParse(split[i], out j) || j >= max)
                return false;
        return true;
    }

    private int[] CommandToIndex(string[] split)
    {
        const int length = 3;

        string[][] parameters = new string[length][]
        {
            new[] { "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "1.7", "1.8", "1.9", "2.0" },
            new[] { "normal", "hard", "insane", "expert" },
            new[] { "off", "on" }
        };

        int[] indexes = new int[length];

        for (int i = 0; i < length; i++)
            indexes[i] = Array.IndexOf(parameters[i], split[i + 1].ToLowerInvariant());

        return indexes;
    }

    private IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.Split();

        if (Regex.IsMatch(split[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            if (!Quaver.init.ready)
                yield return "sendtochaterror The module isn't in submission mode!";

            else if ((split.Length != 2 && !Quaver.init.select.perColumn) || (split.Length != 5 && Quaver.init.select.perColumn))
                yield return "sendtochaterror Incorrect amount of values!";

            else if (!IsSubmitValid(split))
                yield return "sendtochaterror At least one value is invalid!";

            else
            {
                int offset = 0;

                if (Quaver.init.select.perColumn)
                    for (int i = 0; i < new[] { int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]) }.Min(); i++)
                    {
                        Quaver.Buttons[4].OnInteract();
                        offset++;
                        yield return new WaitForSeconds(0.05f);
                    }

                for (int i = 1; i < split.Length; i++)
                {
                    int value = int.Parse(split[i]);
                    for (int j = 0; j < value - offset; j++)
                    {
                        Quaver.Buttons[i - 1].OnInteract();
                        yield return new WaitForSeconds(0.05f);
                    }
                }
            }
        }

        else if (Regex.IsMatch(split[0], @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            
            int scrollSpeed = 0;

            if (Quaver.init.gameplay)
                yield return "sendtochaterror This command cannot run during gameplay!";

            else if (split.Length > 2)
                yield return "sendtochaterror Too many parameters!";

            else if (split.Length == 2 && !int.TryParse(split[1], out scrollSpeed) && (scrollSpeed == 0 || (scrollSpeed >= 10 && scrollSpeed <= 30)))
                yield return "sendtochaterror Parameter specified is invalid!";

            else
            {
                Quaver.Buttons[4].OnInteract();
                yield return new WaitForSeconds(1);

                while (scrollSpeed != 0 && scrollSpeed != ArrowScript.scrollSpeed)
                {
                    Quaver.Buttons[scrollSpeed < ArrowScript.scrollSpeed ? 1 : 2].OnInteract();
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        else if (Regex.IsMatch(split[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            int[] parameters;

            if (Quaver.init.gameplay)
                yield return "sendtochaterror This command cannot run during gameplay.";

            else if (split.Length != 4)
                yield return "sendtochaterror " + (split.Length < 4 ? "Too few parameters!" : "Too many parameters!");

            else if ((parameters = CommandToIndex(split)).Any(i => i == -1))
                yield return "sendtochaterror At least one parameter is invalid!";

            else
            {
                while (Quaver.init.select.ui != 0)
                {
                    Quaver.Buttons[1].OnInteract();
                    yield return new WaitForSeconds(0.05f);
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    while (parameters[i] != new[] { Quaver.init.select.speed, Quaver.init.select.difficulty, Convert.ToByte(Quaver.init.select.perColumn) }[i])
                    {
                        Quaver.Buttons[3].OnInteract();
                        yield return new WaitForSeconds(0.05f);
                    }

                    Quaver.Buttons[1].OnInteract();
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
