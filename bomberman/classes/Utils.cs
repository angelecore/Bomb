using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bomberman.classes
{
    static class Utils
    {
        public static void NewFormOnTop(Form window, Form newForm)
        {
            window.Hide();
            newForm.Tag = window;
            newForm.StartPosition = FormStartPosition.Manual;
            newForm.Location = window.Location;
            newForm.Show(window);
        }

        public static Vector2f AddVectors(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2f AddDirection(Vector2f vector, Directions direction)
        {
            return AddVectors(vector, GetDirectionVector(direction));
        }

        public static Vector2f GetDirectionVector(Directions direction)
        {
            switch (direction)
            {
                case Directions.Up: 
                    return new Vector2f(0, -1);  
                case Directions.Down:
                    return new Vector2f(0, 1);
                case Directions.Left:
                    return new Vector2f(-1, 0);
                case Directions.Right:
                    return new Vector2f(1, 0);
            }
            return new Vector2f(0, 0);
        }
        
        public static Vector2f MultiplyVector(Vector2f vec, int number)
        {
            return new Vector2f(vec.X * number, vec.Y * number);
        }

        public static List<Tuple<Bitmap, string>> GetSpriteTuplesList()
        {
            return Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true)
                       .Cast<DictionaryEntry>()
                       .Where(x => x.Value.GetType() == typeof(Bitmap))
                       .Select(x => new Tuple<Bitmap, string>((Bitmap)x.Value, (string)x.Key))
                       .ToList();
        }

        public static string GetConcatenatedEventsForScore(List<Tuple<object, Player>> scoreEvents)
        {
            string concatenatedEvents = "";

            foreach (var scoreEvent in scoreEvents)
            {
                 concatenatedEvents += scoreEvent.Item1.ToString() + "." + scoreEvent.Item2.Id.ToString() + "-";
            }

            return concatenatedEvents;
        }

        public static bool CheckConcatenatedEventsForScoreHasGivenEvent(string concatenatedEventsForScore, string eventString)
        {
            var eventStrings = concatenatedEventsForScore.Split("-");

            foreach (var concatenatedEventString in eventStrings)
            {
                var splitString = concatenatedEventString.Split(".");

                if (splitString.Contains(eventString))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetConcatenatedEventsForScorePlayerId(string concatenatedEventsForScore, string eventString)
        {
            var eventStrings = concatenatedEventsForScore.Split("-");

            foreach (var concatenatedEventString in eventStrings)
            {
                var splitString = concatenatedEventString.Split(".");

                if (splitString.Contains(eventString))
                {
                    return splitString[1];
                }
            }

            return "";
        }
    }
}
