using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class TPScript : MonoBehaviour 
{
	public QuaverScript Quaver;

    private bool IsSubmitValid(string[] split)
    {
        ushort j, max = Quaver.init.select.difficulty == 3 ? Quaver.init.select.perColumn ? (ushort)80 : (ushort)320 : (ushort)200;
        for (int i = 0; i < split.Length; i++)
            if (!ushort.TryParse(split[i], out j) || j >= max)
                return false;
        return true;
    }

    private IEnumerator ProcessTwitchCommand(string command)
    {
        string[] split = command.Split();

        if (Regex.IsMatch(split[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

            if (!Quaver.init.ready)
                yield return "sendtochaterror The module isn't in submission mode.";

            else if ((split.Length != 2 && !Quaver.init.select.perColumn) || (split.Length != 6 && Quaver.init.select.perColumn))
                yield return "sendtochaterror Incorrect amount of values.";

            else if (!IsSubmitValid(split))
            {

            }
        }

        else if (Regex.IsMatch(split[0], @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

        }

        else if (Regex.IsMatch(split[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;

        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
