using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.RxDevKit.Utilities
{
    public static class ExtensionMethods
    {
        // float //
        public static float FromTo(this float _f, float _fromMin, float _fromMax, float _toMin, float _toMax)
        {
            float returnFloat = (((_toMax - _toMin) * (_f - _fromMin)) / (_fromMax - _fromMin)) + _toMin;
            return returnFloat;
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

        public static IEnumerator AlphaFromTo(this RawImage _image, float _from, float _to, float _time)
        {
            float time = 0;
            Color startColor = _image.color;
            _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, 0));
            while (time < 1)
            {
                yield return null;
                time += Time.deltaTime;
                _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, time));
            }
            _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, 1));
        }

        public static string GetCurrentDateTime(this string _s, bool _includeTime = true)
        {
            return (_includeTime)
                ? DateTime.Now.ToString("dd-MM-yy_HH-mm-ss")
                : System.DateTime.Now.ToString("dd-MM-yy");
        }

        public static int Parse(this string s)
        {
            int num = 0;

            if (int.TryParse(s, out num))
            {
                //Was assigned
            }
            else
            {
                Debug.LogError("Inputted string - [ " + s + " ] - not a number, TryParse() failed... Assigning -1");
                num = -1;
            }

            return num;
        }

    }
}