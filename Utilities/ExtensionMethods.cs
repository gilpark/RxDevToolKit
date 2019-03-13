using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DevToolKit.Utilities
{
    public static class ExtensionMethods
    {
        // float //
        public static float MapValue(this float f, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (toMax - toMin) * (f - fromMin) / (fromMax - fromMin) + toMin;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static IEnumerator AlphaFromTo(this CanvasGroup canvas, float from, float to, float duration)
        {
            float time = 0;
            canvas.alpha = from;
            while (time < duration)
            {
                yield return null;
                time += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(from, to, time);
            }
            canvas.alpha = to;
        }
        
        public static IEnumerator AlphaFromTo(this RawImage image, float @from, float to, float duration)
        {
            float time = 0;
            Color startColor = image.color;
            image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(@from, to, 0));
            while (time < duration)
            {
                yield return null;
                time += Time.deltaTime;
                image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(@from, to, time));
            }
            image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(@from, to, 1));
        }

        public static string GetCurrentDateTime(this string s, bool includeTime = true)
        {
            return includeTime ? DateTime.Now.ToString("dd-MM-yy_HH-mm-ss") : System.DateTime.Now.ToString("dd-MM-yy");
        }

        public static int Parse(this string s)
        {
            int num = 0;
            if (int.TryParse(s, out num)){}
            else
            {
                Debug.LogError("Inputted string - [ " + s + " ] - not a number, TryParse() failed... Assigning -1");
                num = -1;
            }
            return num;
        }

    }
}
