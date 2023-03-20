using NAudio_JJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;

public class test : MonoBehaviour
{
    public Color color_peaks;
    public Color color_silence;
    public string jsonfile = @"D:\C#\Audio_1_file_to_x_files_with_silence_detection\OneTrackToXTracks_SplitterAudio\bin\Debug\net7.0-windows\json.tmp";

    List<Peak> peaks;
    int index;
    int index_step;
    List<Silence> silences = new List<Silence>();
    GameObject parentPeaks;
    GameObject parentSilences;

    public bool drawPeaks;
    public bool findsilences;
    float t_total;
    [Range(0, 1)]
    double silence_sensibilite = 0.01;
    [Range(0, 10)]
    double silence_duree_sec = 1;

    private void Start()
    {
        peaks = Peak.Get_Peaks_FromJson(jsonfile);
        index_step = peaks.Count / 600;
        //100   :31.1
        //500   :11.3
        //600   :10.6   => 710 steps
        //700   :11.3
        //1000  :16.6
        index_step = 710;

        t_total = (float)peaks[peaks.Count - 1].temps;

        parentPeaks = new GameObject();
        parentPeaks.transform.parent = transform;
        parentPeaks.name = "parentPeaks";

        parentSilences = new GameObject();
        parentSilences.transform.parent = transform;
        parentSilences.name = "parentSilences";
    }

    private void Update()
    {
        if (findsilences)
        {
            findsilences = false;
            FindSilences();
            DrawSilences();
        }

        if (drawPeaks)
        {
            drawPeaks = false;
            StartCoroutine(DrawPeaks());
        }
    }

    IEnumerator DrawPeaks()
    {
        float socle = -0.00001f;
        index = 0;
        float t = 0;
        float t0 = Time.time;
        Debug.Log("Start");

        float offsetX = -0.5f;
        float offsetY = -0.5f;

        while (index < peaks.Count)
        {
            int index_stop = index + index_step;
            if (index_stop > peaks.Count)
                index_stop = peaks.Count;

            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(t + offsetX,
                                   socle + offsetY));
            int i;
            for (i = index; i < index_stop; i++)
                points.Add(new Vector2((float)peaks[i].temps / t_total + offsetX,
                                       (float)peaks[i].amplitude + offsetY));
            index = i;
            t = points[points.Count - 1].x - offsetX;
            points.Add(new Vector2(t + offsetX,
                                   socle + offsetY));

            GameObject go = new GameObject();
            go.name = index.ToString();
            go.transform.parent = parentPeaks.transform;
            Polygon2D polygon = go.AddComponent<Polygon2D>();
            polygon.points = points.ToArray();
            polygon.RGBA = color_peaks;
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log(Time.time - t0);
        yield return null;
    }

    void FindSilences()
    {
        silences = Silence.Get_Silences(peaks, silence_sensibilite, silence_duree_sec);
    }

    List<GameObject> quads = new List<GameObject>();
    void DrawSilences()
    {
        foreach (GameObject go in quads)
            Destroy(go);
        quads.Clear();


        float offsetX = -(transform.localScale.x / 2 - transform.localPosition.x);
        float offsetY = -(transform.localScale.y / 2 - transform.localPosition.y);

        offsetX = offsetY = 0;

        //Debug.Log(offsetX);
        //Debug.Log(offsetY);

        foreach (var silence in silences)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

            Material material = quad.GetComponent<Renderer>().material;
            material = new Material(Shader.Find("Unlit/Color"));
            material.color = color_silence;
            quad.GetComponent<Renderer>().material = material;

            quad.transform.parent = parentSilences.transform;
            quad.transform.position = new Vector3((float)silence.milieu / t_total - 0.5f + offsetX, 1 - 0.9f / 2 + offsetY - 0.5f, 0);
            quad.transform.localScale = new Vector3((float)silence.duree / t_total, 0.9f);

            quads.Add(quad);
        }
    }

}
