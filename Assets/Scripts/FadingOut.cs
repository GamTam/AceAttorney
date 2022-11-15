using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadingOut : MonoBehaviour
{
    Image rend;

    [SerializeField] private TextMeshProUGUI[] _textBoxes;

    void Start()
    {
        rend = GetComponent<Image>();
    }

    IEnumerator FadeOut()
    {
        for(float f = 1f; f >= -0.05f; f -= 0.50f)
        {
            for(int i = 0; i < _textBoxes.Length; i++)
            {
                Color Ex = _textBoxes[i].color;
                Ex.a = f;
                _textBoxes[i].color = Ex;
            }
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void startFading()
    {
        StartCoroutine(FadeOut());
    }
}
