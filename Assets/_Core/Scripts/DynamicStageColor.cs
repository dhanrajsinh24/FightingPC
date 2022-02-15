using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicStageColor : MonoBehaviour
{
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private Material material;
    private List<Color32> randomColors = new List<Color32>();
    private Color32 requiredColor;
    private float timeToChange = 3f;
    private float timeInterval = 0.35f;
    private float startTime;
    private float currentTime;
    private void Start()
    {
        randomColors.Add(Color.red);
        randomColors.Add(Color.green);
        randomColors.Add(Color.blue);
        randomColors.Add(Color.yellow);
        randomColors.Add(Color.cyan);
        randomColors.Add(Color.magenta);
        randomColors = Shuffle(randomColors);
        material = GetComponent<MeshRenderer>().material;
        //material.SetColor(BaseColor, Color.white);
        startTime = Time.time;
    }
    
    private List<T> Shuffle<T>(List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            (ts[i], ts[r]) = (ts[r], ts[i]);
        }

        return ts;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        var counter = 0;
        for (var i = 0; i < randomColors.Count; i++)
        {
            if (!(currentTime > i * timeToChange) || !(currentTime <= (i + 1) * timeToChange)) continue;
            requiredColor = Color.Lerp(requiredColor, 
                randomColors[i], Time.deltaTime*timeInterval);
            counter++;
            break;
        }
        if (counter == 0)
        {
            currentTime = 0;
            startTime = startTime = Time.time;
        }

        material.SetColor(EmissionColor, requiredColor);
    }
}
