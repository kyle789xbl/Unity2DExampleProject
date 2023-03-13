using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class FadeDestroy : MonoBehaviour
{
    private TMP_Text _text;
    private Image _image;
    public float fadeSpeed = 1f;
    private int _fadeActivationDealy = 5;
    bool activateFade;
    // Start is called before the first frame update
    private void Awake()
    {
        _image = this.GetComponent<Image>();
        _text = this.GetComponentInChildren<TMP_Text>(true);
        _image.color = Color.white;
        _text.enabled = true;
        Invoke(nameof(ActivateUICharacterFade), _fadeActivationDealy);
    }
    private void ActivateUICharacterFade() =>  activateFade=true; 

    // Update is called once per frame
    void FixedUpdate()
    {
        bool destructionAlpha = _image.color.a <= 0.02 ? true : false;
        if (_image.color != Color.clear)
        {
            _image.color = Color.Lerp(_image.color, Color.clear,fadeSpeed);
            _text.color = Color.Lerp(_text.color, Color.clear, fadeSpeed);

            if(destructionAlpha)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
