using System;

internal class Select
{
    internal Select(QuaverScript quaver)
    {
        this.quaver = quaver;
    }

    internal bool perColumn;
    internal int speed, difficulty, ui;

    private readonly QuaverScript quaver;

    internal KMSelectable.OnInteractHandler Press(int btn)
    {
        return delegate ()
        {
            quaver.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, quaver.Buttons[btn].transform);
            quaver.Buttons[btn].AddInteractionPunch();

            if (!quaver.init.gameplay)
                PressSelection(ref btn);
            else if (quaver.init.canAdjustScroll)
                PressPreGameplay(ref btn);
            else
                PressGameplay(ref btn);
            return false;
        };
    }

    private void PressGameplay(ref int btn)
    {
        if (!quaver.init.ready)
            return;

        if (quaver.init.select.perColumn)
            quaver.ReceptorTexts[btn].text = ((int.Parse(quaver.ReceptorTexts[btn].text) + 1) % (quaver.init.select.difficulty == 3 ? 200 : 50)).ToString();
        else
            quaver.Render.UpdateReceptorTotalText();

        quaver.Render.timer = 50;
    }

    private void PressPreGameplay(ref int btn)
    {
        switch (btn)
        {
            case 1: ArrowScript.scrollSpeed = Math.Max(--ArrowScript.scrollSpeed, 10); quaver.Audio.PlaySoundAtTransform("lower", quaver.transform); break;
            case 2: ArrowScript.scrollSpeed = Math.Min(++ArrowScript.scrollSpeed, 30); quaver.Audio.PlaySoundAtTransform("higher", quaver.transform); break;
        }

        quaver.Render.GameplayScroll.text = "Scroll Speed: " + ArrowScript.scrollSpeed;
    }

    private void PressSelection(ref int btn)
    {
        switch (btn)
        {
            case 0:
                switch (ui)
                {
                    case 0: speed = (--speed + 11) % 11; break;
                    case 1: difficulty = (--difficulty + 4) % 4; break;
                    case 2: perColumn = !perColumn; break;
                }
                quaver.Audio.PlaySoundAtTransform("lower", quaver.transform);
                break;

            case 1: ui++; quaver.Audio.PlaySoundAtTransform("select", quaver.transform); break;

            case 2: ui = Math.Max(--ui, 0); quaver.Audio.PlaySoundAtTransform("select", quaver.transform); break;

            case 3:
                switch (ui)
                {
                    case 0: speed = ++speed % 11; break;
                    case 1: difficulty = ++difficulty % 4; break;
                    case 2: perColumn = !perColumn; break;
                }
                quaver.Audio.PlaySoundAtTransform("higher", quaver.transform);
                break;
        }

        if (ui > 2)
        {
            ui = Init.anotherQuaverReady ? 2 : 0;

            if (!Init.anotherQuaverReady && !quaver.init.solved)
            {
                quaver.init.gameplay = true;
                Init.anotherQuaverReady = true;
            }
        }

        quaver.init.render.UpdateSelection();
    }
}
