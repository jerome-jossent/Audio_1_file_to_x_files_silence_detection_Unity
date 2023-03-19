using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bande : MonoBehaviour
{
    public float diametre_min;
    public float diametre_max;
    public GameObject roueA; //rotation
    public GameObject bandeA;//scale
    public GameObject roueB; //rotation
    public GameObject bandeB;//scale
    public LineRenderer bande;

    public float bande_posmini = -5.13f;
    public float bande_posmaxi = -3.68f;

    [Range(0f, 1f)]
    public float _progression;

    void Update()
    {
        float rA = _progression * (diametre_max - diametre_min) + diametre_min;
        float rB = (1 - _progression) * (diametre_max - diametre_min) + diametre_min;
        bandeA.transform.localScale = Vector3.one * rA;
        bandeB.transform.localScale = Vector3.one * rB;

        float x = (1 - _progression) * (bande_posmaxi - bande_posmini) + bande_posmini;
        bande.SetPosition(0, new Vector3(x, 3.21f, -1f));
        x = -_progression * (bande_posmaxi - bande_posmini) - bande_posmini;
        bande.SetPosition(bande.positionCount - 1, new Vector3(x, 3.21f, -1f));
    }
}
