﻿using System.Collections;
using System.Linq;
using UnityEngine;

internal class Generate
{
    internal Generate(QuaverScript quaver)
    {
        this.quaver = quaver;
    }

    private readonly QuaverScript quaver;
    private int length = 8, amplifier = 1;
    private string sequence;

    private static readonly string[][] patterns = new string[4][]
    {
        new string[] { "10", "01", "11" },
        new string[] { "1000", "0010", "1010", "0101", "1110", "1011", "1101", "0111",  "1111" },
        new string[] { "100000", "100100", "101010", "100101", "101100", "100111", "111100", "101101", "101110", "110101", "111101", "101111", "110111", "111110", "111111" },
        new string[] { "21112111", "31112111", "21113111", "31113111", "21212111", "21112121", "31212111", "31112121", "21312111", "21112131", "21213111", "21113121", "31312111", "31112131", "31213111", "31113121", "21313111", "21113131", "31313111", "31113131", "31212121", "21312121", "21213121", "21212131", "31312121", "31213121", "31212131", "21313121", "21312131", "21213131", "31313121", "31312131", "31213131", "21313131", /*"21212122", "21312122", "21213122", "21313122",*/ "21212221", "31212221", "31312221", "21312221", "21222121", "31222121", "21222131", "31222131", "22212121", "22213121", "22212131", "22213131", "21222221", "31222221", "22222121", "22222131", "22222221", /*"22222222",*/ "11111111" }
    };

    internal void Validate()
    {
        bool isCorrect = true;

        if (quaver.init.select.perColumn)
            for (int i = 0; i < quaver.ReceptorTexts.Length; i++)
            {
                if (quaver.ReceptorTexts[i].text == ArrowScript.arrowsPerColumn[i].ToString())
                    continue;

                isCorrect = false;
                break;
            }

        else
            isCorrect = int.Parse(quaver.ReceptorTotalText.text) == ArrowScript.arrowsPerColumn.Sum();

        if (isCorrect)
        {
            CalculateRating();
        }

        else
        {
            quaver.Audio.PlaySoundAtTransform("strike", quaver.transform);
            quaver.Module.HandleStrike();
        }

        ArrowScript.arrowsPerColumn = new int[4];
        ArrowScript.positionsUsed = new int[4];

        foreach (var text in quaver.ReceptorTexts)
            text.text = string.Empty;

        quaver.ReceptorTotalText.text = string.Empty;
    }

    private void CalculateRating()
    {
        quaver.Audio.PlaySoundAtTransform("note4", quaver.transform);

        float award = (quaver.init.select.difficulty + 0.25f) / 6 * (((float)quaver.init.select.speed / 10) + 1);
        if (!quaver.init.select.perColumn)
            award /= 2;
        if (quaver.init.select.difficulty == 3)
            award = 1;

        quaver.Render.ratingProgress += award;

        if (quaver.Render.ratingProgress >= 1)
        {
            quaver.Module.HandlePass();
            quaver.init.solved = true;
        }
    }

    internal IEnumerator Play(RenderScript render)
    {
        quaver.Audio.PlaySoundAtTransform("start", quaver.transform);

        int difficulty = quaver.init.select.difficulty;
        int speed = quaver.init.select.speed;

        amplifier = difficulty == 3 ? 5 : 1;
        length = difficulty == 3 ? 20 : 8;

        sequence = Pattern();

        yield return quaver.Render.Transition();
        yield return new WaitWhile(() => sequence == string.Empty);

        for (int i = 0; i < sequence.Length; i++)
        {
            if (Alter.CharToInt(sequence[i]) == 0)
                ArrowScript.positionsUsed = new int[4];

            for (int j = 0; j < Alter.CharToInt(sequence[i]); j++)
                render.CreateArrow(Alter.CharToInt(sequence[i]), IndexToColor(i));

            render.songProgress = (float)i / sequence.Length;

            yield return new WaitForSeconds(1 / (float)(GetSpeed(speed, difficulty == 3) * amplifier));
        }

        render.songProgress = 1;

        if (quaver.init.select.perColumn)
            foreach (var text in quaver.ReceptorTexts)
                text.text = "0";
        else
            quaver.Render.UpdateReceptorTotalText(0);

        quaver.init.ready = true;
    }

    internal string Pattern()
    {
        int difficulty = quaver.init.select.difficulty;
        string[] sequences = new string[difficulty == 3 ? 1 : difficulty + 1];

        for (int i = 0; i < sequences.Length; i++)
            for (int j = 0; j < length; j++)
                sequences[i] += patterns[difficulty][Random.Range(0, patterns[difficulty].Length)];

        string finalSequence = string.Empty;

        for (int i = 0; i < sequences[0].Length; i++)
        {
            int sum = 0;
            for (int j = 0; j < sequences.Length; j++)
                sum += Alter.CharToInt(sequences[j][i]);

            finalSequence += sum.ToString();
        }

        Debug.Log(finalSequence);
        return finalSequence;
    }

    private int IndexToColor(int index)
    {
        int difficulty = quaver.init.select.difficulty;
        index %= new[] { 2, 4, 6, 8 }[difficulty];
        return new int[4][] { new[] { 0, 1 }, new[] { 0, 3, 1, 3 }, new[] { 0, 4, 2, 1, 2, 4 }, new[] { 0, 5, 3, 5, 1, 5, 3, 5 } }[difficulty][index];
    }

    private float GetSpeed(int speed, bool expert = false)
    {
        return ((float)speed / (expert ? 5 : 10)) + 1;
    }
}