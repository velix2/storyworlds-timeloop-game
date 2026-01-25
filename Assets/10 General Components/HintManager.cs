using System;

namespace _10_General_Components
{
    public static class HintManager
    {
        public static void SayHint()
        {
            string text = "...";
            if (!StateTracker.IsInIntro)
            {
                // Evelyn quest tips
                switch (StateTracker.Evelyn.QuestState)
                {
                    case StateTracker.EvelynState.QuestStates.Init:
                        text = "Ich frage mich, was im Diner los ist...";
                        break;
                    case StateTracker.EvelynState.QuestStates.IntroCutsceneWatched:
                        text = "Die Dame sind genervt aus. Ich sollte mit ihr reden.";
                        break;
                    case StateTracker.EvelynState.QuestStates.TalkedTo:
                        text = "Ich muss irgendwie die Kaffeemaschine reparieren. Ich brauche Werkzeug...";
                        break;
                }
            }
            else
            {

                switch (StateTracker.IntroState)
                {
                    case StateTracker.IntroStates.LeftTrainCompleted:
                        text = "Keine Zeit zu verlieren. Ich sollte zu Oswald's Rarities...";
                        break;
                    case StateTracker.IntroStates.RarityShopCompleted:
                        text = "Ich habe Hunger. Da vorne gab's doch ein Diner, oder?";
                        break;
                    case StateTracker.IntroStates.DinerCompleted:
                        text = "Was ein Kaff voller komischer Vögel. Ab zum Bahnhof und nach Hause...";
                        break;
                    case StateTracker.IntroStates.NoTrainRunsCompleted:
                        text = "Na klasse, jetzt fährt auch noch kein Zug mehr. Ich sollte mir ein Zimmer im Motel nehmem";
                        break;
                    case StateTracker.IntroStates.MotelCompleted:
                        text =
                            "Ich habe wohl keine andere Wahl. Ich sollte zu Larry und dort nächtigen. Er meinte, er wohnt die Straße vom Motel runter.";
                        break;
                    case StateTracker.IntroStates.LarryWaitingCompleted:
                        text = "Nichts wie rein.";
                        break;
                    case StateTracker.IntroStates.LarrySweetHomeCompleted:
                        text = "Larry wollte was zum Essen machen. Ich sollte mich schon mal an den Tisch setzen.";
                        break;
                    case StateTracker.IntroStates.LarryDinnerCompleted:
                        text =
                            "Ich bin müde. Ab auf die Couch, auch wenn mir so ein richtiges Bett lieber gewesen wäre...";
                        break;
                    case StateTracker.IntroStates.SonCallCompleted:
                        text = "Neuer Tag, neues Glück. Ab zum Bahnhof und nach Hause.";
                        break;
                    case StateTracker.IntroStates.TimothyFirstEncounterCompleted:
                        text = "Das gibt es nicht! Immer noch kein Zug. Ich sollte mich bei Larry im Diner melden.";
                        break;
                    case StateTracker.IntroStates.DinerWithRadioCompleted:
                        text = "Gefangen in Echo's Lake. Schau ich mich halt ein wenig um. Wenn ich keine Lust mehr hab, werf ich mich wieder auf Larry's Couch...";
                        break;
                    case StateTracker.IntroStates.SonCallRepeatCompleted:
                        text = "Jetzt muss doch wirklich ein Zug fahren. Ab zum Bahnhof!";
                        break;
                    case StateTracker.IntroStates.NoTrainRuns2Completed:
                        text =
                            "Ich werde wirklich vom Pech verfolgt... Ich sag nochmal Larry Bescheid. Er sollte wieder im Diner sein.";
                        break;
                    case StateTracker.IntroStates.DinerWithRadioRepeatCompleted:
                        text = "Spinne ich jetzt komplett?";
                        break;

                }
            }
            
            DialogueManager.Instance.EnterDialogueModeSimple(text);
        }
    }
}