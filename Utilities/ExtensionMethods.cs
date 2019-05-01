using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DevToolKit.Utilities
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
            while (time < _time)
            {
                yield return null;
                time += Time.deltaTime;
                _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, time));
            }
            _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, 1));
        }
        public static IEnumerator AlphaFromTo(this Image _image, float _from, float _to, float _time)
        {
            float time = 0;
            Color startColor = _image.color;
            _image.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(_from, _to, 0));
            while (time < _time)
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
        
        private static Color32[] ResampleAndCrop(this Color32[] source, int srcWidth, int srcHeight, int targetWidth, int targetHeight)
        {
            var sourceWidth = srcWidth;
            var sourceHeight = srcHeight;
            var sourceAspect = (float)sourceWidth / sourceHeight;
            var targetAspect = (float)targetWidth / targetHeight;
            var xOffset = 0;
            var yOffset = 0;
            var factor = 1f;
            if (sourceAspect > targetAspect)
            { // crop width
                factor = (float)targetHeight / sourceHeight;
                xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
            }
            else
            { // crop height
                factor = (float)targetWidth / sourceWidth;
                yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
            }
            var srcdata = source;
            var outdata = new Color32[targetWidth * targetHeight];
	     
            Parallel.For(0,targetHeight, y =>
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                    // bilinear filtering
                    var c11 = srcdata[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c12 = srcdata[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    var c21 = srcdata[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                    var c22 = srcdata[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                    outdata[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
                }
            });
            return outdata;
        }
        public static IObservable<int> ScaledUpdate(int fps)
        {
            var frameCount = 0;
            var timePerFrame = 1f / fps;
            var time = 0.0;
            return  Observable.EveryUpdate()
                .Select(x =>
                {
                    time += Time.unscaledDeltaTime;
                    if (!(time >= timePerFrame)) return frameCount;
                    time -= timePerFrame;
                    return frameCount++;
                }).DistinctUntilChanged();
        }
        public static List<List<T>> SplitList<T>(this IEnumerable<T> values, int groupSize, int? maxCount = null)
        {
            List<List<T>> result = new List<List<T>>();
            // Quick and special scenario
            if (values.Count() <= groupSize)
            {
                result.Add(values.ToList());
            }
            else
            {
                List<T> valueList = values.ToList();
                int startIndex = 0;
                int count = valueList.Count;
                int elementCount = 0;

                while (startIndex < count && (!maxCount.HasValue || (maxCount.HasValue && startIndex < maxCount)))
                {
                    elementCount = (startIndex + groupSize > count) ? count - startIndex : groupSize;
                    result.Add(valueList.GetRange(startIndex, elementCount));
                    startIndex += elementCount;
                }
            }

            return result;
        }
        public static IObservable<string> SaveToFile(this IObservable<MemoryStream>src, string filePath)
        {
            return src.SelectMany(ms =>
                Using(() => new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                    , fs =>
                    {
                        try
                        {
                            byte[] bytes = new byte[ms.Length];
                            ms.Read(bytes, 0, (int) ms.Length);
                            fs.Write(bytes, 0, bytes.Length);
                            ms.Close();
                            ms.Dispose();
                        }
                        catch (Exception e)
                        {
                            Observable.Throw<Exception>(e);
                            throw;
                        }

                        return Observable.Return(filePath);
                    }));
        }

        public static IObservable<TSource> Using<TSource, TResource>(
            Func<TResource> resourceFactory,
            Func<TResource, IObservable<TSource>> observableFactory)
            where TResource : IDisposable
        {
            return Observable.Create<TSource>(observer =>
            {
                var source = default(IObservable<TSource>);
                var disposable = Disposable.Empty;
                try
                {
                    var resource = resourceFactory();
                    if (resource != null)
                    {
                        disposable = resource;
                    }

                    source = observableFactory(resource);
                }
                catch (Exception exception)
                {
                    return new CompositeDisposable(Observable.Throw<TSource>(exception).Subscribe(observer), disposable);
                }

                return new CompositeDisposable(source.Subscribe(observer), disposable);
            });
        }
    }
}
