using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCharacters : MonoBehaviour
{
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private float _speed = 5;
    public bool _done;
    public string _charName;

    public void StartSwap(string newChar="NaN", float speed=-3, bool fadeIn=true, bool skipFade=false)
    {
        if (speed < 0)
        {
            speed = _speed;
        }
        StartCoroutine(Swap(newChar, speed, fadeIn, skipFade));
    }

    public IEnumerator Swap(string newChar, float speed, bool fadeIn, bool skipFade)
    {
        _done = false;
        
        if (_mesh.material.color.a != 0)
        {
            while (_mesh.material.color.a >= 0)
            {
                float a = _mesh.material.color.a;
                _mesh.material.color = new Color(1f, 1f, 1f, a - (speed * Time.deltaTime));
                
                yield return null;
            }

            _mesh.material.color = new Color(1f, 1f, 1f, 0f);
        }

        if (newChar != "NaN")
        {
            _mesh.material.mainTexture = (Resources.Load($"Material/{newChar}Mat", typeof(Material)) as Material).mainTexture;
            _charName = newChar;
            if (!skipFade) yield return new WaitForSeconds(0.1f);
        }
        
        if (_mesh.material.color.a != 1 && fadeIn)
        {
            if (!skipFade)
            {
                while (_mesh.material.color.a <= 1)
                {
                    float a = _mesh.material.color.a;
                    _mesh.material.color = new Color(1f, 1f, 1f, a + (speed * Time.deltaTime));
                    yield return null;
                }
            }

            _mesh.material.color = new Color(1f, 1f, 1f, 1f);
        }

        _done = true;
    }
}
